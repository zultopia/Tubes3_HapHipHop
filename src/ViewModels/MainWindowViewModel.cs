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
            var bioPrintViewModel = new BioPrintViewModel();
            bioPrintPage = new BioPrint { DataContext = bioPrintViewModel }; 
            (bioPrintPage.DataContext as BioPrintViewModel)?.BioPrintCommand.Subscribe(_ =>
            {
                (bioPrintPage as IDisposable)?.Dispose();
                bioPrintPage.Close();
                bioPrintPage = null;
            });
            bioPrintPage.Show();
        }

        private void OnBioPrintCommandExecute()
        {
            if (bioPrintPage != null)
            {
                bioPrintPage.Close();
                bioPrintPage = null;
                var mainWindow = new MainWindow { DataContext = new MainWindowViewModel() };
                mainWindow.Show();
            }
            else
            {
                // Kasih handle kalo bioPrintPagenya bernilai null
            }
        }
    }
}