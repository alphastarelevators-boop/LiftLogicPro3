using DevExpress.Mvvm;
using Microsoft.Data.SqlClient;
using System;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace LiftLogicPro.ViewModels.Connection
{
    public class ConnectionViewModel : ViewModelBase
    {
        public string ServerName
        {
            get => GetProperty(() => ServerName);
            set => SetProperty(() => ServerName, value);
        }

        public string DatabaseName
        {
            get => GetProperty(() => DatabaseName);
            set => SetProperty(() => DatabaseName, value);
        }

        public string UserId
        {
            get => GetProperty(() => UserId);
            set => SetProperty(() => UserId, value);
        }

        public string Password
        {
            get => GetProperty(() => Password);
            set => SetProperty(() => Password, value);
        }

        public string StatusMessage
        {
            get => GetProperty(() => StatusMessage);
            set => SetProperty(() => StatusMessage, value);
        }

        public bool IsConnecting
        {
            get => GetProperty(() => IsConnecting);
            set => SetProperty(() => IsConnecting, value);
        }

        public DelegateCommand TestConnectionCommand { get; set; }
        public DelegateCommand SaveConnectionCommand { get; set; }

        public ConnectionViewModel()
        {
            TestConnectionCommand = new DelegateCommand(TestConnection, CanTest);
            SaveConnectionCommand = new DelegateCommand(SaveConnection, CanSave);

            LoadSettings();
        }

        private bool CanTest()
        {
            return !string.IsNullOrWhiteSpace(ServerName) &&
                   !string.IsNullOrWhiteSpace(DatabaseName) &&
                   !string.IsNullOrWhiteSpace(UserId) &&
                   !string.IsNullOrWhiteSpace(Password);
        }

        private bool CanSave()
        {
            return CanTest();
        }

        private void TestConnection()
        {
            IsConnecting = true;
            StatusMessage = "جاري اختبار الاتصال...";

            try
            {
                string connectionString = $"Server={ServerName};Database={DatabaseName};User Id={UserId};Password={Password};TrustServerCertificate=True;";

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    StatusMessage = "✅ الاتصال ناجح!";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ فشل الاتصال: {ex.Message}";
            }
            finally
            {
                IsConnecting = false;
            }
        }

        private void SaveConnection()
        {
            try
            {
                var settings = new ConnectionSettings
                {
                    ServerName = ServerName,
                    DatabaseName = DatabaseName,
                    UserId = UserId,
                    Password = Password
                };

                string json = JsonSerializer.Serialize(settings);
                File.WriteAllText("connection.json", json);

                StatusMessage = "✅ تم حفظ الإعدادات بنجاح!";

                // فتح شاشة الدخول
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    var loginView = new Views.Login.LoginView();
                    loginView.Show();

                    // إغلاق شاشة الاتصال
                    foreach (Window window in System.Windows.Application.Current.Windows)
                    {
                        if (window is Views.Connection.ConnectionView)
                        {
                            window.Close();
                            break;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ فشل الحفظ: {ex.Message}";
            }
        }

        private void LoadSettings()
        {
            try
            {
                if (File.Exists("connection.json"))
                {
                    string json = File.ReadAllText("connection.json");
                    var settings = JsonSerializer.Deserialize<ConnectionSettings>(json);

                    ServerName = settings?.ServerName ?? "";
                    DatabaseName = settings?.DatabaseName ?? "";
                    UserId = settings?.UserId ?? "";
                    Password = settings?.Password ?? "";
                }
            }
            catch { }
        }
    }

    public class ConnectionSettings
    {
        public string ServerName { get; set; }
        public string DatabaseName { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
    }
}