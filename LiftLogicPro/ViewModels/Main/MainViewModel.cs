using DevExpress.Mvvm;
using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace LiftLogicPro.ViewModels.Main
{
    public class MainViewModel : ViewModelBase
    {
        // بيانات المستخدم الحالي
        public string CurrentUserName
        {
            get => GetProperty(() => CurrentUserName);
            set => SetProperty(() => CurrentUserName, value);
        }

        public string CurrentDateTime
        {
            get => GetProperty(() => CurrentDateTime);
            set => SetProperty(() => CurrentDateTime, value);
        }

        // القائمة الجانبية
        public bool IsNavExpanded
        {
            get => GetProperty(() => IsNavExpanded);
            set => SetProperty(() => IsNavExpanded, value);
        }

        // المحتوى الحالي
        public UserControl CurrentView
        {
            get => GetProperty(() => CurrentView);
            set => SetProperty(() => CurrentView, value);
        }

        // الأوامر
        public DelegateCommand ToggleNavCommand { get; set; }
        public DelegateCommand NavigateHomeCommand { get; set; }
        public DelegateCommand NavigateMaintenanceCommand { get; set; }
        public DelegateCommand NavigateInventoryCommand { get; set; }
        public DelegateCommand NavigateSalesCommand { get; set; }
        public DelegateCommand NavigateAccountingCommand { get; set; }
        public DelegateCommand NavigateSettingsCommand { get; set; }
        public DelegateCommand LogoutCommand { get; set; }

        public MainViewModel()
        {
            IsNavExpanded = true;
            LoadCurrentUser();
            StartClock();

            // تهيئة الأوامر
            ToggleNavCommand = new DelegateCommand(() => IsNavExpanded = !IsNavExpanded);
            NavigateHomeCommand = new DelegateCommand(() => NavigateTo("Home"));
            NavigateMaintenanceCommand = new DelegateCommand(() => NavigateTo("Maintenance"));
            NavigateInventoryCommand = new DelegateCommand(() => NavigateTo("Inventory"));
            NavigateSalesCommand = new DelegateCommand(() => NavigateTo("Sales"));
            NavigateAccountingCommand = new DelegateCommand(() => NavigateTo("Accounting"));
            NavigateSettingsCommand = new DelegateCommand(() => NavigateTo("Settings"));
            LogoutCommand = new DelegateCommand(Logout);
        }

        private void LoadCurrentUser()
        {
            try
            {
                if (File.Exists("currentuser.json"))
                {
                    string json = File.ReadAllText("currentuser.json");
                    var user = JsonSerializer.Deserialize<CurrentUser>(json);
                    CurrentUserName = user?.UserName ?? "مستخدم";
                }
            }
            catch
            {
                CurrentUserName = "مستخدم";
            }
        }

        private void StartClock()
        {
            CurrentDateTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm tt");

            // تحديث كل دقيقة
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(1);
            timer.Tick += (s, e) =>
            {
                CurrentDateTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm tt");
            };
            timer.Start();
        }

        private void NavigateTo(string page)
        {
            // لاحقاً سنضيف المحتوى الفعلي
            switch (page)
            {
                case "Home":
                    CurrentView = new UserControl(); // Dashboard
                    break;
                case "Maintenance":
                    CurrentView = new UserControl(); // صيانة
                    break;
                case "Inventory":
                    CurrentView = new UserControl(); // مخازن
                    break;
                case "Sales":
                    CurrentView = new UserControl(); // مبيعات
                    break;
                case "Accounting":
                    CurrentView = new UserControl(); // حسابات
                    break;
                case "Settings":
                    CurrentView = new UserControl(); // إعدادات
                    break;
            }
        }

        private void Logout()
        {
            // حذف بيانات المستخدم الحالي
            if (File.Exists("currentuser.json"))
            {
                File.Delete("currentuser.json");
            }

            // فتح شاشة الدخول
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                var loginView = new Views.Login.LoginView();
                loginView.Show();

                // إغلاق النافذة الرئيسية
                foreach (Window window in System.Windows.Application.Current.Windows)
                {
                    if (window is Views.Main.MainWindow)
                    {
                        window.Close();
                        break;
                    }
                }
            });
        }
    }

    public class CurrentUser
    {
        public int EmployeeId { get; set; }
        public string UserName { get; set; }
        public DateTime LoginTime { get; set; }
    }
}