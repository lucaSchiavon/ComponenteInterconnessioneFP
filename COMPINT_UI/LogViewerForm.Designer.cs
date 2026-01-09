using System.Drawing;
using System.Windows.Forms;

namespace COMPINT_UI
{
    partial class LogViewerForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelFilters;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.ComboBox cboLivello;
        private System.Windows.Forms.ComboBox cboMacchina;
        private System.Windows.Forms.TextBox txtCodArt;
        private System.Windows.Forms.TextBox txtMsgFilter;
        private System.Windows.Forms.Button btnFiltra;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.DataGridView dgvLogs;
        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.Label lblLivello;
        private System.Windows.Forms.Label lblMacchina;
        private System.Windows.Forms.Label lblCodArt;
        private System.Windows.Forms.Label lblMsgFilter;
        private System.Windows.Forms.CheckBox chkFrom;
        private System.Windows.Forms.CheckBox chkTo;
        private System.Windows.Forms.TextBox txtMessageViewer;

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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panelFilters = new System.Windows.Forms.Panel();
            this.lblFrom = new System.Windows.Forms.Label();
            this.lblTo = new System.Windows.Forms.Label();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.chkFrom = new System.Windows.Forms.CheckBox();
            this.chkTo = new System.Windows.Forms.CheckBox();
            this.lblLivello = new System.Windows.Forms.Label();
            this.lblMacchina = new System.Windows.Forms.Label();
            this.lblCodArt = new System.Windows.Forms.Label();
            this.lblMsgFilter = new System.Windows.Forms.Label();
            this.cboLivello = new System.Windows.Forms.ComboBox();
            this.cboMacchina = new System.Windows.Forms.ComboBox();
            this.txtCodArt = new System.Windows.Forms.TextBox();
            this.txtMsgFilter = new System.Windows.Forms.TextBox();
            this.btnFiltra = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.dgvLogs = new System.Windows.Forms.DataGridView();
            this.txtMessageViewer = new System.Windows.Forms.TextBox();
            this.panelFilters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLogs)).BeginInit();
            this.SuspendLayout();
            // 
            // panelFilters
            // 
            this.panelFilters.Controls.Add(this.lblFrom);
            this.panelFilters.Controls.Add(this.lblTo);
            this.panelFilters.Controls.Add(this.dtpFrom);
            this.panelFilters.Controls.Add(this.dtpTo);
            this.panelFilters.Controls.Add(this.chkFrom);
            this.panelFilters.Controls.Add(this.chkTo);
            this.panelFilters.Controls.Add(this.lblLivello);
            this.panelFilters.Controls.Add(this.lblMacchina);
            this.panelFilters.Controls.Add(this.lblCodArt);
            this.panelFilters.Controls.Add(this.lblMsgFilter);
            this.panelFilters.Controls.Add(this.cboLivello);
            this.panelFilters.Controls.Add(this.cboMacchina);
            this.panelFilters.Controls.Add(this.txtCodArt);
            this.panelFilters.Controls.Add(this.txtMsgFilter);
            this.panelFilters.Controls.Add(this.btnFiltra);
            this.panelFilters.Controls.Add(this.btnReset);
            this.panelFilters.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFilters.Location = new System.Drawing.Point(0, 0);
            this.panelFilters.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panelFilters.Name = "panelFilters";
            this.panelFilters.Padding = new System.Windows.Forms.Padding(14, 18, 14, 18);
            this.panelFilters.Size = new System.Drawing.Size(1981, 192);
            this.panelFilters.TabIndex = 2;
            // 
            // lblFrom
            // 
            this.lblFrom.AutoSize = true;
            this.lblFrom.Location = new System.Drawing.Point(18, 21);
            this.lblFrom.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(36, 28);
            this.lblFrom.TabIndex = 0;
            this.lblFrom.Text = "Da";
            // 
            // lblTo
            // 
            this.lblTo.AutoSize = true;
            this.lblTo.Location = new System.Drawing.Point(323, 16);
            this.lblTo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(25, 28);
            this.lblTo.TabIndex = 1;
            this.lblTo.Text = "A";
            // 
            // dtpFrom
            // 
            this.dtpFrom.Location = new System.Drawing.Point(18, 52);
            this.dtpFrom.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(246, 34);
            this.dtpFrom.TabIndex = 2;
            // 
            // dtpTo
            // 
            this.dtpTo.Location = new System.Drawing.Point(323, 49);
            this.dtpTo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(246, 34);
            this.dtpTo.TabIndex = 3;
            // 
            // chkFrom
            // 
            this.chkFrom.Location = new System.Drawing.Point(275, 52);
            this.chkFrom.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkFrom.Name = "chkFrom";
            this.chkFrom.Size = new System.Drawing.Size(40, 40);
            this.chkFrom.TabIndex = 4;
            // 
            // chkTo
            // 
            this.chkTo.Location = new System.Drawing.Point(583, 49);
            this.chkTo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkTo.Name = "chkTo";
            this.chkTo.Size = new System.Drawing.Size(37, 40);
            this.chkTo.TabIndex = 5;
            // 
            // lblLivello
            // 
            this.lblLivello.AutoSize = true;
            this.lblLivello.Location = new System.Drawing.Point(628, 15);
            this.lblLivello.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLivello.Name = "lblLivello";
            this.lblLivello.Size = new System.Drawing.Size(68, 28);
            this.lblLivello.TabIndex = 6;
            this.lblLivello.Text = "Livello";
            // 
            // lblMacchina
            // 
            this.lblMacchina.AutoSize = true;
            this.lblMacchina.Location = new System.Drawing.Point(793, 15);
            this.lblMacchina.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMacchina.Name = "lblMacchina";
            this.lblMacchina.Size = new System.Drawing.Size(95, 28);
            this.lblMacchina.TabIndex = 7;
            this.lblMacchina.Text = "Macchina";
            // 
            // lblCodArt
            // 
            this.lblCodArt.AutoSize = true;
            this.lblCodArt.Location = new System.Drawing.Point(1001, 16);
            this.lblCodArt.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCodArt.Name = "lblCodArt";
            this.lblCodArt.Size = new System.Drawing.Size(124, 28);
            this.lblCodArt.TabIndex = 8;
            this.lblCodArt.Text = "Cod. articolo";
            // 
            // lblMsgFilter
            // 
            this.lblMsgFilter.AutoSize = true;
            this.lblMsgFilter.Location = new System.Drawing.Point(323, 102);
            this.lblMsgFilter.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMsgFilter.Name = "lblMsgFilter";
            this.lblMsgFilter.Size = new System.Drawing.Size(107, 28);
            this.lblMsgFilter.TabIndex = 9;
            this.lblMsgFilter.Text = "Messaggio";
            // 
            // cboLivello
            // 
            this.cboLivello.Location = new System.Drawing.Point(628, 47);
            this.cboLivello.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cboLivello.Name = "cboLivello";
            this.cboLivello.Size = new System.Drawing.Size(136, 36);
            this.cboLivello.TabIndex = 10;
            // 
            // cboMacchina
            // 
            this.cboMacchina.Location = new System.Drawing.Point(793, 47);
            this.cboMacchina.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cboMacchina.Name = "cboMacchina";
            this.cboMacchina.Size = new System.Drawing.Size(170, 36);
            this.cboMacchina.TabIndex = 11;
            // 
            // txtCodArt
            // 
            this.txtCodArt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCodArt.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtCodArt.Location = new System.Drawing.Point(1001, 47);
            this.txtCodArt.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtCodArt.Name = "txtCodArt";
            this.txtCodArt.Size = new System.Drawing.Size(137, 34);
            this.txtCodArt.TabIndex = 12;
            // 
            // txtMsgFilter
            // 
            this.txtMsgFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMsgFilter.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtMsgFilter.Location = new System.Drawing.Point(323, 135);
            this.txtMsgFilter.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtMsgFilter.Name = "txtMsgFilter";
            this.txtMsgFilter.Size = new System.Drawing.Size(815, 34);
            this.txtMsgFilter.TabIndex = 13;
            // 
            // btnFiltra
            // 
            this.btnFiltra.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnFiltra.FlatAppearance.BorderSize = 0;
            this.btnFiltra.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFiltra.ForeColor = System.Drawing.Color.White;
            this.btnFiltra.Location = new System.Drawing.Point(18, 105);
            this.btnFiltra.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnFiltra.Name = "btnFiltra";
            this.btnFiltra.Size = new System.Drawing.Size(124, 70);
            this.btnFiltra.TabIndex = 14;
            this.btnFiltra.Text = "Filtra";
            this.btnFiltra.UseVisualStyleBackColor = false;
            this.btnFiltra.Click += new System.EventHandler(this.btnFiltra_Click);
            // 
            // btnReset
            // 
            this.btnReset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(152)))), ((int)(((byte)(0)))));
            this.btnReset.FlatAppearance.BorderSize = 0;
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReset.ForeColor = System.Drawing.Color.White;
            this.btnReset.Location = new System.Drawing.Point(158, 105);
            this.btnReset.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(124, 70);
            this.btnReset.TabIndex = 15;
            this.btnReset.Text = "Resetta";
            this.btnReset.UseVisualStyleBackColor = false;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // dgvLogs
            // 
            this.dgvLogs.AllowUserToAddRows = false;
            this.dgvLogs.AllowUserToDeleteRows = false;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLogs.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvLogs.ColumnHeadersHeight = 35;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Segoe UI", 12F);
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvLogs.DefaultCellStyle = dataGridViewCellStyle6;
            this.dgvLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLogs.EnableHeadersVisualStyles = false;
            this.dgvLogs.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.dgvLogs.Location = new System.Drawing.Point(0, 192);
            this.dgvLogs.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgvLogs.Name = "dgvLogs";
            this.dgvLogs.ReadOnly = true;
            this.dgvLogs.RowHeadersWidth = 51;
            this.dgvLogs.RowTemplate.Height = 30;
            this.dgvLogs.Size = new System.Drawing.Size(1981, 472);
            this.dgvLogs.TabIndex = 0;
            // 
            // txtMessageViewer
            // 
            this.txtMessageViewer.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtMessageViewer.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMessageViewer.Location = new System.Drawing.Point(0, 664);
            this.txtMessageViewer.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtMessageViewer.Multiline = true;
            this.txtMessageViewer.Name = "txtMessageViewer";
            this.txtMessageViewer.ReadOnly = true;
            this.txtMessageViewer.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtMessageViewer.Size = new System.Drawing.Size(1981, 186);
            this.txtMessageViewer.TabIndex = 1;
            this.txtMessageViewer.Visible = false;
            // 
            // LogViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.ClientSize = new System.Drawing.Size(1981, 1050);
            this.Controls.Add(this.dgvLogs);
            this.Controls.Add(this.txtMessageViewer);
            this.Controls.Add(this.panelFilters);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "LogViewerForm";
            this.Text = "Visualizza log";
            this.panelFilters.ResumeLayout(false);
            this.panelFilters.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLogs)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}