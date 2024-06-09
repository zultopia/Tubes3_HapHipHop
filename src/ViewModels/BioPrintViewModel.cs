using ReactiveUI;
using System;
using System.IO;
using System.Reactive;
using System.Windows.Input;
using HapHipHop.Views;
using HapHipHop.Models;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HapHipHop.ViewModels
{
    public class BioPrintViewModel : ViewModelBase
    {
        public ReactiveCommand<Unit, Unit> BioPrintCommand { get; }
        public ReactiveCommand<Unit, Unit> PilihCitraCommand { get; }
        public ReactiveCommand<Unit, Unit> SearchCommand { get; }

        private bool _isBMChecked;
        private bool _isKMPChecked;
        private Bitmap? _selectedImage;
        private Bitmap? _searchResultImage;
        private string? _selectedImagePath;
        private string? _searchResultBioData;
        private string? _searchTime;
        private string? _matchPercentage;

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

        public Bitmap? SelectedImage
        {
            get => _selectedImage;
            set => this.RaiseAndSetIfChanged(ref _selectedImage, value);
        }

        public Bitmap? SearchResultImage
        {
            get => _searchResultImage;
            set => this.RaiseAndSetIfChanged(ref _searchResultImage, value);
        }

        public string? SelectedImagePath
        {
            get => _selectedImagePath;
            set => this.RaiseAndSetIfChanged(ref _selectedImagePath, value);
        }

        public string? SearchResultBioData
        {
            get => _searchResultBioData;
            set => this.RaiseAndSetIfChanged(ref _searchResultBioData, value);
        }

        public string? SearchTime
        {
            get => _searchTime;
            set => this.RaiseAndSetIfChanged(ref _searchTime, value);
        }

        public string? MatchPercentage
        {
            get => _matchPercentage;
            set => this.RaiseAndSetIfChanged(ref _matchPercentage, value);
        }

        public BioPrintViewModel()
        {
            BioPrintCommand = ReactiveCommand.Create(OnBioPrintCommandExecute);
            PilihCitraCommand = ReactiveCommand.CreateFromTask(OnPilihCitraCommandExecute);
            SearchCommand = ReactiveCommand.CreateFromTask(OnSearchCommandExecuteAsync);
        }

        private void OnBioPrintCommandExecute()
        {
            var mainWindowViewModel = new MainWindowViewModel();
            var mainWindow = new MainWindow { DataContext = mainWindowViewModel };
            mainWindow.Show();
        }

        private async Task OnPilihCitraCommandExecute()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Title = "Pilih Citra",
                Filters = new List<FileDialogFilter>()
                {
                    new FileDialogFilter() { Name = "Images", Extensions = { "png", "jpg", "jpeg", "bmp" } }
                }
            };

            var window = new Window();

            var result = await openFileDialog.ShowAsync(window);

            if (result != null && result.Length > 0)
            {
                var filePath = result[0];
                SelectedImagePath = filePath;
                // File.AppendAllText("log.txt", filePath + "\n");
                using (var stream = File.OpenRead(filePath))
                {
                    SelectedImage = await Task.Run(() => Bitmap.DecodeToWidth(stream, 300));
                }
            }
        }

        private async Task OnSearchCommandExecuteAsync()
        {
            if (SelectedImage == null)
            {
                SearchResultBioData = "Please select an image first.";
                return;
            }

            SearchResultBioData = "Sedang mencari data yang cocok...";

            Bitmap SelectedImageBitmap = new Bitmap(SelectedImagePath);
            var result = Processing.ProcessFingerprintMatching(SelectedImageBitmap, IsBMChecked);

            SearchResultBioData = result.bestPath;

            if (!string.IsNullOrEmpty(result.bestPath))
            {
                SearchResultImage = new Bitmap(result.bestPath);
                SearchResultBioData = result.biodata.ToString();
                SearchTime = $"Waktu Eksekusi: {result.time} ms";
                MatchPercentage = $"Persentase Kecocokan: {result.percentage:F2}%";
            }
            else
            {
                SearchResultImage = null;
                SearchResultBioData = "❌ Tidak ada data yang cocok ❌";
                SearchTime = "Waktu Eksekusi: 0 ms";
                MatchPercentage = "Persentase Kecocokan: 0 %";
            }
        }
    }
}