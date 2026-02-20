using LiftLogicPro.ViewModels.Connection;
using System.Windows;

namespace LiftLogicPro.Views.Connection
{
    public partial class ConnectionView : Window
    {
        public ConnectionView()
        {
            InitializeComponent();
            DataContext = new ConnectionViewModel();
        }
    }
}