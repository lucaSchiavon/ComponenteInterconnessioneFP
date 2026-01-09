using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using COMPINT_UI.Data;

namespace COMPINT_UI
{
    public partial class LetterManagementForm : Form
    {
        private System.ComponentModel.IContainer components = null;
        private DataGridView dgvLetters;
        private TextBox txtAnno;
        private TextBox txtLettera;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Label lblAnno;
        private Label lblLettera;

        public LetterManagementForm()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvLetters = new System.Windows.Forms.DataGridView();
            this.txtAnno = new System.Windows.Forms.TextBox();
            this.txtLettera = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lblAnno = new System.Windows.Forms.Label();
            this.lblLettera = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLetters)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvLetters
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.dgvLetters.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvLetters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvLetters.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 12F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLetters.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvLetters.ColumnHeadersHeight = 35;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 12F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvLetters.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvLetters.EnableHeadersVisualStyles = false;
            this.dgvLetters.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.dgvLetters.Location = new System.Drawing.Point(16, 21);
            this.dgvLetters.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgvLetters.MultiSelect = false;
            this.dgvLetters.Name = "dgvLetters";
            this.dgvLetters.ReadOnly = true;
            this.dgvLetters.RowHeadersWidth = 51;
            this.dgvLetters.RowTemplate.Height = 30;
            this.dgvLetters.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLetters.Size = new System.Drawing.Size(483, 328);
            this.dgvLetters.TabIndex = 7;
            this.dgvLetters.SelectionChanged += new System.EventHandler(this.dgvLetters_SelectionChanged);
            // 
            // txtAnno
            // 
            this.txtAnno.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAnno.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtAnno.Location = new System.Drawing.Point(19, 396);
            this.txtAnno.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtAnno.Name = "txtAnno";
            this.txtAnno.Size = new System.Drawing.Size(171, 34);
            this.txtAnno.TabIndex = 6;
            // 
            // txtLettera
            // 
            this.txtLettera.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLettera.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtLettera.Location = new System.Drawing.Point(252, 396);
            this.txtLettera.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtLettera.Name = "txtLettera";
            this.txtLettera.Size = new System.Drawing.Size(247, 34);
            this.txtLettera.TabIndex = 5;
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.btnAdd.FlatAppearance.BorderSize = 0;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.Location = new System.Drawing.Point(137, 462);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(113, 38);
            this.btnAdd.TabIndex = 4;
            this.btnAdd.Text = "Aggiungi";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnEdit.FlatAppearance.BorderSize = 0;
            this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEdit.ForeColor = System.Drawing.Color.White;
            this.btnEdit.Location = new System.Drawing.Point(258, 462);
            this.btnEdit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(116, 38);
            this.btnEdit.TabIndex = 3;
            this.btnEdit.Text = "Modifica";
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(152)))), ((int)(((byte)(0)))));
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(382, 462);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(117, 38);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "Elimina";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lblAnno
            // 
            this.lblAnno.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblAnno.Location = new System.Drawing.Point(18, 354);
            this.lblAnno.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAnno.Name = "lblAnno";
            this.lblAnno.Size = new System.Drawing.Size(172, 37);
            this.lblAnno.TabIndex = 1;
            this.lblAnno.Text = "Anno";
            // 
            // lblLettera
            // 
            this.lblLettera.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblLettera.Location = new System.Drawing.Point(252, 354);
            this.lblLettera.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLettera.Name = "lblLettera";
            this.lblLettera.Size = new System.Drawing.Size(248, 37);
            this.lblLettera.TabIndex = 0;
            this.lblLettera.Text = "Lettera identificativa anno";
            // 
            // LetterManagementForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.ClientSize = new System.Drawing.Size(531, 526);
            this.Controls.Add(this.lblLettera);
            this.Controls.Add(this.lblAnno);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.txtLettera);
            this.Controls.Add(this.txtAnno);
            this.Controls.Add(this.dgvLetters);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LetterManagementForm";
            this.Text = "Gestione lettere identificative lotto Paglieri";
            ((System.ComponentModel.ISupportInitialize)(this.dgvLetters)).EndInit();
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
                using (var cmd = new SqlCommand("SELECT Anno, LetteraIdentAnno FROM TblLetteraIdentAnnoPag ORDER BY Anno", conn))
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    dgvLetters.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore caricamento dati: " + ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtAnno.Text.Trim(), out var anno))
            {
                MessageBox.Show("Anno deve essere un numero intero.", "Validazione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var letter = txtLettera.Text.Trim();
            if (letter.Length == 0 || letter.Length > 2)
            {
                MessageBox.Show("LetteraIdentAnno deve avere max 2 caratteri.", "Validazione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                using (var cmd = new SqlCommand("INSERT INTO TblLetteraIdentAnnoPag (Anno, LetteraIdentAnno) VALUES (@anno, @let)", conn))
                {
                    cmd.Parameters.AddWithValue("@anno", anno);
                    cmd.Parameters.AddWithValue("@let", letter);
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

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvLetters.CurrentRow == null) return;
            if (!int.TryParse(txtAnno.Text.Trim(), out var anno))
            {
                MessageBox.Show("Anno deve essere un numero intero.", "Validazione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var letter = txtLettera.Text.Trim();
            if (letter.Length == 0 || letter.Length > 2)
            {
                MessageBox.Show("LetteraIdentAnno deve avere max 2 caratteri.", "Validazione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                using (var cmd = new SqlCommand("UPDATE TblLetteraIdentAnnoPag SET LetteraIdentAnno = @let WHERE Anno = @anno", conn))
                {
                    cmd.Parameters.AddWithValue("@let", letter);
                    cmd.Parameters.AddWithValue("@anno", anno);
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvLetters.CurrentRow == null) return;
            var anno = Convert.ToInt32(dgvLetters.CurrentRow.Cells["Anno"].Value);
            var res = MessageBox.Show($"Eliminare record anno {anno}?", "Conferma", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res != DialogResult.Yes) return;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                using (var cmd = new SqlCommand("DELETE FROM TblLetteraIdentAnnoPag WHERE Anno = @anno", conn))
                {
                    cmd.Parameters.AddWithValue("@anno", anno);
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

        private void dgvLetters_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLetters.CurrentRow == null) return;
            txtAnno.Text = dgvLetters.CurrentRow.Cells["Anno"].Value.ToString();
            txtLettera.Text = dgvLetters.CurrentRow.Cells["LetteraIdentAnno"].Value.ToString();
        }
    }
}
