using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using AutomacaoAndamentoProcessos.Business;
using AutomacaoAndamentoProcessos.Models;
using Windows.UI.Xaml;

namespace AutomacaoAndamentoProcessos
{
    public partial class Automator : Form
    {
        readonly Business.Business business = new();
        public Automator()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //caso a Automação seja um serviço apenas descomentar e criar rotina no windows para inciar o executavel
            business.Start();
            Business.Business.Stop();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            business.Start();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Business.Business.Stop();
        }
        private static void ProgressBar1_Click(object sender, EventArgs e)
        {
            progressBar1.PerformStep();
        }

        private void RichTextBox1_TextChanged(object sender, EventArgs e)
        {
            Clipboard.GetText(TextDataFormat.Text).ToString();

        }
    }
}