using App.ViewModels;
using App.ViewModels.AuthPageViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace App.Views.AuthPageViews;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
    }

    private void OnTextClicked(object? sender, PointerPressedEventArgs e)
    {
        if (this.DataContext is LoginViewModel vm)
        {
            if (vm.SwitchToRegCommand.CanExecute(null))
            {
                vm.SwitchToRegCommand.Execute(null);
            }
        }
    }
}