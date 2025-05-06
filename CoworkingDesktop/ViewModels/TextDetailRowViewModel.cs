using CoworkingDesktop.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoworkingDesktop.ViewModels
{
    public class TextDetailRowViewModel : ViewModelBase
    {
        private string _label = string.Empty;
        public string Label
        {
            get => _label;
            set => Set(ref _label, value);
        } 

        private string _value = string.Empty;
        public string Value
        {
            get => _value;
            set => Set(ref _value, value);
        }

        public TextDetailRowViewModel()
        {
        }

        public TextDetailRowViewModel(string label, string value)
        {
            Label = label;
            Value = value;
        }
    }
}
