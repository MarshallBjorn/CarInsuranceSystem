using System;
using App.ViewModels.AuthPageViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace App.Factories;

public class AuthModelFactory : IAuthViewModelFactory
{
    public LoginViewModel CreateLogin(string? prefillEmail = null)
    {
        var vm = AppState.ServiceProvider?.GetRequiredService<LoginViewModel>() ?? throw new ArgumentNullException();
        if (!string.IsNullOrWhiteSpace(prefillEmail))
            vm.Email = prefillEmail;
        return vm;
    }

    public RegisterViewModel CreateRegister()
    {
        return AppState.ServiceProvider?.GetRequiredService<RegisterViewModel>() ?? throw new ArgumentNullException();
    }

    public UserPageViewModel CreateUserPage()
    {
        return AppState.ServiceProvider?.GetRequiredService<UserPageViewModel>() ?? throw new ArgumentNullException();
    }
}