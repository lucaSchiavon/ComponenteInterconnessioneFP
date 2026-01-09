using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using System.ComponentModel;

namespace COMPINT_UI.Config
{
    public class AppConfig
    {
        public DatabaseConfig Database { get; set; }
        public string[] Macchine { get; set; }
        public LoginConfig Login { get; set; }
    }

    public class DatabaseConfig
    {
        public string ConnectionString { get; set; }
    }

    public class LoginConfig
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public static class ConfigManager
    {
        private static readonly object _lock = new object();
        private static bool _initialized;
        private static AppConfig _config;
        private static string _configPath;

        public static AppConfig Config
        {
            get
            {
                if (!_initialized) Initialize();
                return _config;
            }
        }

        public static void Initialize()
        {
            lock (_lock)
            {
                if (_initialized) return;

                // If running in design-time, avoid file IO and message boxes
                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                {
                    _config = CreateDefaultConfig();
                    _initialized = true;
                    return;
                }

                _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appconfig.json");

                if (!File.Exists(_configPath))
                {
                    // create default
                    _config = CreateDefaultConfig();
                    try
                    {
                        SaveConfig(_configPath, _config);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Impossibile creare file di configurazione di default: " + ex.Message, "Avviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    _initialized = true;
                    return;
                }

                try
                {
                    var json = File.ReadAllText(_configPath, Encoding.UTF8);
                    var ser = new JavaScriptSerializer();
                    _config = ser.Deserialize<AppConfig>(json);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Errore lettura file di configurazione: " + ex.Message + "\nVerrà usata una configurazione di default.", "Errore configurazione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _config = CreateDefaultConfig();
                }

                // ensure non-null sub-objects
                if (_config.Database == null) _config.Database = new DatabaseConfig { ConnectionString = string.Empty };
                if (_config.Macchine == null) _config.Macchine = new[] { "", "ICA1", "MECCANOGRAFICA1", "MECCANOGRAFICA4" };
                if (_config.Login == null) _config.Login = new LoginConfig { Username = "Admin", Password = "revihcra" };

                _initialized = true;
            }
        }

        private static AppConfig CreateDefaultConfig()
        {
            return new AppConfig
            {
                Database = new DatabaseConfig
                {
                    ConnectionString = "Data Source=DESKTOP-CB9ESFC\\\\SQLEXPRESS;Initial Catalog=CompInterconnessioneDB;User Id=sa;PWD=revihcra;"
                },
                Macchine = new[] { "", "ICA1", "MECCANOGRAFICA1", "MECCANOGRAFICA4" },
                Login = new LoginConfig { Username = "Admin", Password = "revihcra" }
            };
        }

        private static void SaveConfig(string path, AppConfig cfg)
        {
            var ser = new JavaScriptSerializer();
            var json = ser.Serialize(cfg);
            File.WriteAllText(path, json, Encoding.UTF8);
        }
    }
}
