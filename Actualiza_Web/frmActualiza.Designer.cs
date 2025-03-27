namespace Actualiza_Web
{
    partial class frmActualiza
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmActualiza));
            this.lblActualizando = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.lblRestan = new System.Windows.Forms.Label();
            this.prgBar = new System.Windows.Forms.ProgressBar();
            this.cmdSalir = new System.Windows.Forms.Button();
            this.cmdDetener = new System.Windows.Forms.Button();
            this.cmdIniciar = new System.Windows.Forms.Button();
            this.Notif = new System.Windows.Forms.NotifyIcon(this.components);
            this.cmdMinimizar = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cmdForzar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblActualizando
            // 
            this.lblActualizando.AutoSize = true;
            this.lblActualizando.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblActualizando.ForeColor = System.Drawing.Color.White;
            this.lblActualizando.Location = new System.Drawing.Point(274, 4);
            this.lblActualizando.Name = "lblActualizando";
            this.lblActualizando.Size = new System.Drawing.Size(151, 17);
            this.lblActualizando.TabIndex = 1;
            this.lblActualizando.Text = "Actualizando datos...";
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // lblRestan
            // 
            this.lblRestan.AutoSize = true;
            this.lblRestan.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRestan.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.lblRestan.Location = new System.Drawing.Point(14, 25);
            this.lblRestan.Name = "lblRestan";
            this.lblRestan.Size = new System.Drawing.Size(48, 17);
            this.lblRestan.TabIndex = 2;
            this.lblRestan.Text = "Resto";
            // 
            // prgBar
            // 
            this.prgBar.Location = new System.Drawing.Point(-2, 206);
            this.prgBar.Margin = new System.Windows.Forms.Padding(0);
            this.prgBar.Name = "prgBar";
            this.prgBar.Size = new System.Drawing.Size(475, 25);
            this.prgBar.TabIndex = 3;
            // 
            // cmdSalir
            // 
            this.cmdSalir.Location = new System.Drawing.Point(366, 237);
            this.cmdSalir.Name = "cmdSalir";
            this.cmdSalir.Size = new System.Drawing.Size(87, 25);
            this.cmdSalir.TabIndex = 4;
            this.cmdSalir.Text = "Salir";
            this.cmdSalir.UseVisualStyleBackColor = true;
            this.cmdSalir.Click += new System.EventHandler(this.cmdSalir_Click);
            // 
            // cmdDetener
            // 
            this.cmdDetener.Location = new System.Drawing.Point(150, 237);
            this.cmdDetener.Name = "cmdDetener";
            this.cmdDetener.Size = new System.Drawing.Size(87, 25);
            this.cmdDetener.TabIndex = 5;
            this.cmdDetener.Text = "Detener";
            this.cmdDetener.UseVisualStyleBackColor = true;
            this.cmdDetener.Click += new System.EventHandler(this.cmdDetener_Click);
            // 
            // cmdIniciar
            // 
            this.cmdIniciar.Location = new System.Drawing.Point(12, 237);
            this.cmdIniciar.Name = "cmdIniciar";
            this.cmdIniciar.Size = new System.Drawing.Size(87, 25);
            this.cmdIniciar.TabIndex = 6;
            this.cmdIniciar.Text = "Iniciar";
            this.cmdIniciar.UseVisualStyleBackColor = true;
            this.cmdIniciar.Click += new System.EventHandler(this.cmdIniciar_Click);
            // 
            // Notif
            // 
            this.Notif.BalloonTipText = "Apicación Esperando";
            this.Notif.Icon = ((System.Drawing.Icon)(resources.GetObject("Notif.Icon")));
            this.Notif.Text = "Actualizador";
            this.Notif.Visible = true;
            this.Notif.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Notif_MouseDoubleClick);
            // 
            // cmdMinimizar
            // 
            this.cmdMinimizar.Location = new System.Drawing.Point(443, 0);
            this.cmdMinimizar.Name = "cmdMinimizar";
            this.cmdMinimizar.Size = new System.Drawing.Size(30, 25);
            this.cmdMinimizar.TabIndex = 7;
            this.cmdMinimizar.Text = "-";
            this.cmdMinimizar.UseVisualStyleBackColor = true;
            this.cmdMinimizar.Click += new System.EventHandler(this.cmdMinimizar_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.label1.Location = new System.Drawing.Point(14, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 17);
            this.label1.TabIndex = 8;
            this.label1.Text = "11-01-2022";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // cmdForzar
            // 
            this.cmdForzar.Location = new System.Drawing.Point(257, 237);
            this.cmdForzar.Name = "cmdForzar";
            this.cmdForzar.Size = new System.Drawing.Size(87, 25);
            this.cmdForzar.TabIndex = 9;
            this.cmdForzar.Text = "Forzar";
            this.cmdForzar.UseVisualStyleBackColor = true;
            this.cmdForzar.Click += new System.EventHandler(this.cmdForzar_Click);
            // 
            // frmActualiza
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImage = global::Actualiza_Web.Properties.Resources.migracion;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(470, 267);
            this.Controls.Add(this.cmdForzar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmdMinimizar);
            this.Controls.Add(this.cmdIniciar);
            this.Controls.Add(this.cmdDetener);
            this.Controls.Add(this.cmdSalir);
            this.Controls.Add(this.prgBar);
            this.Controls.Add(this.lblRestan);
            this.Controls.Add(this.lblActualizando);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmActualiza";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Actulizador Web";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.frmActualiza_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblActualizando;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label lblRestan;
        private System.Windows.Forms.ProgressBar prgBar;
        private System.Windows.Forms.Button cmdSalir;
        private System.Windows.Forms.Button cmdDetener;
        private System.Windows.Forms.Button cmdIniciar;
        private System.Windows.Forms.NotifyIcon Notif;
        private System.Windows.Forms.Button cmdMinimizar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cmdForzar;
    }
}

