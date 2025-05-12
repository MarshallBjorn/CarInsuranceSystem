using System;
using App.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;

namespace App.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = AppState.ServiceProvider?.GetRequiredService<MainWindowViewModel>();
    }
}