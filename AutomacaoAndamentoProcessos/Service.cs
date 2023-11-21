using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomacaoAndamentoProcessos.Models;
using static AutomacaoAndamentoProcessos.Models.StatusEnum;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Net.Mail;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Xml.Linq;
using System.Management;
using System.Net.NetworkInformation;
using Windows.Networking.BackgroundTransfer;
using OpenQA.Selenium.Remote;
using System.Security.Policy;

namespace AutomacaoAndamentoProcessos
{
    public class Service
    {
        #region Inicializadores
        readonly Repository.Repository _repository = new();
        readonly List<string> TextosAdcionar = new();
        ProcessoAtual _ProcessoAtual = new();
        List<Solicitacao> _solicitacao = new();
        string? TextoTratadoInserir;
        string? AndamentoUm;
        string? AndamentoDois;
        string? TextoErro;
        string? TextoTratadoInserirDois;
        string? TextoTratadoInserirUm;
        string[]? TextoTratadoUm;
        string[]? TextoTratado;
        string[] TextoTratadoUmInfo = { "", "" };
        string[] TextoTratadoDoisInfo = { "", "" };
        string[]? TextoTratadoDois;
        bool VerificaNumeros;
        int c = 0;
        ChromeDriver? driver;

        DateTime DataUltimaLinha;
        DateTime DataLinhaAtual;

        IWebElement? VerificarProcessos;
        IWebElement? TextoUltimaFase;
        IWebElement? BtnNumeroProcesso;
        IWebElement? BtnPesquisar;
        IWebElement? LinkDetalhesProcesso;
        IWebElement? GridProcesso;
        IWebElement? TextoUltimaFaseUm;
        IWebElement? BtnExpandirDois;
        IWebElement? TextoUltimaFaseDois;
        IWebElement? InfoProcessoUm;
        IWebElement? InfoProcessoDois;
        IWebElement? AbaFases;
        //IWebElement? Indisponivel;
        IWebElement? BtnExpandir;
        IWebElement? ValidaPagina;
        //IWebElement? VerificaHumano;
        //IWebDriver? VerificaFrameHumano;
        readonly ChromeOptions options = new();
        readonly Proxy proxy = new();

        #endregion

        public void GerarOrg()
        {
            _solicitacao = Repository.Repository.RetornaSolicitacoesOrg();
            foreach (var Solicitacao in _solicitacao)
            {
                if (Solicitacao.NumeroProcesso != null)
                {
                    if (Solicitacao.NumeroProcesso.Length == 25)
                    {
                        string verif = Solicitacao.NumeroProcesso.Substring(18, 2);
                        if (verif.Contains("26"))
                            _repository.InserirOrgTabela("1", Solicitacao.NumeroProcesso);
                        else
                            _repository.InserirOrgTabela("2", Solicitacao.NumeroProcesso);
                    }
                }
            }
            _repository.InserirOrgNull();
        }

        static void EnviarEmail(string erro)
        {
            string remetente = "AutProcessos@gmail.com";
            string senha = "Automacao123";
            string destinatario = "Flavio_luno@Hotmail.com";
            string assunto = "WebDriver";
            string corpo = $"Erro ao inciar Webdriver com a seguinte mensagem {erro}";
            try
            {
                // Configurações do servidor SMTP (neste exemplo, estamos usando o Gmail)
                SmtpClient smtpClient = new SmtpClient("smtp.office365.com")
                {

                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(remetente, senha),
                    Port = 587,
                    Host = "smtp.office365.com",
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = true,
                };
                // Construir a mensagem de email
                MailMessage mensagem = new MailMessage(remetente, destinatario, assunto, corpo);
                // Enviar o email
                smtpClient.Send(mensagem);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao enviar o email: " + ex.Message);
            }
        }

        public void RetirarFila()
        {
            try
            {
                _solicitacao = Repository.Repository.RetornaSolicitacoesEncerradas();
                foreach (var Solicitacao in _solicitacao)
                {
                    _repository.RetiraFila(Solicitacao.NumeroProcesso);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Solicitacao> NavegacaoPJE(List<Solicitacao> Processos, IWebDriver Driver)
        {
            try
            {
                Driver.Url = "https://pje1g.trf3.jus.br/pje/ConsultaPublica/listView.seam";
                foreach (Solicitacao processo in Processos)
                {
                    Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                    BtnNumeroProcesso = Driver.FindElement(By.XPath("/html/body/div[5]/div/div/div/div[2]/form/div[1]/div/div/div/div/div[1]/div/div[2]/input"));
                    if (BtnNumeroProcesso.Displayed)
                    {
                        BtnNumeroProcesso.Click();
                        Driver.FindElement(By.XPath("/html/body/div[5]/div/div/div/div[2]/form/div[1]/div/div/div/div/div[1]/div/div[2]/input")).SendKeys(processo.NumeroProcesso);
                        BtnPesquisar = Driver.FindElement(By.XPath("/html/body/div[5]/div/div/div/div[2]/form/div[1]/div/div/div/div/div[8]/div/input"));
                        if (BtnPesquisar.Displayed)
                            BtnPesquisar.Click();

                        Thread.Sleep(100);
                        var ProcessoNEncontrado = EsperarElemento(driver, "XPath", "/html/body/div[5]/div/div/div/div[2]/form/div[2]/div/dl/dt/span");
                        if (ProcessoNEncontrado != null)
                        {
                            //Processo não encontrado, retira da fila e pula para o proximo
                            _repository.RetirarTabelaFila(processo);
                            continue;
                        }

                        LinkDetalhesProcesso = Driver.FindElement(By.XPath("/html/body/div[5]/div/div/div/div[2]/form/div[2]/div/table/tbody/tr/td[1]/a/i"));
                        if (LinkDetalhesProcesso.Displayed)
                        {
                            LinkDetalhesProcesso.Click();
                            Thread.Sleep(100);
                            Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                            Driver.Manage().Window.Maximize();
                        }
                        GridProcesso = Driver.FindElement(By.Id("j_id131:processoEvento"));
                        TextoTratado = GridProcesso.Text.ToString().Split("\r\n");
                        TextoTratadoInserir = TextoTratado[2].TrimStart().TrimEnd();
                        DataUltimaLinha = Convert.ToDateTime(TextoTratadoInserir[..10]);

                        foreach (var item in TextoTratado)
                        {
                            if (item.Length > 10)
                                DataLinhaAtual = Convert.ToDateTime(item[..10]);
                            if (DataUltimaLinha <= DataLinhaAtual && (!TextoTratadoInserir.First().Equals(item)))
                                processo.Andamento.Add(item.ToString().TrimStart().TrimEnd());
                        }

                        //Chamada Sql para Inserir registro no Banco
                        processo.Status = (int)Status.Processado;
                        _ProcessoAtual = Repository.Repository.RetornarDataUltmoRegistro(processo.Pi);
                        if (_ProcessoAtual.Andamento.Contains("PROCESSO CONSULTADO PELA AUTOMAÇÃO"))
                        {
                            if (_ProcessoAtual.Dt_Ult_Acao >= DataUltimaLinha
                        && _ProcessoAtual.Andamento != null)
                            {
                                //sem registro novo então Atualizar data texto padrão
                                _repository.AtualizarTextoTabela(_ProcessoAtual);
                            }
                            else
                            {
                                _repository.AtualizarInserirTextoTabela(processo);
                                _repository.AtualizarInserirTextoTabela(processo);
                            }
                        }
                        else
                        {
                            if (_ProcessoAtual.Dt_Ult_Acao >= DataUltimaLinha
                       && _ProcessoAtual.Andamento != null)
                            {
                                //sem registro novo então Atualizar data texto padrão
                                _repository.AtualizarInserirTextoTabela(processo);
                            }
                            else
                            {
                                _repository.InserirTextoTabelaFila(processo);
                                _repository.AtualizarInserirTextoTabela(processo);
                            }
                        }
                        _repository.InserirTextoTabelaAndamento(processo);

                        Driver.Quit();

                        if (GridProcesso.Displayed)
                            Driver.Quit();
                    }
                }
                return Processos;
            }
            catch (Exception ex)
            {
                Processos.First().Log_Erro = ex.Message;
                driver.Quit();

                //_repository.GravarLogErro(Processos.First());
                return null;
            }
        }
        public List<Solicitacao> NavegacaoTJSP(List<Solicitacao> Processos, IWebDriver driver)
        {
            try
            {
                foreach (Solicitacao processo in Processos)
                {
                    List<string> TextosAdcionar = new();
                    IWebElement? BtnOutros = null;
                    IWebElement? BtnConsultar = null;
                    IWebElement? ProcNaoExiste = null;
                    DateTime DataLinhaAtualVerifica1;
                    string? dataVerifica1 = null;
                    DateTime DataLinhaAtualVerifica;
                    string? dataVerifica = null;
                    string? Juntarlinhas = null;
                    processo.Operador = "AUTOMACAO";
                    c = 0;

                    driver.Url = "https://esaj.tjsp.jus.br/cpopg/open.do";
                    Thread.Sleep(900);
                    BtnOutros = EsperarElemento(driver, "XPath", ("/html/body/div[2]/form/section/div[2]/div/div[1]/div[1]/div/fieldset/label[2]"));
                    if (BtnOutros != null)
                        BtnOutros.Click();

                    BtnNumeroProcesso = driver.FindElement(By.XPath("/html/body/div[2]/form/section/div[2]/div/div[1]/div[1]/span[2]/input"));
                    if (BtnNumeroProcesso.Displayed)
                    {
                        BtnNumeroProcesso.Click();
                        driver.FindElement(By.XPath("/html/body/div[2]/form/section/div[2]/div/div[1]/div[1]/span[2]/input")).SendKeys(processo.NumeroProcesso);
                        BtnConsultar = driver.FindElement(By.XPath("/html/body/div[2]/form/section/div[4]/div/input"));
                        if (BtnConsultar.Displayed)
                        {
                            BtnConsultar.Click();
                            Thread.Sleep(500);
                            ProcNaoExiste = EsperarElemento(driver, "XPath", "/html/body/div[2]/div[1]/table/tbody/tr[2]/td[2]");
                            if (ProcNaoExiste != null)
                            {
                                //Processo não encontrado, retira da fila e pula para o proximo
                                _repository.RetirarTabelaFila(processo);
                                continue;
                            }
                            //Verificar outros Processos Atrelados                 
                            VerificarProcessos = EsperarElemento(driver, "XPath", "/html/body/div[1]/div[2]/div/div[2]/div[5]/div/a");
                            if (VerificarProcessos == null)
                                VerificarProcessos = EsperarElemento(driver, "ClassName", "processoPrinc");
                            VerificarProcessos?.Click();

                            //Raspa Textos
                            TextoUltimaFase = EsperarElemento(driver, "XPath", "/html/body/div[2]/table[2]/tbody[1]");
                            if (TextoUltimaFase != null)
                                TextoTratado = TextoUltimaFase.Text.ToString().Split("\r\n");

                            if (TextoTratado.Length <= 4 || TextoTratado[0].Contains("Autor  Justiça Pública"))
                            {
                                if (TextoTratado[0].Length < 9)
                                {
                                    TextoUltimaFase = driver.FindElement(By.XPath("/html/body/div[2]/table[3]"));
                                    TextoTratado = TextoUltimaFase.Text.ToString().Split("\r\n");
                                }
                                dataVerifica1 = TextoTratado[0][..10];
                                VerificaNumeros = DateTime.TryParse(dataVerifica1, out DataLinhaAtualVerifica1);
                                if (!VerificaNumeros)
                                {
                                    TextoUltimaFase = driver.FindElement(By.XPath("/html/body/div[2]/table[3]"));
                                    TextoTratado = TextoUltimaFase.Text.ToString().Split("\r\n");
                                }
                            }
                            if (TextoTratado.First().Contains("Data   Movimento"))
                                TextoTratado = TextoTratado.Where(o => o != TextoTratado[0]).ToArray();

                            //Juntar Texto
                            foreach (string itemJuntar in TextoTratado)
                            {
                                Juntarlinhas = "";
                                if (itemJuntar.Length > 9)
                                    dataVerifica = itemJuntar[..10];
                                else
                                    dataVerifica = "Sem Data";
                                VerificaNumeros = DateTime.TryParse(dataVerifica, out DataLinhaAtualVerifica);
                                if (!VerificaNumeros)
                                {
                                    Juntarlinhas = TextoTratado[c - 1] + " " + itemJuntar;
                                    TextosAdcionar.Add(Juntarlinhas);
                                }
                                c++;
                            }

                            DataUltimaLinha = Convert.ToDateTime(TextosAdcionar.First()[..10]).AddDays(-1);
                            foreach (string data in TextosAdcionar)
                            {
                                string num = data[..10];
                                DateTime numeric;
                                bool isNumeric = DateTime.TryParse(num, out numeric);
                                if (isNumeric)
                                {
                                    if (numeric > DataUltimaLinha)
                                        DataUltimaLinha = numeric;
                                }
                            }
                            DataUltimaLinha = DataUltimaLinha.AddDays(-1);
                            //tratar Data para Verificações 
                            foreach (string item in TextosAdcionar)
                            {
                                string num1 = item[..10];
                                DateTime numeric1;
                                bool isNumeric1 = DateTime.TryParse(num1, out numeric1);
                                if (isNumeric1)
                                {
                                    DataLinhaAtual = Convert.ToDateTime(item[..10]);
                                    if (DataUltimaLinha <= DataLinhaAtual)
                                        processo.Andamento.Add(item);
                                }
                            }

                            //Chamada Sql para Inserir registro no Banco
                            processo.Status = (int)Status.Processado;
                            _ProcessoAtual = Repository.Repository.RetornarDataUltmoRegistro(processo.Pi);
                            if (_ProcessoAtual.Andamento.Contains("PROCESSO CONSULTADO PELA AUTOMAÇÃO"))
                            {
                                if (_ProcessoAtual.Dt_Ult_Acao >= DataUltimaLinha
                            && _ProcessoAtual.Andamento != null)
                                {
                                    //sem registro novo então Atualizar data texto padrão
                                    _repository.AtualizarTextoTabela(_ProcessoAtual);
                                }
                                else
                                {
                                    _repository.AtualizarInserirTextoTabela(processo);
                                    _repository.AtualizarInserirTextoTabela(processo);
                                }
                            }
                            else
                            {
                                if (_ProcessoAtual.Dt_Ult_Acao >= DataUltimaLinha
                           && _ProcessoAtual.Andamento != null)
                                {
                                    //sem registro novo então Atualizar data texto padrão
                                    _repository.AtualizarInserirTextoTabela(processo);
                                }
                                else
                                {
                                    _repository.InserirTextoTabelaFila(processo);
                                    _repository.AtualizarInserirTextoTabela(processo);
                                }
                            }
                            _repository.InserirTextoTabelaAndamento(processo);
                        }
                    }
                }
                driver.Quit();
                return Processos;
            }
            catch (Exception ex)
            {
                Processos.First().Log_Erro = ex.Message;
                driver.Quit();

                //_repository.GravarLogErro(Processos.First());
                return null;
            }
        }
        public List<Solicitacao> NavegacaoSTJ(List<Solicitacao> Processos, IWebDriver driver)
        {
            try
            {
                TextoErro = " ";
                foreach (Solicitacao processo in Processos)
                {
                    #region Desativado

                    //CODIGO DESATIVADO POR MOTIVO DE FALTA DE ACESSO A PAGINA,QUANDO QUEBRAR SEGURANÇA DA PAGINA DESCOMENTAR

                    //IWebElement NumeroProcesso = driver.FindElement(By.XPath("/html/body/div[2]/div[2]/div[2]/section/div[2]/div[1]/form/div/input[1]"));
                    //if (NumeroProcesso != null)
                    //{
                    //    NumeroProcesso.Click();
                    //    driver.FindElement(By.XPath("/html/body/div[2]/div[2]/div[2]/section/div[2]/div[1]/form/div/input[1]")).SendKeys("2214881/SP");

                    //    IWebElement? Pesquisar = EsperarElemento(driver, By.XPath, "/html/body/div[2]/div[2]/div[2]/section/div[2]/div[1]/form/div/div/button");
                    //    if (Pesquisar != null)
                    //    {
                    //        Pesquisar.Click();
                    #endregion
                    driver.Quit();
                    driver = IniciarChrome(processo);
                    Thread.Sleep(300);

                    driver.Url = $"https://processo.stj.jus.br/processo/pesquisa/?termo={processo.NumeroProcesso}&aplicacao=processos.ea&tipoPesquisa=tipoPesquisaGenerica&chkordem=DESC&chkMorto=MORTO";
                    Thread.Sleep(3000);
                    //Valida se foi redirecionado para pagina de validação Humana e quebra o captcha
                    ValidaPagina = EsperarElemento(driver, "XPath", "/html/body/div[2]/div[6]/div/div/div[3]/div[1]/span[1]");
                    if (ValidaPagina == null)
                    {
                        Thread.Sleep(55000);
                        driver.Quit();
                        driver = IniciarChrome(processo);
                        driver.Url = $"https://processo.stj.jus.br/processo/pesquisa/?termo={processo.NumeroProcesso}&aplicacao=processos.ea&tipoPesquisa=tipoPesquisaGenerica&chkordem=DESC&chkMorto=MORTO";


                        //VerificaFrameHumano = driver.SwitchTo().Frame(driver.FindElement(By.XPath("/html/body/div[1]/div/div[1]/div/div/iframe")));
                        //if (VerificaFrameHumano != null)
                        //    VerificaHumano = driver.FindElements(By.XPath("/html/body/div/div/div[1]/div/label/input")).FirstOrDefault();
                        //if (VerificaHumano != null)
                        //{
                        //    VerificaHumano.Click();
                        //    Thread.Sleep(35000);
                        //}
                        //Indisponivel = EsperarElemento(driver, "XPath", "/html/body/div[2]/div[6]/div/div/div[3]/div[2]/div/div");
                        //if (Indisponivel != null)
                        //    TextoErro = Indisponivel.Text.ToString();

                        //if (TextoErro.Equals("Sistema indisponível."))
                        //    break;
                    }

                    var ProcessoNExiste = EsperarElemento(driver, "XPath", "/html/body/div[2]/div[6]/div/div/div[3]/div[2]/div/div/div[2]/div[2]/span[2]/div[2]/div");
                    if (ProcessoNExiste != null)
                    {
                        //Processo não encontrado, retira da fila e pula para o proximo
                        _repository.RetirarTabelaFila(processo);
                        continue;
                    }
                    var ProcessoNExiste2 = EsperarElemento(driver, "XPath", "/html/body/div[2]/div[6]/div/div/div[3]/div[2]/div/div/div[3]/div[2]/span[2]/div[2]/div");
                    if (ProcessoNExiste2 != null)
                    {
                        //Processo não encontrado, retira da fila e pula para o proximo
                        _repository.RetirarTabelaFila(processo);
                        continue;
                    }

                    AbaFases = EsperarElemento(driver, "XPath", "/html/body/div[2]/div[6]/div/div/div[3]/div[2]/div/div/div[3]/div[2]/span[2]/div[4]/span[2]/a");
                    if (AbaFases != null)
                    {
                        AbaFases.Click();
                        Thread.Sleep(200);
                        TextoUltimaFase = driver.FindElement(By.XPath("/html/body/div[2]/div[6]/div/div/div[3]/div[2]/div/div/div[3]/div[2]/span[2]/div[5]/div[2]/div[2]"));
                        TextoTratado = TextoUltimaFase.Text.ToString().Split("\r\n");
                        TextoTratadoInserir = TextoTratado.First().TrimStart().TrimEnd() + "\\";
                        DataUltimaLinha = Convert.ToDateTime(TextoTratado.First()[..10]);
                        foreach (string item in TextoTratado)
                        {
                            VerificaNumeros = DateTime.TryParse(item[..10], out DataLinhaAtual);
                            if (VerificaNumeros && DataUltimaLinha <= DataLinhaAtual && (!TextoTratadoInserir.Equals(item)))
                                processo.Andamento.Add(item.ToString().TrimStart().TrimEnd());
                        }
                    }
                    else
                    {
                        BtnExpandir = driver.FindElement(By.XPath("/html/body/div[2]/div[6]/div/div/div[3]/div[2]/div/div/span[2]/div[3]/div/div[2]/span[1]/span[2]/span[2]/input"));
                        if (BtnExpandir.Displayed)
                        {
                            BtnExpandir.Click();
                            TextoUltimaFaseUm = driver.FindElement(By.XPath("/html/body/div[2]/div[6]/div/div/div[3]/div[2]/div/div/span[2]/div[3]/div/div[2]/div/span[6]"));
                            TextoTratadoUm = TextoUltimaFaseUm.Text.ToString().Split("\r\n");
                            TextoTratadoInserirUm = TextoTratadoUm[1].TrimStart().TrimEnd();
                        }
                        BtnExpandirDois = driver.FindElement(By.XPath("/html/body/div[2]/div[6]/div/div/div[3]/div[2]/div/div/span[2]/div[3]/div/div[3]/span[1]/span[2]/span[2]/input"));
                        if (BtnExpandirDois.Displayed)
                        {
                            BtnExpandirDois.Click();
                            TextoUltimaFaseDois = driver.FindElement(By.XPath("/html/body/div[2]/div[6]/div/div/div[3]/div[2]/div/div/span[2]/div[3]/div/div[3]/div/span[6]"));
                            TextoTratadoDois = TextoUltimaFaseDois.Text.ToString().Split("\r\n");
                            TextoTratadoInserirDois = TextoTratadoDois[1].TrimStart().TrimEnd();
                        }

                        InfoProcessoUm = driver.FindElement(By.XPath("/html/body/div[2]/div[6]/div/div/div[3]/div[2]/div/div/span[2]/div[3]/div/div[2]/span[1]/span[1]"));
                        TextoTratadoUmInfo = InfoProcessoUm.Text.ToString().Split("\r\n");
                        InfoProcessoDois = driver.FindElement(By.XPath("/html/body/div[2]/div[6]/div/div/div[3]/div[2]/div/div/span[2]/div[3]/div/div[3]/span[1]/span[1]"));
                        TextoTratadoDoisInfo = InfoProcessoDois.Text.ToString().Split("\r\n");
                        AndamentoUm = $"{TextoTratadoInserirUm} {TextoTratadoUmInfo.FirstOrDefault()}\\";
                        AndamentoDois = $"{TextoTratadoInserirDois} {TextoTratadoDoisInfo.FirstOrDefault()}\\";
                        processo.Andamento.Add(AndamentoUm);
                        processo.Andamento.Add(AndamentoDois);
                    }
                    //Chamada Sql para Inserir registro no Banco
                    processo.Status = (int)Status.Processado;
                    _ProcessoAtual = Repository.Repository.RetornarDataUltmoRegistro(processo.Pi);
                    if (_ProcessoAtual.Andamento.Contains("PROCESSO CONSULTADO PELA AUTOMAÇÃO"))
                    {
                        if (_ProcessoAtual.Dt_Ult_Acao >= DataUltimaLinha
                    && _ProcessoAtual.Andamento != null)
                        {
                            //sem registro novo então Atualizar data texto padrão
                            _repository.AtualizarTextoTabela(_ProcessoAtual);
                        }
                        else
                        {
                            _repository.AtualizarInserirTextoTabela(processo);
                            _repository.AtualizarInserirTextoTabela(processo);
                        }
                    }
                    else
                    {
                        if (_ProcessoAtual.Dt_Ult_Acao >= DataUltimaLinha
                   && _ProcessoAtual.Andamento != null)
                        {
                            //sem registro novo então Atualizar data texto padrão
                            _repository.AtualizarInserirTextoTabela(processo);
                        }
                        else
                        {
                            _repository.InserirTextoTabelaFila(processo);
                            _repository.AtualizarInserirTextoTabela(processo);
                        }
                    }
                    _repository.InserirTextoTabelaAndamento(processo);

                    driver.Quit();
                }
                return Processos;
            }
            catch (Exception ex)
            {
                Processos.First().Log_Erro = ex.Message;
                driver.Quit();

                //_repository.GravarLogErro(Processos.First());
                return null;
            }
        }
        #region Auxiliares
        //Auxiliares
        //static string GetChromeDriverVersion(string chromeDriverPath)
        //{
        //    if (File.Exists(chromeDriverPath))
        //    {
        //        FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(chromeDriverPath);
        //        return fileVersionInfo.ProductVersion;
        //    }
        //    return null;
        //}



        //static string? GetChromeVersion()
        //{
        //    using (IWebDriver driver = new ChromeDriver("C:\\Users\\FlávioSilvaVanquishC\\source\\repos\\AutomacaoAndamentoProcessos\\AutomacaoAndamentoProcessos\\bin\\Debug\\net6.0-windows"))
        //    {
        //        return ((IJavaScriptExecutor)driver).ExecuteScript("return chrome.app.getDetails().version").ToString();
        //    }
        //}

        //static void DownloadChromeDriver(string version)
        //{
        //    string downloadUrl = $"https://chromedriver.storage.googleapis.com/{version}/chromedriver_win32.zip";
        //    using (WebClient webClient = new())
        //    {
        //        webClient.DownloadFile(downloadUrl, "chromedriver.zip");
        //    }

        //    // Extrair o arquivo ZIP e substituir o ChromeDriver antigo
        //    // Certifique-se de incluir a lógica de extração e substituição aqui.
        //}

        //static string GetChromeDriverPath()
        //{
        //    string? currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        //    return Path.Combine(currentDirectory, "chromedriver.exe");
        //}
        public IWebDriver IniciarChrome(Solicitacao Solicitacoes)
        {
            //string chromeDriverPath = GetChromeDriverPath();
            //string currentVersion = GetChromeDriverVersion(chromeDriverPath);
            //string? chromeVersion = GetChromeVersion();




            //if (!currentVersion.StartsWith(chromeVersion))
            //{
            //    Console.WriteLine("As versões do ChromeDriver e do Chrome não correspondem. Atualizando o ChromeDriver...");
            //    DownloadChromeDriver(chromeVersion);
            //}
            //else
            //{
            //    Console.WriteLine("As versões do ChromeDriver e do Chrome correspondem.");
            //}

            try
            {
                options.AddArgument("--disable-notifications");
                options.AddArgument("--disable-default-apps");
                options.AddArgument("--disable-dev-shm-usage");
                options.AddArgument("--disable-web-security");
                options.AddArgument("--disable-site-isolation-trials");
                options.AddArgument("--disable-logging");
                options.AddArgument("--disable-bundled-ppapi-flash");
                options.AddArgument("--disable-gpu-compositing");
                options.AddArgument("--disable-gpu-shader-disk-cache");
                options.AddArgument("--disable-blink-features=AutomationControlled");
                options.AddArgument("--window-size=600,600");
                options.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36");
                options.AddArgument("--remote-debugging-port=9222");
                options.PageLoadStrategy = (PageLoadStrategy.None);
                options.AddAdditionalChromeOption("useAutomationExtension", false);
                options.AddUserProfilePreference("disable-popup-blocking", true);
                proxy.IsAutoDetect = false;
                options.Proxy = proxy;
                //driverService.HideCommandPromptWindow = false;
                //driver = new ChromeDriver(@"C:\Users\FlávioSilvaVanquishC\Downloads", options);
                driver = new ChromeDriver(@"G:\AutomacaoAndamento\chromeDrive", options);
                driver.Manage().Window.Minimize();
                return driver;
            }
            catch (Exception ex)
            {
                //EnviarEmail(ex.ToString());
                _repository.GravarLogErroChrome(ex.ToString());
                Solicitacoes.Log_Erro = ex.ToString();
                _repository.GravarLogErro(Solicitacoes);
                throw;
            }
        }

        public static IWebElement? EsperarElemento(IWebDriver drive, string by, string element)
        {
            //Metodo responsavel por buscar elementos no site, caso não encontrar tentar 3x e tratar erro.
            IWebElement? Element = null;
            for (int NTentativas = 0; NTentativas < 3; NTentativas++)
            {
                try
                {
                    Thread.Sleep(100);
                    if (by.Equals("Id"))
                        Element = drive.FindElement(By.Id(element));
                    if (by.Equals("XPath"))
                        Element = drive.FindElement(By.XPath(element));
                    if (by.Equals("ClassName"))
                        Element = drive.FindElement(By.ClassName(element));
                    if (by.Equals("LinkText"))
                        Element = drive.FindElement(By.LinkText(element));
                    if (by.Equals("CssSelector"))
                        Element = drive.FindElement(By.CssSelector(element));
                    if (by.Equals("Name"))
                        Element = drive.FindElement(By.Name(element));
                    if (by.Equals("PartialLinkText"))
                        Element = drive.FindElement(By.PartialLinkText(element));

                    return Element;
                }
                catch (Exception)
                {
                    Thread.Sleep(500);
                }
            }
            return null;
        }

        #endregion
    }
}
