using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure;

namespace App.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly AuthPageViewModel _parentViewModel;

    public LoginViewModel(AuthPageViewModel parentViewModel) => _parentViewModel = parentViewModel;

    [ObservableProperty]
    public string _email = "";
    [ObservableProperty]
    private string _password = "";
    [ObservableProperty]
    private string _messageText = "";

    [RelayCommand]
    private async Task LogIn()
    {
        try {
            var currentUser = await ServiceLocator.UserService.LoginAsync(Email, Password);

            if (currentUser is not null)
            {
                ServiceLocator.AppState.LoggedInUser = currentUser;
                MessageText = $"Auth success. Welcome {currentUser.FirstName} {currentUser.LastName}";
            } else {
                MessageText = "Auth failed";
            }
        } catch (Exception ex)
        {
            MessageText = ex.Message;
        }
        
    }
}