using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CoworkingDesktop.Helpers
{
    public static class HttpResponseHelper
    {
        public static async Task<bool> InfoOnBadStatusCode(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return true;

            var message = await response.Content.ReadAsStringAsync();
            MessageBox.Show(message, "Server Response Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }
}
