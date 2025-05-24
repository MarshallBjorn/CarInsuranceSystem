using App.Factories;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Entities;

namespace App.ViewModels;

public partial class AuthPageViewModel : ViewModelBase
{
    private readonly IAuthViewModelFactory _factory;

    [ObservableProperty] private ViewModelBase _currentAuthView = new();

    [ObservableProperty] private string _messageText = "";
    [ObservableProperty] private bool _popupIsOpen = false;

    public string email = "";

    public AuthPageViewModel(IAuthViewModelFactory factory)
    {
        _factory = factory;

        LoginViewCreator();
    }

    private bool switchBool = true;

    [RelayCommand]
    public void AuthSwitch()
    {
        switchBool ^= true;

        if (switchBool)
        {
            LoginViewCreator();
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
            registerVm.SwitchToLogin = AuthSwitch;
            CurrentAuthView = registerVm;
        }
    }

    [RelayCommand]
    public void PasswordChangeOpen(string email) => PopupIsOpen ^= true;

    private void LoginViewCreator()
    {
        var loginVm = _factory.CreateLogin();
        loginVm.OnLoggingInSuccess = () =>
        {
            UserPageViewCreator();
            // CurrentAuthView = _factory.CreateUserPage();
        };
        loginVm.SwitchToRegister = AuthSwitch;
        CurrentAuthView = loginVm;
    }

    private void UserPageViewCreator()
    {
        var userPageVm = _factory.CreateUserPage();
        userPageVm.OnChangePassword = PasswordChangeOpen;
        CurrentAuthView = userPageVm;
    }
}
