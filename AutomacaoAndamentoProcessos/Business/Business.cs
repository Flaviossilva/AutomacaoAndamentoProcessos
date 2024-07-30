using OpenQA.Selenium;
using AutomacaoAndamentoProcessos.Models;
using System.Diagnostics;
using static AutomacaoAndamentoProcessos.Models.OrgEnum;
using static AutomacaoAndamentoProcessos.Automator;
using System.Threading;
using OpenQA.Selenium.Chrome;

namespace AutomacaoAndamentoProcessos.Business
{

    public class Business
    {
        public async Task Start()
        {
            #region Inicializadores
            Repository.Repository _repository = new();
            List<Solicitacao> _solicitacao = new();
            Service _service = new();
            IWebDriver? driver;
            ChromeDriver? chromeDriver;
            int? ComparaOrgao;
            #endregion

            try
            {
                _service.MatarProcessos();
                _repository.AlimentarFila();
                _service.RetirarFila();
                _service.GerarOrg();
                //Buscar casos e alterar status para Processando
                _solicitacao = _repository.RetornaSolicitacoes();
                while (_solicitacao.Count > 0)
                {
                    driver = _service.IniciarChrome(_solicitacao[0]);
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
                    //Limapa a Lista e Refaz o loop.
                    _solicitacao.Clear();
                    _solicitacao = _repository.RetornaSolicitacoesAtrasadas();
                    if (_solicitacao.Count == 0)
                        _solicitacao = _repository.RetornaSolicitacoes();
                }
                //volta para pendente os casos que ficaram presos em processando
                _repository.AlterarProcessando();
                //Logica para Reprocessar casos com erros e parados em processamento
                _solicitacao = _repository.RetornaSolicitacoes();
                if (_solicitacao.Count > 0)
                    Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                GerarLog(ex.ToString());
                _repository.GravarLogErro(_solicitacao[0]);
                Restart();
                throw;
            }
        }


        public void Restart()
        {
            GerarLog("Aplicação Encerrada");
            //Mata o chrome driver caso ele esteja aberto em segundo plano
            Process[] chromeDriverProcesses = Process.GetProcessesByName("chromedriver");
            foreach (var chromeDriverProcess in chromeDriverProcesses)
            {
                chromeDriverProcess.Kill();
            }
            Process[] chrome = Process.GetProcessesByName("chrome");
            foreach (var chromeDriverProcess in chrome)
            {
                chromeDriverProcess.Kill();
            }
            Thread.Sleep(900000);
            Start();
        }

        public void Stop()
        {
            GerarLog("Aplicação Encerrada");
            //Mata o chrome driver caso ele esteja aberto em segundo plano
            Process[] chromeDriverProcesses = Process.GetProcessesByName("chromedriver");
            foreach (var chromeDriverProcess in chromeDriverProcesses)
            {
                chromeDriverProcess.Kill();
            }
            Process[] chrome = Process.GetProcessesByName("chrome");
            foreach (var chromeDriverProcess in chrome)
            {
                chromeDriverProcess.Kill();
            }
            Process.GetCurrentProcess().Kill();
        }
        public static void GerarLog(string Mensagem, int? NRegistros = null)
        {
            richTextBox1.AppendText($"{DateTime.Now:MM/dd/yy HH:mm:ss}: // {Mensagem}{NRegistros}" + Environment.NewLine);
        }
    }
}
