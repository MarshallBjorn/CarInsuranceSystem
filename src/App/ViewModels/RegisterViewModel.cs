using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace App.ViewModels;

public partial class RegisterViewModel : ViewModelBase
{
    [ObservableProperty] private string _email = "";
    [ObservableProperty] private string _firstname = "";
    [ObservableProperty] private string _lastname = "";
    [ObservableProperty] private DateTimeOffset _birthDate = new DateTimeOffset(DateTime.Today);
    [ObservableProperty] private string _password1 = "";
    [ObservableProperty] private string _password2 = "";
    [ObservableProperty] private string _messageText = "";


}