using System;
using App.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace App.Views;

public partial class AuthPageView : UserControl
{
    public AuthPageView()
    {
        InitializeComponent();
    }

    private void InputElement_OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (DataContext is AuthPageViewModel vm)
        {
            vm.PopupIsOpen = false;
        }
    }
}