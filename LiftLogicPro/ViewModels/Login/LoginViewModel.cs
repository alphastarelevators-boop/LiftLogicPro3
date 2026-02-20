using DevExpress.Mvvm;
using Microsoft.Data.SqlClient;
using System;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace LiftLogicPro.ViewModels.Login
{
    public class LoginViewModel : ViewModelBase
    {
        public string UserInput
        {
            get => GetProperty(() => UserInput);
            set => SetProperty(() => UserInput, value);
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

        public DelegateCommand LoginCommand { get; set; }

        public LoginViewModel()
        {
            LoginCommand = new DelegateCommand(Login, CanLogin);
        }

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(UserInput) &&
                   !string.IsNullOrWhiteSpace(Password);
        }

        private void Login()
        {
            try
            {
                // قراءة إعدادات الاتصال
                string json = File.ReadAllText("connection.json");
                var connSettings = JsonSerializer.Deserialize<ConnectionSettings>(json);

                string connectionString = $"Server={connSettings.ServerName};Database={connSettings.DatabaseName};User Id={connSettings.UserId};Password={connSettings.Password};TrustServerCertificate=True;";

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // ✅ التحقق من المستخدم مع Allow_Login = 1 (true)
                    string query = @"
                        SELECT ID, User_Name 
                        FROM Employees 
                        WHERE (ID = @UserInput OR User_Name = @UserInput) 
                        AND password = @Password 
                        AND Allow_Login = 1";  // BIT = true

                    using (var command = new SqlCommand(query, connection))
                    {
                        // ✅ محاولة تحويل UserInput لرقم
                        if (int.TryParse(UserInput, out int employeeId))
                        {
                            command.Parameters.AddWithValue("@UserInput", employeeId);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@UserInput", UserInput);
                        }

                        command.Parameters.AddWithValue("@Password", Password);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                string userName = reader.GetString(1);

                                StatusMessage = $"✅ مرحباً {userName}!";

                                // حفظ بيانات المستخدم الحالي
                                SaveCurrentUser(id, userName);

                                // فتح الشاشة الرئيسية
                                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                                {
                                    var mainWindow = new Views.Main.MainWindow();
                                    mainWindow.Show();

                                    // إغلاق شاشة الدخول
                                    foreach (Window window in System.Windows.Application.Current.Windows)
                                    {
                                        if (window is Views.Login.LoginView)
                                        {
                                            window.Close();
                                            break;
                                        }
                                    }
                                });
                            }
                            else
                            {
                                StatusMessage = "❌ بيانات الدخول غير صحيحة أو غير مسموح لك بالدخول";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ خطأ: {ex.Message}";
            }
        }

        private void SaveCurrentUser(int employeeId, string userName)
        {
            var currentUser = new CurrentUser
            {
                EmployeeId = employeeId,
                UserName = userName,
                LoginTime = DateTime.Now
            };

            string json = JsonSerializer.Serialize(currentUser);
            File.WriteAllText("currentuser.json", json);
        }
    }

    public class ConnectionSettings
    {
        public string ServerName { get; set; }
        public string DatabaseName { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
    }

    public class CurrentUser
    {
        public int EmployeeId { get; set; }
        public string UserName { get; set; }
        public DateTime LoginTime { get; set; }
    }
}