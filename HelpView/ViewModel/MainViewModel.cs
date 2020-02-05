using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HelpView.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public bool IsShowHelp { get; set; } = false;

        public MainViewModel()
        {
            Task.Factory.StartNew(Init);
        }

        private async void Init()
        {
            await Task.Delay(1000);

            System.Windows.Application.Current?.Dispatcher.Invoke(() => 
            {
                IsShowHelp = true;
                OnPropertyChanged(nameof(IsShowHelp));
            });
        }
    }
}
