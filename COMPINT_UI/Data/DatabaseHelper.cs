using System;
using System.Configuration;
using System.Data.SqlClient;
using COMPINT_UI.Config;

namespace COMPINT_UI.Data
{
    public static class DatabaseHelper
    {
        public static string ConnStr
        {
            get
            {
                ConfigManager.Initialize();
                return ConfigManager.Config.Database?.ConnectionString ?? string.Empty;
            }
        }

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnStr);
        }
    }
}
