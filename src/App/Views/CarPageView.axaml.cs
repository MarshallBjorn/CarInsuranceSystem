using System;
using System.Data;
using App.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace App.Views;

public partial class CarPageView : UserControl
{
    private Control mAddCarPopup;
    private Control mAddCarButton;
    private Control mMainGrid;

    public CarPageView()
    {
        InitializeComponent();

        mAddCarPopup = this.Find<Control>("AddCarPopup") ?? throw new Exception("Cannot find Add Car Popup by name.");
        mAddCarButton = this.Find<Control>("AddCarButton") ?? throw new Exception("Cannot find Add Car Button by name.");;
        mMainGrid = this.Find<Control>("MainGrid") ?? throw new Exception("Cannot find Main Grid by name.");;
        
        // LayoutUpdated += (_, __) => RepositionPopup();
    }

    private void RepositionPopup()
    {
        var relativePos = mAddCarButton.TranslatePoint(new Point(), mMainGrid);

        if (relativePos is { } p)
        {
            mAddCarPopup.Margin = new Thickness(0, p.Y, p.X, 0);
        }
    }

    private void InputElement_OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (DataContext is CarPageViewModel vm)
        {
            vm.CarEditIsOpen = false;
            vm.CarAddIsOpen = false;
        }
    }
}