namespace AutomacaoAndamentoProcessos
{
    partial class Automator
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Automator));
            Btn_Iniciar = new Button();
            Btn_Parar = new Button();
            progressBar1 = new ProgressBar();
            richTextBox1 = new RichTextBox();
            SuspendLayout();
            // 
            // Btn_Iniciar
            // 
            Btn_Iniciar.Location = new Point(19, 68);
            Btn_Iniciar.Name = "Btn_Iniciar";
            Btn_Iniciar.Size = new Size(112, 34);
            Btn_Iniciar.TabIndex = 0;
            Btn_Iniciar.Text = "Iniciar";
            Btn_Iniciar.UseVisualStyleBackColor = true;
            Btn_Iniciar.Click += Button1_Click;
            // 
            // Btn_Parar
            // 
            Btn_Parar.Location = new Point(19, 108);
            Btn_Parar.Name = "Btn_Parar";
            Btn_Parar.Size = new Size(112, 34);
            Btn_Parar.TabIndex = 1;
            Btn_Parar.Text = "Parar";
            Btn_Parar.UseVisualStyleBackColor = true;
            Btn_Parar.Click += Button2_Click;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(19, 383);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(573, 34);
            progressBar1.TabIndex = 4;
            progressBar1.Click += ProgressBar1_Click;
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(12, 205);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(643, 144);
            richTextBox1.TabIndex = 5;
            richTextBox1.Text = "";
            richTextBox1.TextChanged += RichTextBox1_TextChanged;
            // 
            // Automator
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(667, 446);
            Controls.Add(richTextBox1);
            Controls.Add(progressBar1);
            Controls.Add(Btn_Parar);
            Controls.Add(Btn_Iniciar);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Automator";
            Text = "Automação Andamento Processos";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button Btn_Iniciar;
        private Button Btn_Parar;
        public static ProgressBar progressBar1;
        public static RichTextBox richTextBox1;
    }
}