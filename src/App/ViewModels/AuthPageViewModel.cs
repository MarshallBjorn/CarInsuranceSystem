using System;
using System.Threading.Tasks;
using App.Factories;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure;

namespace App.ViewModels;

public partial class AuthPageViewModel : ViewModelBase
{
    private readonly IAuthViewModelFactory _factory;

    [ObservableProperty]
    private ViewModelBase _currentAuthView;
    [ObservableProperty]
    private string _messageText = "";
    public string email = "";

    public AuthPageViewModel(IAuthViewModelFactory factory)
    {
        _factory = factory;
        CurrentAuthView = _factory.CreateLogin();
    }

    private bool switchBool = true;

    [RelayCommand]
    public void AuthSwitch()
    {
        switchBool ^= true;

        if (switchBool) 
            CurrentAuthView = _factory.CreateLogin(email);
        else
        { 
            CurrentAuthView = _factory.CreateRegister();
        }
    }
}