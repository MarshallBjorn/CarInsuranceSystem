using System;
using Core.Entities;

namespace App;

public class AppState
{
    public User? LoggedInUser { get; set; }

    public static IServiceProvider? ServiceProvider { get; set; }

    public event Action? OnLogin;

    public void RaiseLogin()
    {
        OnLogin?.Invoke();
    }
}
