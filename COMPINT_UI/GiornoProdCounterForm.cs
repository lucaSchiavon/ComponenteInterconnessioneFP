using COMPINT_UI.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace COMPINT_UI
{
    public class GiornoProdCounterForm : Form
    {
        private DataGridView dgv;
        //private DateTimePicker dtpDataProduzione;
        private TextBox txtIncrementale;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Label label1;
        private TextBox txtDataProduzione;
        private Label lbl2;

        public GiornoProdCounterForm()
        {
            InitializeComponent();
            this.Load += GiornoProdCounterForm_Load;
        }

        private void GiornoProdCounterForm_Load(object sender, EventArgs e)
        {
            // Attach event handlers only at runtime (not at design time)
            if (!this.DesignMode)
            {
                // wire events
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
            this.txtIncrementale = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lbl2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtDataProduzione = new System.Windows.Forms.TextBox();
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
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 12F);
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
            this.dgv.Size = new System.Drawing.Size(760, 297);
            this.dgv.TabIndex = 0;
            // 
            // txtIncrementale
            // 
            this.txtIncrementale.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtIncrementale.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtIncrementale.Location = new System.Drawing.Point(252, 359);
            this.txtIncrementale.Name = "txtIncrementale";
            this.txtIncrementale.Size = new System.Drawing.Size(312, 34);
            this.txtIncrementale.TabIndex = 2;
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.btnAdd.FlatAppearance.BorderSize = 0;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.Location = new System.Drawing.Point(424, 413);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(114, 38);
            this.btnAdd.TabIndex = 5;
            this.btnAdd.Text = "Aggiungi";
            this.btnAdd.UseVisualStyleBackColor = false;
            //this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click_1);
            // 
            // btnEdit
            // 
            this.btnEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnEdit.FlatAppearance.BorderSize = 0;
            this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEdit.ForeColor = System.Drawing.Color.White;
            this.btnEdit.Location = new System.Drawing.Point(544, 413);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(112, 38);
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
            this.btnDelete.Location = new System.Drawing.Point(662, 413);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(110, 38);
            this.btnDelete.TabIndex = 7;
            this.btnDelete.Text = "Elimina";
            this.btnDelete.UseVisualStyleBackColor = false;
            // 
            // lbl2
            // 
            this.lbl2.AutoSize = true;
            this.lbl2.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lbl2.Location = new System.Drawing.Point(247, 328);
            this.lbl2.Name = "lbl2";
            this.lbl2.Size = new System.Drawing.Size(317, 28);
            this.lbl2.TabIndex = 3;
            this.lbl2.Text = "Incrementale giorno di prodConter";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.label1.Location = new System.Drawing.Point(12, 328);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(181, 28);
            this.label1.TabIndex = 8;
            this.label1.Text = "Data di produzione";
            // 
            // txtDataProduzione
            // 
            this.txtDataProduzione.Location = new System.Drawing.Point(17, 359);
            this.txtDataProduzione.Name = "txtDataProduzione";
            this.txtDataProduzione.Size = new System.Drawing.Size(220, 34);
            this.txtDataProduzione.TabIndex = 9;
            // 
            // GiornoProdCounterForm
            // 
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.ClientSize = new System.Drawing.Size(784, 463);
            this.Controls.Add(this.txtDataProduzione);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.lbl2);
            this.Controls.Add(this.txtIncrementale);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnDelete);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.Location = new System.Drawing.Point(12, 320);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GiornoProdCounterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Gestione giorni produzione";
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void LoadData()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                using (var cmd = new SqlCommand("SELECT DataProduzione, IncrementaleGDiProdConter FROM TblGiornoProdConter ORDER BY DataProduzione, IncrementaleGDiProdConter", conn))
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    dgv.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                // do not throw at design time; show message only at runtime
                if (!this.DesignMode)
                {
                    MessageBox.Show("Errore caricamento dati: " + ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;

            var cellValue = dgv.CurrentRow.Cells["DataProduzione"].Value;

            if (cellValue == null || cellValue == DBNull.Value || string.IsNullOrWhiteSpace(cellValue.ToString()))
            {
                // Eviti l'assegnazione e imposti un valore di default
                //dtpDataProduzione.Value = DateTime.Today; // oppure NON fai nulla
                txtDataProduzione.Text = DateTime.Today.ToString();
            }
            else
            {
                //dtpDataProduzione.Value = Convert.ToDateTime(cellValue);
                txtDataProduzione.Text= cellValue.ToString();
            }

            txtIncrementale.Text = dgv.CurrentRow.Cells["IncrementaleGDiProdConter"].Value?.ToString() ?? string.Empty;
        }


        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtIncrementale.Text.Trim(), out var inc))
            {
                MessageBox.Show("Incrementale deve essere un intero.", "Validazione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                using (var cmd = new SqlCommand("INSERT INTO TblGiornoProdConter (DataProduzione, IncrementaleGDiProdConter) VALUES (@data, @inc)", conn))
                {
                    //cmd.Parameters.AddWithValue("@data", dtpDataProduzione.Value.Date);
                    cmd.Parameters.AddWithValue("@data", Convert.ToDateTime(txtDataProduzione.Text));
                    cmd.Parameters.AddWithValue("@inc", inc);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                //scrive nel log che è stato fatto un inserimento manuale
                using (var conn = DatabaseHelper.GetConnection())
                using (var cmd = new SqlCommand("INSERT INTO [dbo].[TblLogUI]([TipoModifica],[DettaglioModifica]) VALUES (@TipoModifica,@DettaglioModifica)", conn))
                {
                    cmd.Parameters.AddWithValue("@TipoModifica","Gest. giorni prod. Conter - INSERIMENTO");
                    cmd.Parameters.AddWithValue("@DettaglioModifica", $"INSERT INTO TblGiornoProdConter (DataProduzione, IncrementaleGDiProdConter) VALUES ({txtDataProduzione.Text},{inc})");
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

            if (!int.TryParse(txtIncrementale.Text.Trim(), out var inc))
            {
                MessageBox.Show("Incrementale deve essere un intero.", "Validazione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!DateTime.TryParse(txtDataProduzione.Text.Trim(), out var dataProd))
            {
                MessageBox.Show("Data di produzione deve essere una data.", "Validazione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var origData = Convert.ToDateTime(dgv.CurrentRow.Cells["DataProduzione"].Value);
            var origInc = Convert.ToInt32(dgv.CurrentRow.Cells["IncrementaleGDiProdConter"].Value);

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                using (var cmd = new SqlCommand("UPDATE TblGiornoProdConter SET IncrementaleGDiProdConter=@inc, DataProduzione=@dataProd WHERE DataProduzione=@origData AND IncrementaleGDiProdConter=@origInc", conn))
                {
                    cmd.Parameters.AddWithValue("@inc", inc);
                    cmd.Parameters.AddWithValue("@dataProd", dataProd);
                    cmd.Parameters.AddWithValue("@origData", origData);
                    cmd.Parameters.AddWithValue("@origInc", origInc);
                    conn.Open();
                    var rows = cmd.ExecuteNonQuery();
                    if (rows == 0) MessageBox.Show("Nessun record aggiornato.", "Avviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                //scrive nel log che è stato fatto una modifica manuale
                using (var conn = DatabaseHelper.GetConnection())
                using (var cmd = new SqlCommand("INSERT INTO [dbo].[TblLogUI]([TipoModifica],[DettaglioModifica]) VALUES (@TipoModifica,@DettaglioModifica)", conn))
                {
                    cmd.Parameters.AddWithValue("@TipoModifica", "Gest. giorni prod. Conter - MODIFICA");
                    cmd.Parameters.AddWithValue("@DettaglioModifica", $"UPDATE TblGiornoProdConter SET IncrementaleGDiProdConter = {inc}, DataProduzione = {dataProd} WHERE DataProduzione = {origData} AND IncrementaleGDiProdConter = {origInc}");
                    conn.Open();
                    cmd.ExecuteNonQuery();
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

            var origData = Convert.ToDateTime(dgv.CurrentRow.Cells["DataProduzione"].Value);
            var origInc = Convert.ToInt32(dgv.CurrentRow.Cells["IncrementaleGDiProdConter"].Value);

            var res = MessageBox.Show("Eliminare il record selezionato?", "Conferma", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res != DialogResult.Yes) return;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                using (var cmd = new SqlCommand("DELETE FROM TblGiornoProdConter WHERE DataProduzione=@origData AND IncrementaleGDiProdConter=@origInc", conn))
                {
                    cmd.Parameters.AddWithValue("@origData", origData);
                    cmd.Parameters.AddWithValue("@origInc", origInc);
                    conn.Open();
                    var rows = cmd.ExecuteNonQuery();
                    if (rows == 0) MessageBox.Show("Nessun record eliminato.", "Avviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                //scrive nel log che è stato fatto una eliminazione manuale
                using (var conn = DatabaseHelper.GetConnection())
                using (var cmd = new SqlCommand("INSERT INTO [dbo].[TblLogUI]([TipoModifica],[DettaglioModifica]) VALUES (@TipoModifica,@DettaglioModifica)", conn))
                {
                    cmd.Parameters.AddWithValue("@TipoModifica", "Gest. giorni prod. Conter - ELIMINAZIONE");
                    cmd.Parameters.AddWithValue("@DettaglioModifica", $"DELETE FROM TblGiornoProdConter WHERE DataProduzione={origData} AND IncrementaleGDiProdConter={origInc}");
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                LoadData();
                MessageBox.Show("Record eliminato.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore eliminazione: " + ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //private void btnAdd_Click_1(object sender, EventArgs e)
        //{

        //}
    }
}
