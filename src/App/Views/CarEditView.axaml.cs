using App.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace App.Views;

public partial class CarEditView : Window
{
    public CarEditView()
    {
        InitializeComponent();
        DataContext = new CarPageViewModel();
    }
}