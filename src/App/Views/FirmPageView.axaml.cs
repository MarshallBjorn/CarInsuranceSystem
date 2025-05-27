using App.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace App.Views;

public partial class FirmPageView : UserControl
{
    public FirmPageView()
    {
        InitializeComponent();
    }

    private void InputElement_OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (DataContext is FirmPageViewModel vm)
        {
            vm.IsAnyPopupOpen = false;
        }
    }
}