using App.ViewModels.AuthPageViewModels;

namespace App.Factories;

public interface IAuthViewModelFactory
{
    LoginViewModel CreateLogin(string? prefillEmail = null);
    RegisterViewModel CreateRegister();
    UserPageViewModel CreateUserPage();
}