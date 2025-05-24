using System;
using App.ViewModels;
using Avalonia.Controls;
using Avalonia.Input;

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
        mAddCarButton = this.Find<Control>("AddCarButton") ?? throw new Exception("Cannot find Add Car Button by name.");
        mMainGrid = this.Find<Control>("MainGrid") ?? throw new Exception("Cannot find Main Grid by name.");
    }

    private void InputElement_OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (DataContext is CarPageViewModel vm)
        {
            vm.CarEditIsOpen = false;
            vm.CarAddIsOpen = false;
            vm.ErrorText = "";
        }
    }
}