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
using OpenQA.Selenium.Remote;
using System.Security.Policy;
using OpenQA.Selenium.Interactions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using CheckData;
using System.Drawing.Imaging;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using OpenQA.Selenium.Support.UI;
using System.Runtime.InteropServices;
using System.Collections;
using OpenQA.Selenium.DevTools;

namespace AutomacaoAndamentoProcessos
{
    public class Service
    {
        #region Inicializadores
        List<Solicitacao> _solicitacao = new();
        readonly Repository.Repository _repository = new();
        readonly List<string> TextosAdcionar = new();
        Business.Business _business = new Business.Business();

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
        string? errro = null;

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
            string destinatario = " ";
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

        public void NavegacaoPJE(List<Solicitacao> Processos, IWebDriver Driver)
        {
            //IWebElement? Indisponivel;
            try
            {
                ProcessoAtual _ProcessoAtual = new();

                foreach (Solicitacao processo in Processos)
                {
                    errro = processo.NumeroProcesso;
                    DateTime? DataUltAcaoProcesso = null;
                    _ProcessoAtual = _repository.RetornarDataUltmoRegistro(processo.Pi);
                    DataUltAcaoProcesso = _repository.RetornarDataUltmaAcao(processo.NumeroProcesso);
                    if (DataUltAcaoProcesso == null)
                    {
                        if (_ProcessoAtual.Dt_Ult_Acao == null)
                            DataUltimaLinha = DateTime.Now.AddYears(-100);
                        else
                            DataUltimaLinha = (DateTime)_ProcessoAtual.Dt_Ult_Acao;
                    }
                    else
                        DataUltimaLinha = (DateTime)DataUltAcaoProcesso;

                    processo.Andamento.Clear();
                    for (int i = 1; i < 3; i++)
                    {
                        Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                        if (i == 1)
                            Driver.Url = "https://pje1g.trf3.jus.br/pje/ConsultaPublica/listView.seam";
                        else
                            Driver.Url = "https://pje2g.trf3.jus.br/pje/ConsultaPublica/listView.seam";
                        Thread.Sleep(1200);


                        Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                        BtnNumeroProcesso = Driver.FindElement(By.XPath("/html/body/div[5]/div/div/div/div[2]/form/div[1]/div/div/div/div/div[1]/div/div[2]/input"));
                        if (BtnNumeroProcesso.Displayed)
                        {

                            BtnNumeroProcesso.Click();
                            BtnNumeroProcesso.Clear();
                            Driver.FindElement(By.XPath("/html/body/div[5]/div/div/div/div[2]/form/div[1]/div/div/div/div/div[1]/div/div[2]/input")).SendKeys(processo.NumeroProcesso);
                            BtnPesquisar = EsperarElemento(driver, "XPath", "/html/body/div[5]/div/div/div/div[2]/form/div[1]/div/div/div/div/div[8]/div/input");
                            if (BtnPesquisar.Displayed)
                                BtnPesquisar.Click();

                            Thread.Sleep(200);
                            var ProcessoNEncontrado = EsperarElemento(driver, "XPath", "/html/body/div[5]/div/div/div/div[2]/form/div[2]/div/dl/dt/span");
                            if (ProcessoNEncontrado != null)
                            {
                                //Processo não encontrado, retira da fila e pula para o proximo
                                if (i == 2)
                                    _repository.RetirarTabelaFilaDoiss(processo);
                                else
                                    _repository.RetirarTabelaFilaPrimeira(processo);
                                continue;
                            }

                            LinkDetalhesProcesso = Driver.FindElement(By.XPath("/html/body/div[5]/div/div/div/div[2]/form/div[2]/div/table/tbody/tr/td[1]/a/i"));
                            if (LinkDetalhesProcesso.Displayed)
                            {
                                LinkDetalhesProcesso.Click();
                                Thread.Sleep(500);
                                Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                                Thread.Sleep(1000);
                                Driver.Manage().Window.Maximize();
                            }
                            Thread.Sleep(1000);
                            GridProcesso = null;
                            TextoTratado = null;
                            TextoTratadoInserir = null;

                            GridProcesso = Driver.FindElement(By.Id("j_id131:processoEvento"));
                            TextoTratado = GridProcesso.Text.ToString().Split("\r\n");
                            TextoTratadoInserir = TextoTratado[2].TrimStart().TrimEnd();
                            foreach (var item in TextoTratado)
                            {
                                if (item.Length > 10)
                                    DataLinhaAtual = Convert.ToDateTime(item[..10]);
                                if (DataUltimaLinha <= DataLinhaAtual && DataLinhaAtual <= DateTime.Today.AddDays(-1) && (!TextoTratadoInserir.First().Equals(item)))
                                    processo.Andamento.Add(item.ToString().TrimStart().TrimEnd());
                            }
                            //Logica de inverter ordem da lista de andamentos
                            List<string> ListAndamento = new List<string>();
                            var n = processo.Andamento.Count();
                            for (int j = 0; j < n;)
                            {
                                ListAndamento.Add(processo.Andamento[n - 1]);
                                n = n - 1;
                            }
                            if (ListAndamento.Count > 0)
                            {
                                processo.Andamento.Clear();
                                processo.Andamento = ListAndamento;
                            }
                            n = 0;

                            //Chamada Sql para Inserir registro no Banco
                            processo.Status = (int)Status.Processado;
                            _ProcessoAtual = _repository.RetornarDataUltmoRegistro(processo.Pi);
                            if (_ProcessoAtual.Andamento.Contains("PROCESSO CONSULTADO PELA AUTOMAÇÃO"))
                            {
                                if (processo.Andamento.Count <= 0)
                                {
                                    //sem registro novo então Atualizar data texto padrão
                                    _repository.AtualizarTextoTabela(_ProcessoAtual);
                                }
                                else
                                {
                                    _repository.InserirTextoTabelaFila(processo);
                                    _repository.AtualizarTextoTabela(_ProcessoAtual);

                                }
                            }
                            else
                            {
                                if (processo.Andamento.Count <= 0)
                                {
                                    var idAndamento = _repository.RetornarIdUltmoRegistro(processo.Pi);
                                    if (idAndamento.Id != null)
                                    {
                                        _repository.AtualizarTextoTabela(idAndamento);
                                    }
                                    else
                                    {
                                        //sem registro novo então Atualizar data texto padrão
                                        _repository.AtualizarInserirTextoTabela(processo);
                                    }
                                }
                                else
                                {
                                    var idAndamento = _repository.RetornarIdUltmoRegistro(processo.Pi);
                                    if (idAndamento.Id != null)
                                    {
                                        _repository.InserirTextoTabelaFila(processo);
                                        _repository.AtualizarTextoTabela(idAndamento);
                                    }
                                    else
                                    {
                                        _repository.InserirTextoTabelaFila(processo);
                                        _repository.AtualizarInserirTextoTabela(processo);
                                    }
                                }
                            }
                            _repository.InserirTextoTabelaAndamento(processo);
                            Driver.Close();
                        }
                    }
                }
                Driver.Quit();
            }
            catch (Exception ex)
            {
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                var ticks = DateTime.Now.Ticks;
                screenshot.SaveAsFile(@$"F:\AutomacaoAndamento\ScreenShot\{ticks}.jpg");
                Processos.First().Log_Erro = ex.Message;
                driver.Quit();
                _repository.GravarLogErro(Processos.First(), errro);
            }
        }



        public List<Solicitacao> NavegacaoTJSP(List<Solicitacao> Processos, IWebDriver driver)
        {
            try
            {
                Thread.Sleep(3000);
                driver.Url = "https://esaj.tjsp.jus.br/sajcas/login?service=https%3A%2F%2Fesaj.tjsp.jus.br%2Fesaj%2Fj_spring_cas_security_check";
                Thread.Sleep(3000);
                var VerifAbort = EsperarElemento(driver, "XPath", "/html/body/table[4]/tbody/tr/td/table[2]/tbody/tr[1]/td[1]/div/table/tbody/tr[1]/td/div[2]/div[2]/form/div[1]/table/tbody/tr[1]/td[2]/input");
                if (VerifAbort == null)
                {
                    Thread.Sleep(9000);
                    driver.Url = "https://esaj.tjsp.jus.br/sajcas/login?service=https%3A%2F%2Fesaj.tjsp.jus.br%2Fesaj%2Fj_spring_cas_security_check";
                    Thread.Sleep(1000);
                    VerifAbort = EsperarElemento(driver, "XPath", "/html/body/table[4]/tbody/tr/td/table[2]/tbody/tr[1]/td[1]/div/table/tbody/tr[1]/td/div[2]/div[2]/form/div[1]/table/tbody/tr[1]/td[2]/input");
                    if (VerifAbort == null)
                    {
                        _business.Restart();
                    }
                }
                //Login no site 
                driver.FindElement(By.XPath("/html/body/table[4]/tbody/tr/td/table[2]/tbody/tr[1]/td[1]/div/table/tbody/tr[1]/td/div[2]/div[2]/form/div[1]/table/tbody/tr[1]/td[2]/input")).SendKeys("472.061.598-85");
                driver.FindElement(By.XPath("/html/body/table[4]/tbody/tr/td/table[2]/tbody/tr[1]/td[1]/div/table/tbody/tr[1]/td/div[2]/div[2]/form/div[1]/table/tbody/tr[2]/td[2]/input")).SendKeys("17012015");
                var BtnSubmit = driver.FindElement(By.XPath("/html/body/table[4]/tbody/tr/td/table[2]/tbody/tr[1]/td[1]/div/table/tbody/tr[1]/td/div[2]/div[2]/form/div[1]/table/tbody/tr[4]/td[2]/input[4]"));
                BtnSubmit.Click();
                foreach (Solicitacao processo in Processos)
                {

                    List<string> TextosAdcionar = new();
                    IWebElement? BtnOutros = null;
                    IWebElement? BtnConsultar = null;
                    bool SegundaInstancia = false;
                    List<string> listNamelink = new List<string>();
                    IWebElement? ProcNaoExiste = null;
                    IWebElement? tableElement = null;
                    Actions actions = new Actions(driver);
                    DateTime DataLinhaAtualVerifica1;
                    string? dataVerifica1 = null;
                    DateTime DataLinhaAtualVerifica;
                    DateTime DataLinhaAdd;
                    string? dataVerifica = null;
                    List<string> currentURL = new();
                    ProcessoAtual _ProcessoAtual = new();
                    string? Juntarlinhas = null;
                    processo.Operador = "AUTOMACAO";
                    Thread.Sleep(900);
                    _ProcessoAtual = _repository.RetornarDataUltmoRegistro(processo.Pi);
                    if (_ProcessoAtual.Dt_Ult_Acao == null)
                        DataUltimaLinha = DateTime.Now.AddYears(-100);
                    else
                        DataUltimaLinha = (DateTime)_ProcessoAtual.Dt_Ult_Acao;
                    var TodosProcessos = _repository.RetornaSolicitacoesPi(processo);

                    foreach (var Proc in TodosProcessos)
                    {
                        //For responsavel por processar 2x cada solicitação, primeira e segunda instancia
                        for (int i = 0; i < 2; i++)
                        {
                            SegundaInstancia = false;
                            if (i > 0)
                            {
                                SegundaInstancia = true;
                                driver.Url = "https://esaj.tjsp.jus.br/cposg/open.do";
                            }
                            else
                                driver.Url = "https://esaj.tjsp.jus.br/cpopg/open.do";
                            Thread.Sleep(1500);
                            BtnOutros = EsperarElemento(driver, "XPath", ("/html/body/div[2]/form/section/div[2]/div/div[1]/div[1]/div/fieldset/label[2]"));
                            if (BtnOutros != null)
                                BtnOutros.Click();
                            Thread.Sleep(400);
                            BtnNumeroProcesso = driver.FindElement(By.XPath("/html/body/div[2]/form/section/div[2]/div/div[1]/div[1]/span[2]/input"));
                            if (BtnNumeroProcesso.Displayed)
                            {
                                Thread.Sleep(600);
                                BtnNumeroProcesso.Click();
                                driver.FindElement(By.XPath("/html/body/div[2]/form/section/div[2]/div/div[1]/div[1]/span[2]/input")).SendKeys(Proc.NumeroProcesso);
                                BtnConsultar = driver.FindElement(By.XPath("/html/body/div[2]/form/section/div[4]/div/input"));
                                if (BtnConsultar.Displayed)
                                {
                                    BtnConsultar.Click();
                                    Thread.Sleep(500);
                                    ProcNaoExiste = EsperarElemento(driver, "Id", "mensagemRetorno");
                                    if (ProcNaoExiste != null)
                                    {
                                        //Processo não encontrado, retira da fila e pula para o proximo
                                        if (SegundaInstancia)
                                            _repository.RetirarTabelaFila(Proc);
                                        else
                                            _repository.RetirarTabelaFilaPrimeira(Proc);
                                        continue;
                                    }
                                    var Selecionar = EsperarElemento(driver, "XPath", "/html/body/div[1]/div/div[2]/div/article/section/div[1]/div[1]/input");
                                    if (Selecionar != null)
                                    {
                                        Selecionar.Click();
                                        var SelecionarClick = EsperarElemento(driver, "XPath", "/html/body/div[1]/div/div[3]/input[2]");
                                        if (SelecionarClick != null)
                                            SelecionarClick.Click();
                                    }
                                    var ComplementoAndamento = EsperarElemento(driver, "XPath", "/html/body/div[1]/div/div[3]/input[2]");
                                    if (ComplementoAndamento != null)
                                        ComplementoAndamento.Click();
                                    IWebElement? Expandir = null;
                                    currentURL.Clear();
                                    currentURL.Add(driver.Url);
                                    errro = Proc.NumeroProcesso;

                                    var retitarFila = EsperarElemento(driver, "XPath", "/html/body/div[2]/div[1]/div[1]/span");
                                    if (retitarFila != null)
                                    {
                                        _repository.RetirarFilaDadosDesatualizados(Proc);
                                        continue;
                                    }

                                    //Descer pagina e subir ate os links
                                    {
                                        actions.SendKeys(OpenQA.Selenium.Keys.End).Build().Perform();
                                        actions.SendKeys(OpenQA.Selenium.Keys.Up).Build().Perform();
                                        actions.SendKeys(OpenQA.Selenium.Keys.Up).Build().Perform();
                                        actions.SendKeys(OpenQA.Selenium.Keys.Up).Build().Perform();
                                        actions.SendKeys(OpenQA.Selenium.Keys.Up).Build().Perform();
                                        actions.SendKeys(OpenQA.Selenium.Keys.Up).Build().Perform();
                                    }
                                    listNamelink.Clear();
                                    var rows = EsperarElemento(driver, "Id", "processoSemIncidentes");
                                    if (rows == null)
                                    {
                                        if (SegundaInstancia)
                                            tableElement = driver.FindElement(By.XPath("/ html / body / div[2] / table[6]"));
                                        else
                                            tableElement = driver.FindElement(By.XPath("/html/body/div[2]/table[4]"));

                                        IList<IWebElement> tableRow = tableElement.FindElements(By.TagName("tr"));
                                        IList<IWebElement> rowTD;
                                        IList<IWebElement> rowTDi;
                                        string? Compar = null;
                                        foreach (IWebElement row in tableRow)
                                        {
                                            rowTD = row.FindElements(By.TagName("td"));
                                            rowTDi = row.FindElements(By.TagName("th"));

                                            foreach (var tite in rowTDi)
                                            {
                                                Compar = tite.Text;
                                            }
                                            if (rowTDi.Count == 0)
                                                Compar = "T";
                                            if (rowTD.Count > 0)
                                            {
                                                if (rowTD[0].Text.Equals("Não há Audiências futuras vinculadas a este processo."))
                                                {
                                                    tableElement = driver.FindElement(By.XPath("/html/body/div[2]/table[4]"));
                                                    tableRow = tableElement.FindElements(By.TagName("tr"));
                                                    foreach (IWebElement row1 in tableRow)
                                                    {
                                                        rowTD = row1.FindElements(By.TagName("td"));
                                                        if (rowTD.Count > 0)
                                                        {
                                                            foreach (var rowText in rowTD)
                                                            {
                                                                var texto = rowText.Text;
                                                                if (!texto.Equals(""))
                                                                {
                                                                    string dataVerificaff;
                                                                    if (texto.Length > 9)
                                                                        dataVerificaff = texto[..10];
                                                                    else
                                                                        dataVerificaff = "Sem Data";
                                                                    VerificaNumeros = DateTime.TryParse(dataVerificaff, out DataLinhaAtualVerifica);
                                                                    if (!VerificaNumeros && !texto.Equals("Não há incidentes, ações incidentais, recursos ou execuções de sentenças vinculados a este processo."))
                                                                    {
                                                                        listNamelink.Add(texto);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    break;
                                                }
                                            }
                                            if (rowTD.Count > 0)
                                            {
                                                if (rowTD[0].Text.Equals("Não há petições diversas vinculadas a este processo."))
                                                {
                                                    tableElement = driver.FindElement(By.XPath("/html/body/div[2]/table[5]"));
                                                    tableRow = tableElement.FindElements(By.TagName("tr"));
                                                    foreach (IWebElement row1 in tableRow)
                                                    {
                                                        rowTD = row1.FindElements(By.TagName("td"));
                                                        if (rowTD.Count > 0)
                                                        {
                                                            foreach (var rowText in rowTD)
                                                            {
                                                                var texto = rowText.Text;
                                                                if (!texto.Equals(""))
                                                                {
                                                                    string dataVerificaff;
                                                                    if (texto.Length > 9)
                                                                        dataVerificaff = texto[..10];
                                                                    else
                                                                        dataVerificaff = "Sem Data";
                                                                    VerificaNumeros = DateTime.TryParse(dataVerificaff, out DataLinhaAtualVerifica);
                                                                    if (!VerificaNumeros && !texto.Equals("Não há incidentes, ações incidentais, recursos ou execuções de sentenças vinculados a este processo."))
                                                                    {
                                                                        listNamelink.Add(texto);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    break;
                                                }
                                            }

                                            if (Compar.Equals("Tipo"))
                                            {
                                                tableElement = driver.FindElement(By.XPath("/html/body/div[2]/table[5]"));
                                                tableRow = tableElement.FindElements(By.TagName("tr"));
                                                foreach (IWebElement row1 in tableRow)
                                                {
                                                    rowTD = row1.FindElements(By.TagName("td"));
                                                    if (rowTD.Count > 0)
                                                    {
                                                        foreach (var rowText in rowTD)
                                                        {
                                                            var texto = rowText.Text;
                                                            if (!texto.Equals(""))
                                                            {
                                                                string dataVerificaff;
                                                                if (texto.Length > 9)
                                                                    dataVerificaff = texto[..10];
                                                                else
                                                                    dataVerificaff = "Sem Data";
                                                                VerificaNumeros = DateTime.TryParse(dataVerificaff, out DataLinhaAtualVerifica);
                                                                if (!VerificaNumeros && !texto.Equals("Não há incidentes, ações incidentais, recursos ou execuções de sentenças vinculados a este processo."))
                                                                {
                                                                    listNamelink.Add(texto);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                break;
                                            }

                                            if (rowTD.Count > 0)
                                            {
                                                foreach (var rowText in rowTD)
                                                {
                                                    var texto = rowText.Text;
                                                    if (!texto.Equals(""))
                                                    {
                                                        string dataVerificaff;
                                                        if (texto.Length > 9)
                                                            dataVerificaff = texto[..10];
                                                        else
                                                            dataVerificaff = "Sem Data";
                                                        VerificaNumeros = DateTime.TryParse(dataVerificaff, out DataLinhaAtualVerifica);
                                                        if (!VerificaNumeros && !texto.Equals("Não há incidentes, ações incidentais, recursos ou execuções de sentenças vinculados a este processo."))
                                                        {
                                                            listNamelink.Add(texto);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    foreach (var itemName in listNamelink)
                                    {
                                        try
                                        {
                                            driver.FindElements(By.LinkText(itemName)).FirstOrDefault().Click();
                                            currentURL.Add(driver.Url);
                                            driver.Navigate().Back();
                                            Thread.Sleep(300);
                                        }
                                        catch (Exception) { }
                                    }
                                    //Processamento de todas as paginas atreladas ao um processo
                                    foreach (var atual in currentURL)
                                    {

                                        driver.Url = atual.ToString();
                                        Expandir = EsperarElemento(driver, "Id", "linkmovimentacoes");
                                        if (Expandir != null)
                                        {
                                            var element = driver.FindElement(By.Id("divLinksTituloBlocoMovimentacoes"));
                                            actions.MoveToElement(element);
                                            actions.Perform();
                                            actions.SendKeys(OpenQA.Selenium.Keys.Down).Build().Perform();
                                            actions.SendKeys(OpenQA.Selenium.Keys.Down).Build().Perform();
                                            actions.SendKeys(OpenQA.Selenium.Keys.Down).Build().Perform();
                                            Thread.Sleep(500);
                                            driver.FindElements(By.Id("linkmovimentacoes")).FirstOrDefault().Click();
                                        }
                                        else
                                        {
                                            Expandir = EsperarElemento(driver, "Id", "tabelaUltimasMovimentacoes");
                                            if (Expandir != null)
                                            {
                                                var element = driver.FindElement(By.Id("tabelaUltimasMovimentacoes"));
                                                actions.MoveToElement(element);
                                                actions.Perform();
                                                actions.SendKeys(OpenQA.Selenium.Keys.Down).Build().Perform();
                                                actions.SendKeys(OpenQA.Selenium.Keys.Down).Build().Perform();
                                                actions.SendKeys(OpenQA.Selenium.Keys.Down).Build().Perform();
                                                Thread.Sleep(500);
                                                driver.FindElements(By.Id("tabelaUltimasMovimentacoes")).FirstOrDefault().Click();
                                            }
                                        }

                                        TextoTratado = null;
                                        TextoUltimaFase = null;
                                        //Raspa Textos
                                        TextoUltimaFase = EsperarElemento(driver, "XPath", "/html/body/div[2]/table[2]/tbody[2]");
                                        if (TextoUltimaFase != null)
                                            TextoTratado = TextoUltimaFase.Text.ToString().Split("\r\n");
                                        else
                                        {
                                            TextoUltimaFase = driver.FindElement(By.XPath("/html/body/div[2]/table[3]"));
                                            TextoTratado = TextoUltimaFase.Text.ToString().Split("\r\n");
                                        }
                                        if (SegundaInstancia)
                                        {
                                            TextoUltimaFase = driver.FindElement(By.XPath("/ html / body / div[2] / table[5]"));
                                            TextoTratado = TextoUltimaFase.Text.ToString().Split("\r\n");
                                            if (TextoTratado.Count() <= 1)
                                            {
                                                TextoUltimaFase = driver.FindElement(By.Id("tabelaTodasMovimentacoes"));
                                                TextoTratado = TextoUltimaFase.Text.ToString().Split("\r\n");
                                            }
                                            if (TextoTratado.Count() <= 1)
                                            {
                                                TextoUltimaFase = driver.FindElement(By.Id("tabelaUltimasMovimentacoes"));
                                                TextoTratado = TextoUltimaFase.Text.ToString().Split("\r\n");
                                            }

                                        }
                                        if (TextoTratado.Length <= 4 || TextoTratado[0].Contains("Autor  Justiça Pública"))
                                        {
                                            if (TextoTratado[0].Length < 9)
                                            {
                                                //TextoUltimaFase = driver.FindElement(By.XPath("/html/body/div[2]/table[3]"));
                                                TextoUltimaFase = driver.FindElement(By.Id("tabelaTodasMovimentacoes"));
                                                TextoTratado = TextoUltimaFase.Text.ToString().Split("\r\n");
                                            }
                                            if (TextoTratado.Length >= 10)
                                                dataVerifica1 = TextoTratado[0][..10];
                                            else
                                                dataVerifica1 = TextoTratado[0].Substring(0, TextoTratado[0].Length);
                                            VerificaNumeros = DateTime.TryParse(dataVerifica1, out DataLinhaAtualVerifica1);
                                            if (!VerificaNumeros)
                                            {
                                                TextoUltimaFase = driver.FindElement(By.XPath("/html/body/div[2]/table[2]"));
                                                TextoTratado = TextoUltimaFase.Text.ToString().Split("\r\n");
                                            }
                                        }
                                        if (TextoTratado.First().Contains("Data   Movimento"))
                                            TextoTratado = TextoTratado.Where(o => o != TextoTratado[0]).ToArray();

                                        if (TextoTratado.First().Equals(""))
                                            TextoTratado = TextoTratado.Where(o => o != TextoTratado[0]).ToArray();

                                        //Juntar Texto
                                        c = 0;
                                        TextosAdcionar.Clear();
                                        foreach (string itemJuntar in TextoTratado)
                                        {
                                            var conf = c;
                                            Juntarlinhas = "";
                                            if (itemJuntar.Length > 9)
                                                dataVerifica = itemJuntar[..10];
                                            else
                                                dataVerifica = "Sem Data";
                                            VerificaNumeros = DateTime.TryParse(dataVerifica, out DataLinhaAtualVerifica);
                                            if (!VerificaNumeros)
                                            {
                                                Juntarlinhas = TextoTratado[conf - 1] + " " + itemJuntar;
                                                TextosAdcionar.Add(Juntarlinhas);
                                            }
                                            else
                                            {
                                                int contadortexto = TextoTratado.Count() - 1;
                                                int indexum = conf + 1;
                                                if (c < contadortexto)
                                                {
                                                    string? verifData = null;
                                                    if (TextoTratado[indexum].Length > 9)
                                                        verifData = TextoTratado[indexum][..10];
                                                    else
                                                        verifData = "Texto sem Data " + TextoTratado[indexum];
                                                    VerificaNumeros = DateTime.TryParse(verifData, out DataLinhaAdd);
                                                    if (VerificaNumeros)
                                                    {
                                                        TextosAdcionar.Add(itemJuntar);
                                                    }
                                                }
                                                else
                                                {
                                                    TextosAdcionar.Add(itemJuntar);
                                                }
                                            }
                                            c = c + 1;
                                        }
                                        IMain check = new Main();
                                        c = 0;
                                        Proc.Andamento.Clear();
                                        DateTime DataUltimaLinhaCompara;
                                        DateTime? DataUltAcaoProcesso = null;
                                        DataUltAcaoProcesso = _repository.RetornarDataUltmaAcao(processo.NumeroProcesso);
                                        if (DataUltAcaoProcesso == null)
                                            DataUltimaLinhaCompara = DataUltimaLinha;
                                        else
                                            DataUltimaLinhaCompara = (DateTime)DataUltAcaoProcesso;
                                        //tratar Data para Verificações 
                                        foreach (string item in TextosAdcionar)
                                        {
                                            string AtualData = item[..10];
                                            bool Valid = check.IsDate(AtualData);
                                            if (Valid)
                                            {
                                                DataLinhaAtual = Convert.ToDateTime(item[..10]);
                                                if (DataUltimaLinhaCompara <= DataLinhaAtual && DataLinhaAtual <= DateTime.Today.AddDays(-1))
                                                    Proc.Andamento.Add(item);
                                            }
                                        }
                                        //Logica de inverter ordem da lista de andamentos
                                        List<string> ListAndamento = new List<string>();
                                        var n = Proc.Andamento.Count();
                                        for (int j = 0; j < n;)
                                        {
                                            ListAndamento.Add(Proc.Andamento[n - 1]);
                                            n = n - 1;
                                        }
                                        if (ListAndamento.Count > 0)
                                        {
                                            Proc.Andamento.Clear();
                                            Proc.Andamento = ListAndamento;
                                        }
                                        n = 0;
                                    }
                                    //Chamada Sql para Inserir registro no Banco
                                    Proc.Status = (int)Status.Processado;
                                    if (_ProcessoAtual.Andamento.Contains("PROCESSO CONSULTADO PELA AUTOMAÇÃO"))
                                    {
                                        if (Proc.Andamento.Count <= 0)
                                        {
                                            //sem registro novo então Atualizar data texto padrão
                                            _repository.AtualizarTextoTabela(_ProcessoAtual);
                                        }
                                        else
                                        {
                                            _repository.InserirTextoTabelaFila(Proc);
                                            _repository.AtualizarTextoTabela(_ProcessoAtual);
                                        }
                                    }
                                    else
                                    {
                                        if (Proc.Andamento.Count <= 0)
                                        {
                                            var idAndamento = _repository.RetornarIdUltmoRegistro(processo.Pi);
                                            if (idAndamento.Id != null)
                                            {
                                                _repository.AtualizarTextoTabela(idAndamento);
                                            }
                                            else
                                            {
                                                //sem registro novo então Atualizar data texto padrão
                                                _repository.AtualizarInserirTextoTabela(Proc);
                                            }
                                        }
                                        else
                                        {
                                            var idAndamento = _repository.RetornarIdUltmoRegistro(processo.Pi);
                                            if (idAndamento.Id != null)
                                            {
                                                _repository.InserirTextoTabelaFila(Proc);
                                                _repository.AtualizarTextoTabela(idAndamento);
                                            }
                                            else
                                            {
                                                _repository.InserirTextoTabelaFila(Proc);
                                                _repository.AtualizarInserirTextoTabela(Proc);
                                            }
                                        }
                                    }
                                    _repository.InserirTextoTabelaAndamento(Proc);
                                }
                            }
                        }
                    }
                }
                driver.Quit();
                return Processos;
            }
            catch (Exception ex)
            {
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                var ticks = DateTime.Now.Ticks;
                screenshot.SaveAsFile(@$"F:\AutomacaoAndamento\ScreenShot\{ticks}.jpg");
                Processos.First().Log_Erro = ex.Message;
                driver.Quit();
                _repository.GravarLogErro(Processos.First(), errro);
                return null;
            }
        }
        public List<Solicitacao> NavegacaoSTJ(List<Solicitacao> Processos, IWebDriver driver)
        {
            ProcessoAtual _ProcessoAtual = new();
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
                    errro = processo.NumeroProcesso;
                    driver.Url = $"https://processo.stj.jus.br/processo/pesquisa/?termo={processo.NumeroProcesso}&aplicacao=processos.ea&tipoPesquisa=tipoPesquisaGenerica&chkordem=DESC&chkMorto=MORTO";
                    Thread.Sleep(3000);
                    processo.Andamento.Clear();
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
                        _repository.RetirarTabelaFilaDois(processo);
                        continue;
                    }
                    var ProcessoNExiste2 = EsperarElemento(driver, "XPath", "/html/body/div[2]/div[6]/div/div/div[3]/div[2]/div/div/div[3]/div[2]/span[2]/div[2]/div");
                    if (ProcessoNExiste2 != null)
                    {
                        //Processo não encontrado, retira da fila e pula para o proximo
                        _repository.RetirarTabelaFilaDois(processo);
                        continue;
                    }
                    //var SistemaIndisponivel = EsperarElemento(driver, "XPath", "/html/body/div[2]/div[6]/div/div/div[3]/div[2]/div/div");
                    //if (SistemaIndisponivel != null)
                    //{
                    //    if (SistemaIndisponivel.Text.Equals("Sistema indisponível."))
                    //        continue;
                    //}

                    AbaFases = EsperarElemento(driver, "XPath", "/html/body/div[2]/div[6]/div/div/div[3]/div[2]/div/div/div[3]/div[2]/span[2]/div[4]/span[2]/a");
                    if (AbaFases != null)
                    {
                        AbaFases.Click();
                        Thread.Sleep(200);
                        DateTime? DataUltAcaoProcesso = null;
                        TextoUltimaFase = driver.FindElement(By.XPath("/html/body/div[2]/div[6]/div/div/div[3]/div[2]/div/div/div[3]/div[2]/span[2]/div[5]/div[2]/div[2]"));
                        TextoTratado = TextoUltimaFase.Text.ToString().Split("\r\n");
                        TextoTratadoInserir = TextoTratado.First().TrimStart().TrimEnd() + "\\";
                        _ProcessoAtual = _repository.RetornarDataUltmoRegistro(processo.Pi);
                        DataUltAcaoProcesso = _repository.RetornarDataUltmaAcao(processo.NumeroProcesso);
                        if (DataUltAcaoProcesso == null)
                        {
                            if (_ProcessoAtual.Dt_Ult_Acao == null)
                                DataUltimaLinha = DateTime.Now.AddYears(-100);
                            else
                                DataUltimaLinha = (DateTime)_ProcessoAtual.Dt_Ult_Acao;
                        }
                        else
                            DataUltimaLinha = (DateTime)DataUltAcaoProcesso;

                        foreach (string item in TextoTratado)
                        {
                            VerificaNumeros = DateTime.TryParse(item[..10], out DataLinhaAtual);
                            if (VerificaNumeros && DataLinhaAtual <= DateTime.Today.AddDays(-1) && DataUltimaLinha <= DataLinhaAtual && (!TextoTratadoInserir.Equals(item)))
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

                    //Logica de inverter ordem da lista de andamentos
                    List<string> ListAndamento = new List<string>();
                    var n = processo.Andamento.Count();
                    for (int j = 0; j < n;)
                    {
                        ListAndamento.Add(processo.Andamento[n - 1]);
                        n = n - 1;
                    }
                    if (ListAndamento.Count > 0)
                    {
                        processo.Andamento.Clear();
                        processo.Andamento = ListAndamento;
                    }
                    n = 0;

                    //Chamada Sql para Inserir registro no Banco
                    processo.Status = (int)Status.Processado;
                    _ProcessoAtual = _repository.RetornarDataUltmoRegistro(processo.Pi);
                    if (_ProcessoAtual.Andamento.Contains("PROCESSO CONSULTADO PELA AUTOMAÇÃO"))
                    {
                        if (processo.Andamento.Count <= 0)
                        {
                            //sem registro novo então Atualizar data texto padrão
                            _repository.AtualizarTextoTabela(_ProcessoAtual);
                        }
                        else
                        {
                            _repository.InserirTextoTabelaFila(processo);
                            _repository.AtualizarTextoTabela(_ProcessoAtual);

                        }
                    }
                    else
                    {
                        if (processo.Andamento.Count <= 0)
                        {
                            var idAndamento = _repository.RetornarIdUltmoRegistro(processo.Pi);
                            if (idAndamento.Id != null)
                            {
                                _repository.AtualizarTextoTabela(idAndamento);
                            }
                            else
                            {
                                //sem registro novo então Atualizar data texto padrão
                                _repository.AtualizarInserirTextoTabela(processo);
                            }
                        }
                        else
                        {
                            var idAndamento = _repository.RetornarIdUltmoRegistro(processo.Pi);
                            if (idAndamento.Id != null)
                            {
                                _repository.InserirTextoTabelaFila(processo);
                                _repository.AtualizarTextoTabela(idAndamento);
                            }
                            else
                            {
                                _repository.InserirTextoTabelaFila(processo);
                                _repository.AtualizarInserirTextoTabela(processo);
                            }
                        }
                    }
                    _repository.InserirTextoTabelaAndamento(processo);

                    driver.Quit();
                }
                driver.Quit();
                return Processos;
            }
            catch (Exception ex)
            {
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                var ticks = DateTime.Now.Ticks;
                screenshot.SaveAsFile(@$"F:\AutomacaoAndamento\ScreenShot\{ticks}.jpg");
                Processos.First().Log_Erro = ex.Message;
                driver.Quit();
                _repository.GravarLogErro(Processos.First(), errro);
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
        public void MatarProcessos()
        {
            //Mata o chrome driver caso ele esteja aberto em segundo plano
            Process[] chromeDriverProcesses = Process.GetProcessesByName("chromedriver");
            foreach (var chromeDriverProcess in chromeDriverProcesses)
            {
                chromeDriverProcess.Kill();
            }

            Process[] chrome = Process.GetProcessesByName("chrome");
            foreach (var chromeProcess in chrome)
            {
                chromeProcess.Kill();
            }
        }


        public IWebDriver IniciarChrome(Solicitacao Solicitacoes)
        {

            try
            {
                MatarProcessos();
                options.AddArgument("--disable-notifications");
                options.AddArgument("--disable-default-apps");
                options.AddArgument("--disable-dev-shm-usage");
                options.AddArgument("--disable-web-security");
                options.AddArgument("--disable-site-isolation-trials");
                options.AddArgument("--disable-logging");
                options.AddArgument("--log-level=3");
                options.AddArgument("--ignore-certificate-errors");
                options.AddArgument("--disable-bundled-ppapi-flash");
                options.AddArgument("--disable-gpu-compositing");
                options.AddArgument("--disable-gpu-shader-disk-cache");
                options.AddArgument("--disable-blink-features=AutomationControlled");
                options.AddArgument("--window-size=600,600");
                options.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36");
                //config responsavel por setar uma porta, sem essa config é possivel abrir varias instancias da automação 
                options.AddArgument("--remote-debugging-port=9222");
                //options.DebuggerAddress = "127.0.0.1:5555";
                options.PageLoadStrategy = (PageLoadStrategy.None);
                options.AddAdditionalChromeOption("useAutomationExtension", false);
                options.AddUserProfilePreference("disable-popup-blocking", true);
                options.AddArgument("--ignore-certificate-errors");
                options.AddArgument("no-sandbox");
                options.AddArgument("test-type");
                options.AddExcludedArguments(new List<string>() { "enable-automation" });
                proxy.IsAutoDetect = false;
                options.Proxy = proxy;
                var driverService = ChromeDriverService.CreateDefaultService(@"F:\AutomacaoAndamento\chromeDrive");
                //var driverService = ChromeDriverService.CreateDefaultService(@"C:\Users\FlávioSilvaVanquishC\Downloads");
                driverService.SuppressInitialDiagnosticInformation = true;
                //driver = new ChromeDriver(@"C:\Users\FlávioSilvaVanquishC\Downloads", options);
                driver = new ChromeDriver(driverService, options);
                driver.Manage().Window.Maximize();
                return driver;
            }
            catch (Exception ex)
            {
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                var ticks = DateTime.Now.Ticks;
                screenshot.SaveAsFile(@$"F:\AutomacaoAndamento\ScreenShot\{ticks}.jpg");
                //screenshot.SaveAsFile(@$"C:\Users\FlávioSilvaVanquishC\desktop\{ticks}.jpg");
                _repository.GravarLogErroChrome(ex.ToString());
                Solicitacoes.Log_Erro = ex.ToString();
                _repository.GravarLogErro(Solicitacoes);
                throw;
            }
        }

        public static IWebElement? EsperarElemento(IWebDriver? drive, string by, string element)
        {
            //Metodo responsavel por buscar elementos no site, caso não encontrar tentar 3x e tratar erro.
            IWebElement? Element = null;
            for (int NTentativas = 0; NTentativas < 3; NTentativas++)
            {
                try
                {
                    if (drive != null)
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
                    }
                    return Element;
                }
                catch (Exception)
                {
                    Thread.Sleep(200);
                }
            }
            return null;
        }

        #endregion
    }
}
