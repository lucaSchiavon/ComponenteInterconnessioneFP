using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using COMPINT_UI.Data;
using COMPINT_UI.Config;

namespace COMPINT_UI
{
    public partial class LogViewerForm : Form
    {
        private readonly LogDao _dao = new LogDao();

        public LogViewerForm()
        {
            InitializeComponent();

            // Open the form maximized so the DataGridView fills the available space
            this.WindowState = FormWindowState.Maximized;

            ConfigManager.Initialize();

            cboLivello.Items.AddRange(new[] { "", "DEBUG", "INFO", "WARNING", "ERROR" });

            // Load macchine from config
            var macs = ConfigManager.Config.Macchine ?? new[] { "", "PRONTOWASH1", "PRONTOWASH2", "MECCANOPLASTICA1", "MECCANOPLASTICA4", "DUETTI", "DUETTI2", "AXOMATIC", "ETICH", "LAY", "PICKER23017", "PICKER23018" };
            cboMacchina.Items.AddRange(macs);

            dgvLogs.CellClick += DgvLogs_CellClick;
            dgvLogs.CellFormatting += DgvLogs_CellFormatting;

            this.Resize += LogViewerForm_Resize;
        }

        // Public helper to preload today's logs
        public void PreloadToday()
        {
            var today = DateTime.Today;
            dtpFrom.Value = today;
            dtpTo.Value = today;
            chkFrom.Checked = true;
            chkTo.Checked = true;

            // clear other filters
            cboLivello.SelectedIndex = 0;
            cboMacchina.SelectedIndex = 0;
            txtCodArt.Clear();
            txtMsgFilter.Clear();

            // load
            LoadLogs(dtpFrom.Value.Date, dtpTo.Value.Date.AddDays(1).AddSeconds(-1), null, null, null, null);
        }

        private void LoadLogs(DateTime? from, DateTime? to, string livello, string macchina, string codArt, string msgFilter)
        {
            try
            {
                var dt = _dao.GetLogs(from, to, livello, macchina, codArt, msgFilter);
                dgvLogs.DataSource = dt;
                // Ensure column sizing and visibility
                AdjustGridColumns();
                // Apply any row colors (ColorRows used to set a baseline but formatting will handle persistent coloring)
                ColorRows();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore durante il caricamento dei log: " + ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LogViewerForm_Resize(object sender, EventArgs e)
        {
            // Docking handles layout when resized or maximized.
            // Keep method in case future adjustments are needed.
        }

        private void DgvLogs_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0) return;
                var row = dgvLogs.Rows[e.RowIndex];

                // If row is selected, use highlight color
                //bool isSelected = row.Selected || (dgvLogs.CurrentCell != null && dgvLogs.CurrentCell.RowIndex == e.RowIndex);
                //if (isSelected)
                //{
                //    e.CellStyle.BackColor = Color.Yellow;
                //    return;
                //}

                // determine livello
                var livello = row.Cells["Livello"].Value?.ToString();
                if (string.IsNullOrEmpty(livello))
                {
                    e.CellStyle.BackColor = SystemColors.Window;
                    return;
                }

                switch (livello.ToUpper())
                {
                    case "ERROR":
                        e.CellStyle.BackColor = Color.LightCoral;
                        break;
                    case "WARNING":
                        e.CellStyle.BackColor = Color.Orange;
                        break;
                    case "INFO":
                        e.CellStyle.BackColor = Color.LightGreen;
                        break;
                    case "DEBUG":
                        e.CellStyle.BackColor = Color.LightGray;
                        break;
                    default:
                        e.CellStyle.BackColor = SystemColors.Window;
                        break;
                }
            }
            catch
            {
                // ignore formatting errors
            }
        }

        private void DgvLogs_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                // Click header or invalid
                return;
            }

            var colName = dgvLogs.Columns[e.ColumnIndex].Name;
            if (string.Equals(colName, "Messaggio", StringComparison.OrdinalIgnoreCase))
            {
                var row = dgvLogs.Rows[e.RowIndex];
                var dataOra = row.Cells["DataOra"].Value?.ToString() ?? "";
                var livello = row.Cells["Livello"].Value?.ToString() ?? "";
                var macchina = row.Cells["Macchina"].Value?.ToString() ?? "";
                var codArt = row.Cells["CodArt"].Value?.ToString() ?? "";
                var fileName = row.Cells["FileName"].Value?.ToString() ?? "";
                var messaggio = row.Cells["Messaggio"].Value?.ToString() ?? "";

                var sb = new StringBuilder();
                sb.AppendLine($"DataOra: {dataOra}");
                sb.AppendLine($"Livello: {livello}");
                sb.AppendLine($"Macchina: {macchina}");
                sb.AppendLine($"Codice Articolo: {codArt}");
                sb.AppendLine($"FileName: {fileName}");
                sb.AppendLine("Messaggio: ");
                sb.AppendLine(messaggio);

                txtMessageViewer.Text = sb.ToString();

                // set background color based on livello
                if (string.Equals(livello, "INFO", StringComparison.OrdinalIgnoreCase))
                {
                    txtMessageViewer.BackColor = Color.FromArgb(225, 245, 225); // soft green
                }
                else if (string.Equals(livello, "ERROR", StringComparison.OrdinalIgnoreCase))
                {
                    txtMessageViewer.BackColor = Color.FromArgb(255, 230, 230); // soft red
                }
                else
                {
                    txtMessageViewer.BackColor = SystemColors.Window;
                }

                txtMessageViewer.Visible = true;
            }
            else
            {
                if (txtMessageViewer.Visible)
                {
                    txtMessageViewer.Visible = false;
                    txtMessageViewer.Text = string.Empty;
                }
            }
        }

        private void btnFiltra_Click(object sender, EventArgs e)
        {
            var from = chkFrom.Checked ? (DateTime?)dtpFrom.Value.Date : null;
            var to = chkTo.Checked ? (DateTime?)dtpTo.Value.Date.AddDays(1).AddSeconds(-1) : null;
            var livello = string.IsNullOrEmpty(cboLivello.Text) ? null : cboLivello.Text;
            var macchina = string.IsNullOrEmpty(cboMacchina.Text) ? null : cboMacchina.Text;
            var codArt = string.IsNullOrEmpty(txtCodArt.Text) ? null : txtCodArt.Text.Trim();
            var msgFilter = string.IsNullOrEmpty(txtMsgFilter.Text) ? null : txtMsgFilter.Text.Trim();

            LoadLogs(from, to, livello, macchina, codArt, msgFilter);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            dtpFrom.Value = DateTime.Now;
            dtpTo.Value = DateTime.Now;
            chkFrom.Checked = false;
            chkTo.Checked = false;
            cboLivello.SelectedIndex = 0;
            cboMacchina.SelectedIndex = 0;
            txtCodArt.Clear();
            txtMsgFilter.Clear();
            dgvLogs.DataSource = null;

            txtMessageViewer.Visible = false;
            txtMessageViewer.Text = string.Empty;
        }

        private void ColorRows()
        {
            // Keep for backward compatibility: trigger a refresh so CellFormatting applies colors.
            dgvLogs.Refresh();
        }

        private void AdjustGridColumns()
        {
            if (dgvLogs.DataSource == null) return;

            // Hide IdLog and StackTrace if present
            if (dgvLogs.Columns.Contains("IdLog"))
            {
                dgvLogs.Columns["IdLog"].Visible = false;
            }
            if (dgvLogs.Columns.Contains("StackTrace"))
            {
                dgvLogs.Columns["StackTrace"].Visible = false;
            }

            // First, set all columns to autosize to content
            foreach (DataGridViewColumn col in dgvLogs.Columns)
            {
                // Keep hidden columns hidden
                if (!col.Visible) continue;

                if (string.Equals(col.Name, "Messaggio", StringComparison.OrdinalIgnoreCase))
                {
                    // Messaggio will fill remaining space
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    // Provide a reasonable minimum so other columns remain visible
                    col.MinimumWidth = 200;
                }
                else
                {
                    // Other columns autosize to their content
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    // Ensure a minimum so they are readable
                    col.MinimumWidth = 60;
                }
            }

            // Force layout update
            dgvLogs.PerformLayout();
        }
    }
}
