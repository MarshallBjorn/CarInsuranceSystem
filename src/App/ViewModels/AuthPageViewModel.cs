using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure;

namespace App.ViewModels;

public partial class AuthPageViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _currentAuthView;
    [ObservableProperty]
    private string _messageText = "";
    public string email = "";

    public AuthPageViewModel() => _currentAuthView = new LoginViewModel(this);

    private bool switchBool = true;

    public void Switch()
    {
        switchBool ^= true;

        if (!switchBool) CurrentAuthView = new RegisterViewModel(this);
        else
        { 
            var loginViewModel = new LoginViewModel(this);

            if (!String.IsNullOrEmpty(email))
                loginViewModel.Email = email;
            CurrentAuthView = loginViewModel;
        }
    }

    [RelayCommand]
    private void AuthSwitch()
    {
        Switch();
    }

}