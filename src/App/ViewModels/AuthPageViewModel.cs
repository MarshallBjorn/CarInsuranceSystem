using System;
using System.Threading.Tasks;
using App.Factories;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
        {
            CurrentAuthView = _factory.CreateLogin(email);
            MessageText = "";
        }
        else
        {
            var registerVm = _factory.CreateRegister();
            registerVm.OnRegistrationSuccess = (string newEmail) =>
            {
                email = newEmail;
                MessageText = "Registration successful. Please log in.";
                switchBool = true;
                CurrentAuthView = _factory.CreateLogin(email);
            };

            CurrentAuthView = registerVm;
        }
    }
}