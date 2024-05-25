using ReactiveUI;
using System;
using System.Reactive;
using System.Windows.Input;
using HapHipHop.Views;

namespace HapHipHop.ViewModels
{
    public class BioPrintViewModel : ViewModelBase
    {
        public ReactiveCommand<Unit, Unit> BioPrintCommand { get; }
        public ReactiveCommand<Unit, Unit> PilihCitraCommand { get; }
        public ReactiveCommand<Unit, Unit> SearchCommand { get; }
        
        private bool _isBMChecked;
        private bool _isKMPChecked;

        public bool IsBMChecked
        {
            get => _isBMChecked;
            set => this.RaiseAndSetIfChanged(ref _isBMChecked, value);
        }

        public bool IsKMPChecked
        {
            get => _isKMPChecked;
            set => this.RaiseAndSetIfChanged(ref _isKMPChecked, value);
        }

        public BioPrintViewModel()
        {
            BioPrintCommand = ReactiveCommand.Create(OnBioPrintCommandExecute);
            PilihCitraCommand = ReactiveCommand.Create(OnPilihCitraCommandExecute);
            SearchCommand = ReactiveCommand.Create(OnSearchCommandExecute);

            // Perubahan pada toggle switch dan tombol Pilih Citra
            this.WhenAnyValue(x => x.IsBMChecked)
                .Subscribe(isChecked =>
                {
                    // Toggle switch BM 
                });

            this.WhenAnyValue(x => x.IsKMPChecked)
                .Subscribe(isChecked =>
                {
                    // Toggle switch KMP 
                });
        }

        private void OnBioPrintCommandExecute()
        {
            // Implementasi logika navigasi kembali ke MainWindow
            var mainWindowViewModel = new MainWindowViewModel();
            var mainWindow = new MainWindow { DataContext = mainWindowViewModel };
            mainWindow.Show();
        }

        private void OnPilihCitraCommandExecute()
        {
            // Implementasi logika pilih citra
        }

        private void OnSearchCommandExecute()
        {
            // Implementasi logika search
        }
    }
}