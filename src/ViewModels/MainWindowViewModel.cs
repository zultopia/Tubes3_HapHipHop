using ReactiveUI;
using System;
using System.Reactive;
using System.Windows.Input;
using HapHipHop.Views;

namespace HapHipHop.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Welcome Azul!";

        public ReactiveCommand<Unit, Unit> StartCommand { get; }
        public ReactiveCommand<Unit, Unit> BioPrintCommand { get; }
        private BioPrint? bioPrintPage; 

        public MainWindowViewModel()
        {
            StartCommand = ReactiveCommand.Create(OnStartCommandExecute);
            BioPrintCommand = ReactiveCommand.Create(OnBioPrintCommandExecute);
        }

        private void OnStartCommandExecute()
        {
            // Implement logic to navigate or perform actions on Start button click
            var bioPrintViewModel = new BioPrintViewModel();
            bioPrintPage = new BioPrint { DataContext = bioPrintViewModel }; 
            bioPrintPage.Show();
        }

        private void OnBioPrintCommandExecute()
        {
            if (bioPrintPage != null)
            {
                var mainWindow = new MainWindow { DataContext = new MainWindowViewModel() };
                mainWindow.Show();
                (bioPrintPage as IDisposable)?.Dispose();
                bioPrintPage.Close();
                bioPrintPage = null; // Disetel ke ke null lagi abis digunain
            }
            else
            {
                // Kasih handle kalo bioPrintPagenya bernilai null
            }
        }
    }
}