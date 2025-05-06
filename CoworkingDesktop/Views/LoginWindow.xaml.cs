using CoworkingDesktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CoworkingDesktop.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private IServiceProvider _services;

        public LoginWindow(LoginViewModel vm, IServiceProvider services)
        {
            InitializeComponent();
            DataContext = vm;
            _services = services;

            vm.LoginSucceeded += OnLoginSucceeded;
        }

        private void OnLoginSucceeded(object? sender, EventArgs e)
        {
            var mainWindow = _services.GetRequiredService<MainWindow>();
            mainWindow.Show();
            this.Close();
        }

        private void OnLoginClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
                vm.Password = passwordBox.Password;
        }
    }
}
