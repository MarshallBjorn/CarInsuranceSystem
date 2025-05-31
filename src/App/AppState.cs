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
    public event Action? OnCarChange;
    public event Action? OnFirmChange;

    public void RaiseLogin()
    {
        OnLogin?.Invoke();
    }

    public void RaiseLogout()
    {
        OnLogOut?.Invoke();
    }

    public void RaiseInsurance() => OnInsuranceChange?.Invoke();

    public void RaiseCar() => OnCarChange?.Invoke();

    public void RaiseFirm() => OnFirmChange?.Invoke();
}
