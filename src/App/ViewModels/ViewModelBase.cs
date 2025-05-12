using System;
using System.Net.Http;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace App.ViewModels;

public class ViewModelBase : ObservableObject
{
    protected IHttpClientFactory HttpClientFactory => AppState.ServiceProvider?.GetRequiredService<IHttpClientFactory>()
        ?? throw new InvalidOperationException("ServiceProvider not initialized.");

    protected AppState AppState => AppState.ServiceProvider?.GetRequiredService<AppState>();
}
