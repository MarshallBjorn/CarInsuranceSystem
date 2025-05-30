using System;
using Core.Entities;

namespace App;

public class AppState
{
    public User? LoggedInUser { get; set; }

    public static IServiceProvider? ServiceProvider { get; set; }

    public event Action? OnLogin;
    public event Action? OnLogOut;
    public event Action? OnInsuranceChange;

    public void RaiseLogin()
    {
        OnLogin?.Invoke();
    }

    public void RaiseLogout()
    {
        OnLogOut?.Invoke();
    }

    public void RaiseInsurance() => OnInsuranceChange?.Invoke();
}
