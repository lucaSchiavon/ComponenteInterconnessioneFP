using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace COMPINT_UI.Data
{
    public class LogDao
    {
        public DataTable GetLogs(DateTime? from, DateTime? to, string livello, string macchina, string codArt, string messageFilter)
        {
            var dt = new DataTable();
            var sb = new StringBuilder();
            sb.Append("SELECT IdLog, DataOra, Livello, Messaggio, StackTrace, Macchina, CodArt, FileName FROM TblLog WHERE 1=1");

            if (from.HasValue)
            {
                sb.Append(" AND DataOra >= @from");
            }
            if (to.HasValue)
            {
                sb.Append(" AND DataOra <= @to");
            }
            if (!string.IsNullOrEmpty(livello))
            {
                sb.Append(" AND Livello = @livello");
            }
            if (!string.IsNullOrEmpty(macchina))
            {
                sb.Append(" AND Macchina = @macchina");
            }
            if (!string.IsNullOrEmpty(codArt))
            {
                sb.Append(" AND CodArt LIKE @codArt");
            }
            if (!string.IsNullOrEmpty(messageFilter))
            {
                sb.Append(" AND Messaggio LIKE @messaggio");
            }

            sb.Append(" ORDER BY DataOra DESC");

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                if (from.HasValue) cmd.Parameters.AddWithValue("@from", from.Value);
                if (to.HasValue) cmd.Parameters.AddWithValue("@to", to.Value);
                if (!string.IsNullOrEmpty(livello)) cmd.Parameters.AddWithValue("@livello", livello);
                if (!string.IsNullOrEmpty(macchina)) cmd.Parameters.AddWithValue("@macchina", macchina);
                if (!string.IsNullOrEmpty(codArt)) cmd.Parameters.AddWithValue("@codArt", "%" + codArt + "%");
                if (!string.IsNullOrEmpty(messageFilter)) cmd.Parameters.AddWithValue("@messaggio", "%" + messageFilter + "%");

                da.Fill(dt);
            }

            return dt;
        }
    }
}
