using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace COMPINT_UI
{
    //todo:metti a posto come formattazione le form (sposta i controlli in basso
    //todo:fai si che si vedano in design time
    //todo:crea barra di strumenti con bottoni
    //todo:evita che le form si massimizzino (fisse)
    //todo:da due accessi amministrativo ed utente l'utente vede solo logviewer
    //todo:modifica documentazione aggiungendo parte tecnica e modifiche in corso includendo parte gest email
    //todo:includi documentazione finita
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.Shown += MainForm_Shown;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            // If a LogViewerForm is not already open, open and preload today's logs
            var existing = this.MdiChildren.OfType<LogViewerForm>().FirstOrDefault();
            if (existing == null)
            {
                var f = new LogViewerForm();
                f.MdiParent = this;
                f.WindowState = FormWindowState.Maximized;
                f.Show();
                try
                {
                    f.PreloadToday();
                }
                catch
                {
                    // ignore preload errors
                }
            }
            else
            {
                existing.WindowState = FormWindowState.Maximized;
                try { existing.PreloadToday(); } catch { }
                existing.Show();
            }
        }

        private void visualizzaLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = new LogViewerForm();
            f.MdiParent = this;
            f.WindowState = FormWindowState.Maximized;
            f.Show();
        }

        private void gestioneLettereIdentificativeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var f = new LetterManagementForm())
            {
                //f.MdiParent = this;
                //f.WindowState = FormWindowState.Normal;
                //f.Show();
                f.ShowDialog();
            }
        }

        private void gestioneErrorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var f = new ErrorsNotificationForm())
            {
               
                f.ShowDialog();
            }
            //var f = new ErrorsNotificationForm();
            //f.MdiParent = this;
            //f.WindowState = FormWindowState.Normal;
            //f.Show();
        }

        private void gestioneGiornoProdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var f = new GiornoProdCounterForm())
            {

                f.ShowDialog();
            }

            //var f = new GiornoProdCounterForm();
            //f.MdiParent = this;
            //f.WindowState = FormWindowState.Normal;
            //f.Show();
        }

        private void apriPdfToolStripMenuItem_Click(object sender, EventArgs e)
        {

            // Path dell'eseguibile + nome del PDF
            string path = Path.Combine(Application.StartupPath, "Manuale tecnico COMPINT.pdf");

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true
                };

                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Impossibile aprire la documentazione: " + ex.Message,
                                "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}