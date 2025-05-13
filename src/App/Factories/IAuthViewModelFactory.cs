using App.ViewModels;

namespace App.Factories;

public interface IAuthViewModelFactory
{
    LoginViewModel CreateLogin(string? prefillEmail = null);
    RegisterViewModel CreateRegister();
}