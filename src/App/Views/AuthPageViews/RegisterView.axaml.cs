using App.ViewModels.AuthPageViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace App.Views.AuthPageViews;

public partial class RegisterView : UserControl
{
    public RegisterView()
    {
        InitializeComponent();
    }

    private void OnTextClicked(object? sender, PointerPressedEventArgs e)
    {
        if (this.DataContext is RegisterViewModel vm)
        {
            if (vm.SwitchToLogCommand.CanExecute(null))
            {
                vm.SwitchToLogCommand.Execute(null);
            }
        }
    }
}