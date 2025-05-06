using CoworkingDesktop.ViewModels;
using CoworkingDesktop.ViewModels.Base;
using CoworkingDesktop.ViewModels.Features;
using CoworkingDesktop.Views;
using CoworkingDesktop.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CoworkingDesktop.Services
{
    public class DialogService : IDialogService
    {
        private readonly IServiceProvider _services;
        private readonly Dictionary<Type, Type> _viewMap = [];

        public DialogService(IServiceProvider services)
        {
            _services = services;
            // Register all view models and their corresponding views
            _viewMap[typeof(ReservationFormViewModel)] = typeof(ReservationFormView);
            _viewMap[typeof(ReservationDetailViewModel)] = typeof(ReservationDetailView);
            _viewMap[typeof(WorkspaceDetailViewModel)] = typeof(WorkspaceDetailView);
            _viewMap[typeof(WorkspaceFormViewModel)] = typeof(WorkspaceFormView);
            _viewMap[typeof(CoworkingCenterFormViewModel)] = typeof(CoworkingCenterFormView);
            _viewMap[typeof(UserFormViewModel)] = typeof(UserFormView);
            _viewMap[typeof(UserDetailViewModel)] = typeof(UserDetailView);
        }

        public bool? ShowDialog(object viewModel)
        {
            if (!_viewMap.TryGetValue(viewModel.GetType(), out var viewType))
                throw new InvalidOperationException($"No view registered for {viewModel.GetType().Name}!");

            var view = (FrameworkElement)ActivatorUtilities.CreateInstance(_services, viewType);

            try
            {
                var window = new DialogHostWindow
                {
                    Content = view,
                    DataContext = viewModel,
                };

                if (viewModel is IClosableDialog vmWithClose)
                {
                    vmWithClose.RequestClose += (_, result) =>
                    {
                        window.DialogResult = result;
                        window.Close();
                    };
                }

                return window.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return false;
        }
    }
}
