using System;
using System.Net.Http.Json;
using System.Threading.Tasks;
using App.Factories;
using App.Support;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.RequestModels;

namespace App.ViewModels;

public partial class AuthPageViewModel : ViewModelBase
{
    private readonly IAuthViewModelFactory _factory;

    [ObservableProperty] private ViewModelBase _currentAuthView = new();
    [ObservableProperty] private string _messageText = "";
    [ObservableProperty] private bool _popupIsOpen = false;

    [ObservableProperty] private string _popupText = "";
    [ObservableProperty] private string _currentPassword = "";
    [ObservableProperty] private string _newPassword = "";
    [ObservableProperty] private string _confirmPassword = "";

    private string emailForPasswordChange = "";

    public AuthPageViewModel(IAuthViewModelFactory factory)
    {
        _factory = factory;
        LoginViewCreator(); // Start with login
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
                MessageText = "Registration successful. Please log in.";
                switchBool = true;
                LoginViewCreator(newEmail);
            };
            registerVm.SwitchToLogin = AuthSwitch;
            CurrentAuthView = registerVm;
        }
    }

    [RelayCommand]
    public void PasswordChangeOpen(string email)
    {
        PopupIsOpen ^= true;
        emailForPasswordChange = email;
    }

    [RelayCommand]
    public async Task ConfirmChange()
    {
        var client = HttpClientFactory.CreateClient("CarInsuranceApi");
        PopupText = "";

        try
        {
            var request = new ChangePasswordRequest
            {
                Email = emailForPasswordChange,
                CurrentPassword = CurrentPassword,
                NewPassword = NewPassword,
                ConfirmNewPassword = ConfirmPassword
            };

            var response = await client.PutAsJsonAsync("api/User/change-password", request);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                PopupText = errorContent;
            }
            else
            {
                PopupText = "Password successfully changed";
                await Task.Delay(2000);
                PopupIsOpen = false;
            }
        }
        catch (Exception ex)
        {
            PopupText = ex.Message;
        }
    }

    private void Logout()
    {
        TokenStorage.Token = string.Empty;
        AppState.LoggedInUser = null;
        AppState.RaiseLogout();
        LoginViewCreator();
    }

    private void LoginViewCreator(string? email = null)
    {
        var loginVm = email is not null ? _factory.CreateLogin(email) : _factory.CreateLogin();
        loginVm.OnLoggingInSuccess = UserPageViewCreator;
        loginVm.SwitchToRegister = AuthSwitch;
        CurrentAuthView = loginVm;
    }

    private void UserPageViewCreator()
    {
        var userPageVm = _factory.CreateUserPage();
        userPageVm.OnChangePassword = PasswordChangeOpen;
        userPageVm.OnLogoutClick = Logout;
        CurrentAuthView = userPageVm;
    }
}
