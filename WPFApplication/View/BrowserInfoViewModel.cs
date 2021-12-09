using System.Collections.Generic;
using System.ComponentModel;
using Core.Browser;
using Microsoft.Win32;
using WPFApplication.BrowserInfo;
using WPFApplication.Properties;
using WPFApplication.Command;

namespace WPFApplication.View
{
    public sealed class BrowserInfoViewModel : INotifyPropertyChanged
    {
        private List<NamespaceBrowserInfo> _namespaces;

        public List<NamespaceBrowserInfo> Namespaces
        {
            get => _namespaces;
            set
            {
                if (Equals(value, _namespaces)) return;
                _namespaces = value;
                OnPropertyChanged(nameof(Namespaces));
            }
        }

        private string _selectedFile;

        private string SelectedFile
        {
            get => _selectedFile;
            set
            {
                _selectedFile = value;
                var browser = new AssemblyBrowser(value);
                var ns = new List<NamespaceBrowserInfo>();
                browser.GetNamespaces().ForEach(n =>
                {
                    var nn = new NamespaceBrowserInfo
                    {
                        Name = n
                    };
                    browser.GetTypes(n).ForEach(t =>
                    {
                        var tt = new TypeBrowserInfo
                        {
                            Name = t
                        };
                        browser.GetMethods(n, t).ForEach(m => { tt.Signatures.Add(m); });
                        nn.Types.Add(tt);
                    });
                    ns.Add(nn);
                });
                Namespaces = ns;
            }
        }

        private RelayCommand _openCommand;

        public RelayCommand OpenCommand
        {
            get
            {
                return _openCommand ??
                       (_openCommand = new RelayCommand(obj =>
                       {
                           var d = new OpenFileDialog();
                           d.Multiselect = false;
                           d.Filter = "Assembly | *.dll";
                           if (d.ShowDialog() == true)
                           {
                               SelectedFile = d.FileName;
                           }
                       }));
            }
        }

        public BrowserInfoViewModel()
        {
            _namespaces = new List<NamespaceBrowserInfo>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}