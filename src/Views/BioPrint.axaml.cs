using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using HapHipHop.ViewModels;

namespace HapHipHop.Views
{
    public partial class BioPrint : Window
    {
        public BioPrint()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}