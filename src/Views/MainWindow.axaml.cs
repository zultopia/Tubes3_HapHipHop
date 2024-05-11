using Avalonia;
using Avalonia.Controls;
using Avalonia.Logging;
using Avalonia.ReactiveUI;
using HapHipHop.ViewModels;
using ReactiveUI;
using System;
using Avalonia.Markup.Xaml;

namespace HapHipHop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        //Logger.TryGet(LogEventLevel.Fatal, LogArea.Control)?.Log(this, "Avalonia Infrastructure");
        System.Diagnostics.Debug.WriteLine("System Diagnostics Debug");
        InitializeComponent();
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public void OnStartButtonClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        (DataContext as MainWindowViewModel)?.StartCommand.Execute();
    }
}