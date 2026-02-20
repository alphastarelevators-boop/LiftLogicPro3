using System.Windows;
using LiftLogicPro.ViewModels.Login;

namespace LiftLogicPro.Views.Login
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();
        }
    }
}