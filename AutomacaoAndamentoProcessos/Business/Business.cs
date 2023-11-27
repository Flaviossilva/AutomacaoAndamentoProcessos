using OpenQA.Selenium;
using AutomacaoAndamentoProcessos.Models;
using System.Diagnostics;
using static AutomacaoAndamentoProcessos.Models.OrgEnum;
using static AutomacaoAndamentoProcessos.Automator;
using System.Threading;

namespace AutomacaoAndamentoProcessos.Business
{

    public class Business
    {
        public void Start()
        {
            #region Inicializadores
            Repository.Repository _repository = new();
            List<Solicitacao> _solicitacao = new();
            Service _service = new();
            IWebDriver? driver;
            int? ComparaOrgao;
            #endregion

            try
            {
                GerarLog("Iniciando Automação");
                _repository.AlimentarFila();
                GerarLog("Alimentando Fila");
                _service.RetirarFila();
                GerarLog("Gerando Orgs");
                _service.GerarOrg();
                //Buscar casos e alterar status para Processando
                _solicitacao = _repository.RetornaSolicitacoes();
                while (_solicitacao.Count > 0)
                {
                    GerarLog("Iniciando Chrome");
                    driver = _service.IniciarChrome(_solicitacao[0]);
                    GerarLog("Chrome iniciado");
                    foreach (var AtualizaProcessamento in _solicitacao)
                    {
                        _repository.AlterarConsultados(AtualizaProcessamento);
                    }
                    ComparaOrgao = Convert.ToInt32(_solicitacao[0].Orgao);
                    //Seleciona a Navegação  usando orgão       
                    switch (ComparaOrgao)
                    {
                        case int _ when (ComparaOrgao == (int)Orgs.TJSP):
                            GerarLog("Iniciando Navegação TJSP");
                            _service.NavegacaoTJSP(_solicitacao, driver);
                            break;
                        case int _ when (ComparaOrgao == (int)Orgs.PJE):
                            GerarLog("Iniciando Navegação PJE");
                            _service.NavegacaoPJE(_solicitacao, driver);
                            break;
                        case int _ when (ComparaOrgao == (int)Orgs.STJ):
                            GerarLog("Iniciando Navegação STJ");
                            _service.NavegacaoSTJ(_solicitacao, driver);
                            break;
                    }
                    GerarLog("Foi Processado o Lote");
                    //Limapa a Lista e Refaz o loop.
                    _solicitacao.Clear();
                    _solicitacao = _repository.RetornaSolicitacoes();
                }
                GerarLog("Verificando Processos com erros");
                //volta para pendente os casos que ficaram presos em processando
                _repository.AlterarProcessando();
                //Logica para Reprocessar casos com erros e parados em processamento
                _solicitacao = _repository.RetornaSolicitacoes();
                if (_solicitacao.Count > 0)
                {
                    GerarLog("Iniciando Reprocessamento");
                    Start();
                }
                GerarLog("Fim do Processamento");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro inesperado {ex.ToString()[..50]}");
                GerarLog(ex.ToString());
                _repository.GravarLogErro(_solicitacao[0]);
                throw;
            }
        }
        public void Stop()
        {
            GerarLog("Aplicação Encerrada");
            Process.GetCurrentProcess().Kill();
        }
        public static void GerarLog(string Mensagem, int? NRegistros = null)
        {
            richTextBox1.AppendText($"{DateTime.Now:MM/dd/yy HH:mm:ss}: // {Mensagem}{NRegistros}" + Environment.NewLine);
        }
    }
}
