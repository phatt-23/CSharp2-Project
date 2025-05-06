using CoworkingDesktop.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CoworkingDesktop.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for UserFormView.xaml
    /// </summary>
    public partial class UserFormView : UserControl
    {
        public UserFormView()
        {
            InitializeComponent();
        }

        private void OnPasswordBoxChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is UserFormViewModel vm)
            {
                vm.Password = passwordBox.Password;
            }
        }
    }
}
