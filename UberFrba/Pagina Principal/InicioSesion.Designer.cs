namespace UberFrba.Pagina_Principal
{
    partial class InicioSesion
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelUsuario = new System.Windows.Forms.Label();
            this.labelPassword = new System.Windows.Forms.Label();
            this.textoUsuario = new System.Windows.Forms.TextBox();
            this.textoPassword = new System.Windows.Forms.TextBox();
            this.Ingresar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelUsuario
            // 
            this.labelUsuario.AutoSize = true;
            this.labelUsuario.Location = new System.Drawing.Point(208, 48);
            this.labelUsuario.Name = "labelUsuario";
            this.labelUsuario.Size = new System.Drawing.Size(43, 13);
            this.labelUsuario.TabIndex = 0;
            this.labelUsuario.Text = "Usuario";
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.Location = new System.Drawing.Point(208, 90);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(53, 13);
            this.labelPassword.TabIndex = 1;
            this.labelPassword.Text = "Password";
            // 
            // textoUsuario
            // 
            this.textoUsuario.Location = new System.Drawing.Point(285, 48);
            this.textoUsuario.Name = "textoUsuario";
            this.textoUsuario.Size = new System.Drawing.Size(100, 20);
            this.textoUsuario.TabIndex = 2;
            this.textoUsuario.TextChanged += new System.EventHandler(this.textoUsuario_TextChanged);
            // 
            // textoPassword
            // 
            this.textoPassword.Location = new System.Drawing.Point(285, 90);
            this.textoPassword.Name = "textoPassword";
            this.textoPassword.Size = new System.Drawing.Size(100, 20);
            this.textoPassword.TabIndex = 3;
            this.textoPassword.UseSystemPasswordChar = true;
            // 
            // Ingresar
            // 
            this.Ingresar.Location = new System.Drawing.Point(236, 189);
            this.Ingresar.Name = "Ingresar";
            this.Ingresar.Size = new System.Drawing.Size(113, 46);
            this.Ingresar.TabIndex = 4;
            this.Ingresar.Text = "Iniciar Sesion";
            this.Ingresar.UseVisualStyleBackColor = true;
            this.Ingresar.Click += new System.EventHandler(this.Ingresar_Click);
            // 
            // InicioSesion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(708, 345);
            this.Controls.Add(this.Ingresar);
            this.Controls.Add(this.textoPassword);
            this.Controls.Add(this.textoUsuario);
            this.Controls.Add(this.labelPassword);
            this.Controls.Add(this.labelUsuario);
            this.Name = "InicioSesion";
            this.Text = "InicioSesion";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelUsuario;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.TextBox textoUsuario;
        private System.Windows.Forms.TextBox textoPassword;
        private System.Windows.Forms.Button Ingresar;
    }
}