using App.ViewModels.AuthPageViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace App.Views.AuthPageViews;

public partial class UserPageView : UserControl
{
    public UserPageView()
    {
        InitializeComponent();
    }

    private void OnTextClicked(object? sender, PointerPressedEventArgs e)
    {
        if (this.DataContext is UserPageViewModel vm)
        {
            if (vm.LogoutCommand.CanExecute(null))
            {
                vm.LogoutCommand.Execute(null);
            }
        }
    }
}