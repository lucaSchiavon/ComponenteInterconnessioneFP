using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using COMPINT_UI.Data;

namespace COMPINT_UI
{
    public partial class ErrorsNotificationForm : Form
    {
        private System.ComponentModel.IContainer components = null;
        private DataGridView dgv;
        private TextBox txtSubject;
        private TextBox txtErrorsToCheck;
        private TextBox txtAddresses;
        private TextBox txtSender;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Label lbl1;
        private Label lbl2;
        private Label lbl3;
        private Label lbl4;

        public ErrorsNotificationForm()
        {
            InitializeComponent();
            this.Load += ErrorsNotificationForm_Load;
        }

        private void ErrorsNotificationForm_Load(object sender, EventArgs e)
        {
            if (!this.DesignMode)
            {
                // attach runtime handlers
                this.dgv.SelectionChanged += Dgv_SelectionChanged;
                this.btnAdd.Click += BtnAdd_Click;
                this.btnEdit.Click += BtnEdit_Click;
                this.btnDelete.Click += BtnDelete_Click;

                LoadData();
            }
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.txtSubject = new System.Windows.Forms.TextBox();
            this.txtErrorsToCheck = new System.Windows.Forms.TextBox();
            this.txtAddresses = new System.Windows.Forms.TextBox();
            this.txtSender = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lbl1 = new System.Windows.Forms.Label();
            this.lbl2 = new System.Windows.Forms.Label();
            this.lbl3 = new System.Windows.Forms.Label();
            this.lbl4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.dgv.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgv.ColumnHeadersHeight = 35;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 12F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.dgv.Location = new System.Drawing.Point(12, 12);
            this.dgv.MultiSelect = false;
            this.dgv.Name = "dgv";
            this.dgv.ReadOnly = true;
            this.dgv.RowHeadersWidth = 51;
            this.dgv.RowTemplate.Height = 30;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv.Size = new System.Drawing.Size(760, 334);
            this.dgv.TabIndex = 0;
            // 
            // txtSubject
            // 
            this.txtSubject.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSubject.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtSubject.Location = new System.Drawing.Point(12, 391);
            this.txtSubject.Name = "txtSubject";
            this.txtSubject.Size = new System.Drawing.Size(760, 34);
            this.txtSubject.TabIndex = 1;
            // 
            // txtErrorsToCheck
            // 
            this.txtErrorsToCheck.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtErrorsToCheck.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtErrorsToCheck.Location = new System.Drawing.Point(12, 474);
            this.txtErrorsToCheck.Multiline = true;
            this.txtErrorsToCheck.Name = "txtErrorsToCheck";
            this.txtErrorsToCheck.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtErrorsToCheck.Size = new System.Drawing.Size(760, 60);
            this.txtErrorsToCheck.TabIndex = 2;
            // 
            // txtAddresses
            // 
            this.txtAddresses.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAddresses.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtAddresses.Location = new System.Drawing.Point(12, 568);
            this.txtAddresses.Name = "txtAddresses";
            this.txtAddresses.Size = new System.Drawing.Size(487, 34);
            this.txtAddresses.TabIndex = 3;
            // 
            // txtSender
            // 
            this.txtSender.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSender.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtSender.Location = new System.Drawing.Point(505, 568);
            this.txtSender.Name = "txtSender";
            this.txtSender.Size = new System.Drawing.Size(267, 34);
            this.txtSender.TabIndex = 4;
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.btnAdd.FlatAppearance.BorderSize = 0;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.Location = new System.Drawing.Point(431, 625);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(110, 40);
            this.btnAdd.TabIndex = 5;
            this.btnAdd.Text = "Aggiungi";
            this.btnAdd.UseVisualStyleBackColor = false;
            // 
            // btnEdit
            // 
            this.btnEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnEdit.FlatAppearance.BorderSize = 0;
            this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEdit.ForeColor = System.Drawing.Color.White;
            this.btnEdit.Location = new System.Drawing.Point(547, 625);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(110, 40);
            this.btnEdit.TabIndex = 6;
            this.btnEdit.Text = "Modifica";
            this.btnEdit.UseVisualStyleBackColor = false;
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(152)))), ((int)(((byte)(0)))));
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(663, 625);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(109, 40);
            this.btnDelete.TabIndex = 7;
            this.btnDelete.Text = "Elimina";
            this.btnDelete.UseVisualStyleBackColor = false;
            // 
            // lbl1
            // 
            this.lbl1.AutoSize = true;
            this.lbl1.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lbl1.Location = new System.Drawing.Point(12, 361);
            this.lbl1.Name = "lbl1";
            this.lbl1.Size = new System.Drawing.Size(184, 28);
            this.lbl1.TabIndex = 8;
            this.lbl1.Text = "Soggetto dell\'email";
            // 
            // lbl2
            // 
            this.lbl2.AutoSize = true;
            this.lbl2.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lbl2.Location = new System.Drawing.Point(12, 443);
            this.lbl2.Name = "lbl2";
            this.lbl2.Size = new System.Drawing.Size(341, 28);
            this.lbl2.TabIndex = 9;
            this.lbl2.Text = "Errore da verificare per spedire l\'email";
            // 
            // lbl3
            // 
            this.lbl3.AutoSize = true;
            this.lbl3.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lbl3.Location = new System.Drawing.Point(12, 537);
            this.lbl3.Name = "lbl3";
            this.lbl3.Size = new System.Drawing.Size(106, 28);
            this.lbl3.TabIndex = 10;
            this.lbl3.Text = "Destinatari";
            // 
            // lbl4
            // 
            this.lbl4.AutoSize = true;
            this.lbl4.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lbl4.Location = new System.Drawing.Point(500, 537);
            this.lbl4.Name = "lbl4";
            this.lbl4.Size = new System.Drawing.Size(87, 28);
            this.lbl4.TabIndex = 11;
            this.lbl4.Text = "Mittente";
            // 
            // ErrorsNotificationForm
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.ClientSize = new System.Drawing.Size(784, 677);
            this.Controls.Add(this.lbl1);
            this.Controls.Add(this.lbl2);
            this.Controls.Add(this.lbl3);
            this.Controls.Add(this.lbl4);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.txtSubject);
            this.Controls.Add(this.txtErrorsToCheck);
            this.Controls.Add(this.txtAddresses);
            this.Controls.Add(this.txtSender);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnDelete);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.Name = "ErrorsNotificationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Gestione Errori per notifica email";
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void LoadData()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                using (var cmd = new SqlCommand("SELECT SubjectOfEmail, ErrorsToCheckForNotifications, Addresses, Sender FROM TblErrorsUsedForNotifications", conn))
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    dgv.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                if (!this.DesignMode)
                {
                    MessageBox.Show("Errore caricamento dati: " + ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            txtSubject.Text = dgv.CurrentRow.Cells["SubjectOfEmail"].Value?.ToString() ?? string.Empty;
            txtErrorsToCheck.Text = dgv.CurrentRow.Cells["ErrorsToCheckForNotifications"].Value?.ToString() ?? string.Empty;
            txtAddresses.Text = dgv.CurrentRow.Cells["Addresses"].Value?.ToString() ?? string.Empty;
            txtSender.Text = dgv.CurrentRow.Cells["Sender"].Value?.ToString() ?? string.Empty;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                using (var cmd = new SqlCommand("INSERT INTO TblErrorsUsedForNotifications (SubjectOfEmail, ErrorsToCheckForNotifications, Addresses, Sender) VALUES (@sub, @errs, @addr, @sender)", conn))
                {
                    cmd.Parameters.AddWithValue("@sub", (object)txtSubject.Text ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@errs", (object)txtErrorsToCheck.Text ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@addr", (object)txtAddresses.Text ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@sender", (object)txtSender.Text ?? DBNull.Value);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadData();
                MessageBox.Show("Record aggiunto.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore aggiunta: " + ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;

            // As the table has no primary key, use all original values to identify the row in WHERE clause
            var origSubject = dgv.CurrentRow.Cells["SubjectOfEmail"].Value?.ToString();
            var origErrors = dgv.CurrentRow.Cells["ErrorsToCheckForNotifications"].Value?.ToString();
            var origAddresses = dgv.CurrentRow.Cells["Addresses"].Value?.ToString();
            var origSender = dgv.CurrentRow.Cells["Sender"].Value?.ToString();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                using (var cmd = new SqlCommand(
                    "UPDATE TblErrorsUsedForNotifications SET SubjectOfEmail=@sub, ErrorsToCheckForNotifications=@errs, Addresses=@addr, Sender=@sender WHERE (SubjectOfEmail=@origSub OR (@origSub IS NULL AND SubjectOfEmail IS NULL)) AND (ErrorsToCheckForNotifications=@origErrs OR (@origErrs IS NULL AND ErrorsToCheckForNotifications IS NULL)) AND (Addresses=@origAddr OR (@origAddr IS NULL AND Addresses IS NULL)) AND (Sender=@origSender OR (@origSender IS NULL AND Sender IS NULL))", conn))
                {
                    cmd.Parameters.AddWithValue("@sub", (object)txtSubject.Text ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@errs", (object)txtErrorsToCheck.Text ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@addr", (object)txtAddresses.Text ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@sender", (object)txtSender.Text ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@origSub", (object)origSubject ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@origErrs", (object)origErrors ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@origAddr", (object)origAddresses ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@origSender", (object)origSender ?? DBNull.Value);

                    conn.Open();
                    var rows = cmd.ExecuteNonQuery();
                    if (rows == 0)
                    {
                        MessageBox.Show("Nessun record aggiornato. La riga potrebbe essere stata modificata da altri.", "Avviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                LoadData();
                MessageBox.Show("Record aggiornato.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore aggiornamento: " + ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;

            var origSubject = dgv.CurrentRow.Cells["SubjectOfEmail"].Value?.ToString();
            var origErrors = dgv.CurrentRow.Cells["ErrorsToCheckForNotifications"].Value?.ToString();
            var origAddresses = dgv.CurrentRow.Cells["Addresses"].Value?.ToString();
            var origSender = dgv.CurrentRow.Cells["Sender"].Value?.ToString();

            var res = MessageBox.Show("Eliminare il record selezionato?", "Conferma", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res != DialogResult.Yes) return;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                using (var cmd = new SqlCommand(
                    "DELETE FROM TblErrorsUsedForNotifications WHERE (SubjectOfEmail=@origSub OR (@origSub IS NULL AND SubjectOfEmail IS NULL)) AND (ErrorsToCheckForNotifications=@origErrs OR (@origErrs IS NULL AND ErrorsToCheckForNotifications IS NULL)) AND (Addresses=@origAddr OR (@origAddr IS NULL AND Addresses IS NULL)) AND (Sender=@origSender OR (@origSender IS NULL AND Sender IS NULL))", conn))
                {
                    cmd.Parameters.AddWithValue("@origSub", (object)origSubject ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@origErrs", (object)origErrors ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@origAddr", (object)origAddresses ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@origSender", (object)origSender ?? DBNull.Value);

                    conn.Open();
                    var rows = cmd.ExecuteNonQuery();
                    if (rows == 0)
                    {
                        MessageBox.Show("Nessun record eliminato. La riga potrebbe essere stata modificata da altri.", "Avviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                LoadData();
                MessageBox.Show("Record eliminato.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore eliminazione: " + ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
