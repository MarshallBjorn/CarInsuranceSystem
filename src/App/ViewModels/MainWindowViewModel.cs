using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using App.Views;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace App.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _isPaneOpen = true;

    [ObservableProperty]
    private ViewModelBase _currentPage = new HomePageViewModel();

    [ObservableProperty]
    private ListItemTemplate? _selectedListItem;

    [ObservableProperty] private string _credentialFirstName = string.Empty;
    [ObservableProperty] private string _credentialLastName = string.Empty;

    public MainWindowViewModel()
    {
        AppState.OnLogin += () => LoadCredentials();
        AppState.OnLogOut += () =>
        {
            CredentialFirstName = "";
            CredentialLastName = "";
        };
    }

    partial void OnSelectedListItemChanged(ListItemTemplate? value)
    {
        if (value is null) return;
        var instance = AppState.ServiceProvider?.GetRequiredService(value.ModelType);
        if (instance is null) return;
        CurrentPage = (ViewModelBase)instance;
    }

    public ObservableCollection<ListItemTemplate> Items { get; } = new()
    {
        new ListItemTemplate(typeof(HomePageViewModel), "HomeRegular"),
        new ListItemTemplate(typeof(CarPageViewModel), "Vehicle"),
        new ListItemTemplate(typeof(AuthPageViewModel), "PersonAccount"),
        new ListItemTemplate(typeof(FirmPageViewModel), "Globe")
    };

    [RelayCommand]
    private void TriggerPane()
    {
        IsPaneOpen = !IsPaneOpen;
    }

    private void LoadCredentials()
    {
        var user = AppState.LoggedInUser ?? throw new ArgumentNullException(nameof(LoadCredentials));
        CredentialFirstName = $"{user.FirstName}";
        CredentialLastName = $"{user.LastName}";
    }
}

public class ListItemTemplate
{
    public ListItemTemplate(Type type, string iconKey)
    {
        ModelType = type;
        Label = type.Name.Replace("PageViewModel", "");
        Application.Current!.TryFindResource(iconKey, out var res);
        ListItemIcon = (StreamGeometry)res!;
    }

    public string Label { get; }
    public Type ModelType { get; }
    public StreamGeometry ListItemIcon { get; }
}