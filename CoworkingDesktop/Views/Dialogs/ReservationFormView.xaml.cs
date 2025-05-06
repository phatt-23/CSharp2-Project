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
    /// Interaction logic for ReservationFormView.xaml
    /// </summary>
    public partial class ReservationFormView : UserControl
    {
        public ReservationFormView(/* ReservationFormViewModel  vm */)
        {
            InitializeComponent();
            // View model is set when creating the window by callee
            //DataContext = vm;
        }
    }
}
