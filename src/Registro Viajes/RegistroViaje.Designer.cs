namespace UberFrba.Registro_Viajes
{
    partial class RegistroViaje
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
            this.tbNombre = new System.Windows.Forms.TextBox();
            this.tbDni = new System.Windows.Forms.TextBox();
            this.tbApellido = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btCargarChofer = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.tbApellidoCliente = new System.Windows.Forms.TextBox();
            this.tbDniCliente = new System.Windows.Forms.TextBox();
            this.tbNombreCliente = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.dgvChofer = new System.Windows.Forms.DataGridView();
            this.dgvCliente = new System.Windows.Forms.DataGridView();
            this.dtFechaDesde = new System.Windows.Forms.DateTimePicker();
            this.dtFechaHasta = new System.Windows.Forms.DateTimePicker();
            this.tbKM = new System.Windows.Forms.TextBox();
            this.km = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btRegistrar = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChofer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCliente)).BeginInit();
            this.SuspendLayout();
            // 
            // tbNombre
            // 
            this.tbNombre.Location = new System.Drawing.Point(82, 53);
            this.tbNombre.Name = "tbNombre";
            this.tbNombre.Size = new System.Drawing.Size(163, 20);
            this.tbNombre.TabIndex = 0;
            // 
            // tbDni
            // 
            this.tbDni.Location = new System.Drawing.Point(622, 53);
            this.tbDni.Name = "tbDni";
            this.tbDni.Size = new System.Drawing.Size(163, 20);
            this.tbDni.TabIndex = 4;
            this.tbDni.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbDni_KeyPress);
            // 
            // tbApellido
            // 
            this.tbApellido.Location = new System.Drawing.Point(346, 53);
            this.tbApellido.Name = "tbApellido";
            this.tbApellido.Size = new System.Drawing.Size(176, 20);
            this.tbApellido.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Nombre";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(551, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Dni";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(276, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Apellido";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(35, 18);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(69, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Datos Chofer";
            // 
            // btCargarChofer
            // 
            this.btCargarChofer.Location = new System.Drawing.Point(78, 263);
            this.btCargarChofer.Name = "btCargarChofer";
            this.btCargarChofer.Size = new System.Drawing.Size(711, 23);
            this.btCargarChofer.TabIndex = 17;
            this.btCargarChofer.Text = "Buscar datos de Chofer";
            this.btCargarChofer.UseVisualStyleBackColor = true;
            this.btCargarChofer.Click += new System.EventHandler(this.btCargarChofer_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(58, 583);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(748, 23);
            this.button1.TabIndex = 30;
            this.button1.Text = "Buscar datos de Cliente";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(260, 382);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(44, 13);
            this.label11.TabIndex = 26;
            this.label11.Text = "Apellido";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(567, 517);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(23, 13);
            this.label12.TabIndex = 25;
            this.label12.Text = "Dni";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(8, 389);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(44, 13);
            this.label13.TabIndex = 24;
            this.label13.Text = "Nombre";
            // 
            // tbApellidoCliente
            // 
            this.tbApellidoCliente.Location = new System.Drawing.Point(330, 382);
            this.tbApellidoCliente.Name = "tbApellidoCliente";
            this.tbApellidoCliente.Size = new System.Drawing.Size(176, 20);
            this.tbApellidoCliente.TabIndex = 23;
            // 
            // tbDniCliente
            // 
            this.tbDniCliente.Location = new System.Drawing.Point(606, 386);
            this.tbDniCliente.Name = "tbDniCliente";
            this.tbDniCliente.Size = new System.Drawing.Size(163, 20);
            this.tbDniCliente.TabIndex = 21;
            this.tbDniCliente.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbDniCliente_KeyPress);
            // 
            // tbNombreCliente
            // 
            this.tbNombreCliente.Location = new System.Drawing.Point(66, 382);
            this.tbNombreCliente.Name = "tbNombreCliente";
            this.tbNombreCliente.Size = new System.Drawing.Size(163, 20);
            this.tbNombreCliente.TabIndex = 18;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(8, 357);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(70, 13);
            this.label14.TabIndex = 31;
            this.label14.Text = "Datos Cliente";
            // 
            // dgvChofer
            // 
            this.dgvChofer.AllowUserToAddRows = false;
            this.dgvChofer.AllowUserToDeleteRows = false;
            this.dgvChofer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvChofer.Location = new System.Drawing.Point(78, 90);
            this.dgvChofer.Name = "dgvChofer";
            this.dgvChofer.ReadOnly = true;
            this.dgvChofer.Size = new System.Drawing.Size(707, 150);
            this.dgvChofer.TabIndex = 32;
            // 
            // dgvCliente
            // 
            this.dgvCliente.AllowUserToAddRows = false;
            this.dgvCliente.AllowUserToDeleteRows = false;
            this.dgvCliente.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCliente.Location = new System.Drawing.Point(58, 427);
            this.dgvCliente.Name = "dgvCliente";
            this.dgvCliente.ReadOnly = true;
            this.dgvCliente.Size = new System.Drawing.Size(748, 150);
            this.dgvCliente.TabIndex = 33;
            // 
            // dtFechaDesde
            // 
            this.dtFechaDesde.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.dtFechaDesde.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtFechaDesde.Location = new System.Drawing.Point(346, 314);
            this.dtFechaDesde.Name = "dtFechaDesde";
            this.dtFechaDesde.Size = new System.Drawing.Size(200, 20);
            this.dtFechaDesde.TabIndex = 34;
            // 
            // dtFechaHasta
            // 
            this.dtFechaHasta.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.dtFechaHasta.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtFechaHasta.Location = new System.Drawing.Point(652, 314);
            this.dtFechaHasta.Name = "dtFechaHasta";
            this.dtFechaHasta.Size = new System.Drawing.Size(200, 20);
            this.dtFechaHasta.TabIndex = 35;
            // 
            // tbKM
            // 
            this.tbKM.Location = new System.Drawing.Point(129, 314);
            this.tbKM.Name = "tbKM";
            this.tbKM.Size = new System.Drawing.Size(100, 20);
            this.tbKM.TabIndex = 36;
            this.tbKM.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbKM_KeyPress_1);
            // 
            // km
            // 
            this.km.AutoSize = true;
            this.km.Location = new System.Drawing.Point(75, 317);
            this.km.Name = "km";
            this.km.Size = new System.Drawing.Size(44, 13);
            this.km.TabIndex = 37;
            this.km.Text = "CantKm";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(249, 317);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 13);
            this.label5.TabIndex = 38;
            this.label5.Text = "Fecha Desde";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(567, 317);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(68, 13);
            this.label7.TabIndex = 39;
            this.label7.Text = "Fecha Hasta";
            // 
            // btRegistrar
            // 
            this.btRegistrar.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btRegistrar.Location = new System.Drawing.Point(492, 632);
            this.btRegistrar.Name = "btRegistrar";
            this.btRegistrar.Size = new System.Drawing.Size(267, 57);
            this.btRegistrar.TabIndex = 41;
            this.btRegistrar.Text = "Registrar ";
            this.btRegistrar.UseVisualStyleBackColor = false;
            this.btRegistrar.Click += new System.EventHandler(this.button3_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(551, 389);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(23, 13);
            this.label4.TabIndex = 43;
            this.label4.Text = "Dni";
            // 
            // RegistroViaje
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(921, 701);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btRegistrar);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.km);
            this.Controls.Add(this.tbKM);
            this.Controls.Add(this.dtFechaHasta);
            this.Controls.Add(this.dtFechaDesde);
            this.Controls.Add(this.dgvCliente);
            this.Controls.Add(this.dgvChofer);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.tbApellidoCliente);
            this.Controls.Add(this.tbDniCliente);
            this.Controls.Add(this.tbNombreCliente);
            this.Controls.Add(this.btCargarChofer);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbApellido);
            this.Controls.Add(this.tbDni);
            this.Controls.Add(this.tbNombre);
            this.Name = "RegistroViaje";
            this.Text = "Registro Viajes";
            this.Load += new System.EventHandler(this.RegistroViaje_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvChofer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCliente)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbNombre;
        private System.Windows.Forms.TextBox tbDni;
        private System.Windows.Forms.TextBox tbApellido;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btCargarChofer;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox tbApellidoCliente;
        private System.Windows.Forms.TextBox tbDniCliente;
        private System.Windows.Forms.TextBox tbNombreCliente;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.DataGridView dgvChofer;
        private System.Windows.Forms.DataGridView dgvCliente;
        private System.Windows.Forms.DateTimePicker dtFechaDesde;
        private System.Windows.Forms.DateTimePicker dtFechaHasta;
        private System.Windows.Forms.TextBox tbKM;
        private System.Windows.Forms.Label km;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btRegistrar;
        private System.Windows.Forms.Label label4;
    }
}