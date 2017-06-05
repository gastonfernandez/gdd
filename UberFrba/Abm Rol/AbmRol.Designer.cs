namespace UberFrba.Abm_Rol
{
    partial class AbmRol
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
            this.btnnuevo = new System.Windows.Forms.Button();
            this.btnmod = new System.Windows.Forms.Button();
            this.btnelim = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnnuevo
            // 
            this.btnnuevo.Location = new System.Drawing.Point(44, 167);
            this.btnnuevo.Name = "btnnuevo";
            this.btnnuevo.Size = new System.Drawing.Size(77, 36);
            this.btnnuevo.TabIndex = 0;
            this.btnnuevo.Text = "Agregar";
            this.btnnuevo.UseVisualStyleBackColor = true;
            this.btnnuevo.Click += new System.EventHandler(this.btnnuevo_Click);
            // 
            // btnmod
            // 
            this.btnmod.Location = new System.Drawing.Point(202, 167);
            this.btnmod.Name = "btnmod";
            this.btnmod.Size = new System.Drawing.Size(78, 36);
            this.btnmod.TabIndex = 1;
            this.btnmod.Text = "Modificar";
            this.btnmod.UseVisualStyleBackColor = true;
            this.btnmod.Click += new System.EventHandler(this.btnmod_Click);
            // 
            // btnelim
            // 
            this.btnelim.Location = new System.Drawing.Point(360, 167);
            this.btnelim.Name = "btnelim";
            this.btnelim.Size = new System.Drawing.Size(78, 36);
            this.btnelim.TabIndex = 2;
            this.btnelim.Text = "Inhabilitar";
            this.btnelim.UseVisualStyleBackColor = true;
            this.btnelim.Click += new System.EventHandler(this.btnelim_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(180, 63);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(506, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Seleccione un rol para modificar o inhabilitar. En el caso que desee agregar un n" +
    "uevo rol presione agregar";
            // 
            // AbmRol
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(525, 316);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.btnelim);
            this.Controls.Add(this.btnmod);
            this.Controls.Add(this.btnnuevo);
            this.Name = "AbmRol";
            this.Text = "AbmRol";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnnuevo;
        private System.Windows.Forms.Button btnmod;
        private System.Windows.Forms.Button btnelim;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
    }
}