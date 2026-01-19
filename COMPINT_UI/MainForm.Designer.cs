namespace COMPINT_UI
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem visualizzaLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gestioneLettereIdentificativeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem apriPdfToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gestioneErrorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gestioneGiornoProdToolStripMenuItem;

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbVisualizzaLog;
        private System.Windows.Forms.ToolStripButton tsbLettere;
        private System.Windows.Forms.ToolStripButton tsbErrors;
        private System.Windows.Forms.ToolStripButton tsbGiornoProd;
        private System.Windows.Forms.ToolStripButton tsbApriPdf;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.visualizzaLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gestioneLettereIdentificativeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gestioneErrorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gestioneGiornoProdToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.apriPdfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbVisualizzaLog = new System.Windows.Forms.ToolStripButton();
            this.tsbLettere = new System.Windows.Forms.ToolStripButton();
            this.tsbErrors = new System.Windows.Forms.ToolStripButton();
            this.tsbGiornoProd = new System.Windows.Forms.ToolStripButton();
            this.tsbApriPdf = new System.Windows.Forms.ToolStripButton();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1000, 38);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuToolStripMenuItem
            // 
            this.menuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.visualizzaLogToolStripMenuItem,
            this.gestioneLettereIdentificativeToolStripMenuItem,
            this.gestioneErrorsToolStripMenuItem,
            this.gestioneGiornoProdToolStripMenuItem,
            this.apriPdfToolStripMenuItem});
            this.menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            this.menuToolStripMenuItem.Size = new System.Drawing.Size(60, 34);
            this.menuToolStripMenuItem.Text = "Menu";
            // 
            // visualizzaLogToolStripMenuItem
            // 
            this.visualizzaLogToolStripMenuItem.Name = "visualizzaLogToolStripMenuItem";
            this.visualizzaLogToolStripMenuItem.Size = new System.Drawing.Size(290, 26);
            this.visualizzaLogToolStripMenuItem.Text = "Visualizza Log";
            this.visualizzaLogToolStripMenuItem.Click += new System.EventHandler(this.visualizzaLogToolStripMenuItem_Click);
            // 
            // gestioneLettereIdentificativeToolStripMenuItem
            // 
            this.gestioneLettereIdentificativeToolStripMenuItem.Name = "gestioneLettereIdentificativeToolStripMenuItem";
            this.gestioneLettereIdentificativeToolStripMenuItem.Size = new System.Drawing.Size(290, 26);
            this.gestioneLettereIdentificativeToolStripMenuItem.Text = "Gestione Lettere Identificative";
            this.gestioneLettereIdentificativeToolStripMenuItem.Click += new System.EventHandler(this.gestioneLettereIdentificativeToolStripMenuItem_Click);
            // 
            // gestioneErrorsToolStripMenuItem
            // 
            this.gestioneErrorsToolStripMenuItem.Name = "gestioneErrorsToolStripMenuItem";
            this.gestioneErrorsToolStripMenuItem.Size = new System.Drawing.Size(290, 26);
            this.gestioneErrorsToolStripMenuItem.Text = "Gestione Errors Notifications";
            this.gestioneErrorsToolStripMenuItem.Click += new System.EventHandler(this.gestioneErrorsToolStripMenuItem_Click);
            // 
            // gestioneGiornoProdToolStripMenuItem
            // 
            this.gestioneGiornoProdToolStripMenuItem.Name = "gestioneGiornoProdToolStripMenuItem";
            this.gestioneGiornoProdToolStripMenuItem.Size = new System.Drawing.Size(290, 26);
            this.gestioneGiornoProdToolStripMenuItem.Text = "Gestione GiornoProdConter";
            this.gestioneGiornoProdToolStripMenuItem.Click += new System.EventHandler(this.gestioneGiornoProdToolStripMenuItem_Click);
            // 
            // apriPdfToolStripMenuItem
            // 
            this.apriPdfToolStripMenuItem.Name = "apriPdfToolStripMenuItem";
            this.apriPdfToolStripMenuItem.Size = new System.Drawing.Size(290, 26);
            this.apriPdfToolStripMenuItem.Text = "Apri PDF";
            this.apriPdfToolStripMenuItem.Click += new System.EventHandler(this.apriPdfToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbVisualizzaLog,
            this.tsbLettere,
            this.tsbErrors,
            this.tsbGiornoProd,
            this.tsbApriPdf});
            this.toolStrip1.Location = new System.Drawing.Point(0, 48);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(1000, 39);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbVisualizzaLog
            // 
            this.tsbVisualizzaLog.Image = ((System.Drawing.Image)(resources.GetObject("tsbVisualizzaLog.Image")));
            this.tsbVisualizzaLog.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbVisualizzaLog.Name = "tsbVisualizzaLog";
            this.tsbVisualizzaLog.Size = new System.Drawing.Size(124, 36);
            this.tsbVisualizzaLog.Text = "Visualizza log";
            this.tsbVisualizzaLog.ToolTipText = "Visualizza Log";
            this.tsbVisualizzaLog.Click += new System.EventHandler(this.visualizzaLogToolStripMenuItem_Click);
            // 
            // tsbLettere
            // 
            this.tsbLettere.Image = ((System.Drawing.Image)(resources.GetObject("tsbLettere.Image")));
            this.tsbLettere.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbLettere.Name = "tsbLettere";
            this.tsbLettere.Size = new System.Drawing.Size(235, 36);
            this.tsbLettere.Text = "Gestione Lettera Lotto paglieri";
            this.tsbLettere.ToolTipText = "Gestione Lettere Identificative";
            this.tsbLettere.Click += new System.EventHandler(this.gestioneLettereIdentificativeToolStripMenuItem_Click);
            // 
            // tsbErrors
            // 
            this.tsbErrors.Image = ((System.Drawing.Image)(resources.GetObject("tsbErrors.Image")));
            this.tsbErrors.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbErrors.Name = "tsbErrors";
            this.tsbErrors.Size = new System.Drawing.Size(193, 24);
            this.tsbErrors.Text = "Gestione notifiche errori";
            this.tsbErrors.ToolTipText = "Gestione notifiche errori";
            this.tsbErrors.Click += new System.EventHandler(this.gestioneErrorsToolStripMenuItem_Click);
            // 
            // tsbGiornoProd
            // 
            this.tsbGiornoProd.Image = ((System.Drawing.Image)(resources.GetObject("tsbGiornoProd.Image")));
            this.tsbGiornoProd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbGiornoProd.Name = "tsbGiornoProd";
            this.tsbGiornoProd.Size = new System.Drawing.Size(265, 36);
            this.tsbGiornoProd.Text = "Gestione incrementale lotto Conter";
            this.tsbGiornoProd.ToolTipText = "Gestione incrementale lotto Conter";
            this.tsbGiornoProd.Click += new System.EventHandler(this.gestioneGiornoProdToolStripMenuItem_Click);
            // 
            // tsbApriPdf
            // 
            this.tsbApriPdf.Image = ((System.Drawing.Image)(resources.GetObject("tsbApriPdf.Image")));
            this.tsbApriPdf.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbApriPdf.Name = "tsbApriPdf";
            this.tsbApriPdf.Size = new System.Drawing.Size(146, 36);
            this.tsbApriPdf.Text = "Documentazione";
            this.tsbApriPdf.ToolTipText = "Apri PDF (Documentazione)";
            this.tsbApriPdf.Click += new System.EventHandler(this.apriPdfToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "COMPINT - Main";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}