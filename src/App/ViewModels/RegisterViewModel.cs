using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Entities;
using Core.RequestModels;
using Core.Validators;
using Tmds.DBus.Protocol;

namespace App.ViewModels;

public partial class RegisterViewModel : ViewModelBase
{
    public Action<string>? OnRegistrationSuccess { get; set; }

    [ObservableProperty] private string _email = "";
    [ObservableProperty] private string _firstname = "";
    [ObservableProperty] private string _lastname = "";
    [ObservableProperty] private DateTimeOffset _birthDate = new DateTimeOffset(DateTime.Today);
    [ObservableProperty] private string _password1 = "";
    [ObservableProperty] private string _password2 = "";
    [ObservableProperty] private string _messageText = "";
    
    private readonly List<string> _emailErrorList = new();
    private readonly List<string> _firstNameErrorList = new();
    private readonly List<string> _lastNameErrorList = new();
    private readonly List<string> _dateErrorList = new();
    private readonly List<string> _passwordErrorList1 = new();
    private readonly List<string> _passwordErrorList2 = new();

    public string EmailErrors => _emailErrorList.Count > 0 
        ? string.Join("\n", _emailErrorList) 
        : string.Empty;

    public string FirstNameErrors => _firstNameErrorList.Count > 0 
        ? string.Join("\n", _firstNameErrorList) 
        : string.Empty;
    
    public string LastNameErrors => _lastNameErrorList.Count > 0 
        ? string.Join("\n", _lastNameErrorList) 
        : string.Empty;

    public string DateErrors => _dateErrorList.Count > 0 
        ? string.Join("\n", _dateErrorList) 
        : string.Empty;

    public string PasswordErrors1 => _passwordErrorList1.Count > 0 
        ? string.Join("\n", _passwordErrorList1) 
        : string.Empty;
    
    public string PasswordErrors2 => _passwordErrorList2.Count > 0 
        ? string.Join("\n", _passwordErrorList2) 
        : string.Empty;

    [RelayCommand]
    private async Task Register()
    {
        MessageText = "";
        _firstNameErrorList.Clear();
        OnPropertyChanged(nameof(FirstNameErrors));
        _emailErrorList.Clear();
        OnPropertyChanged(nameof(EmailErrors));
        _lastNameErrorList.Clear();
        OnPropertyChanged(nameof(LastNameErrors));
        _dateErrorList.Clear();
        OnPropertyChanged(nameof(DateErrors));
        _passwordErrorList1.Clear();
        OnPropertyChanged(nameof(PasswordErrors1));
        _passwordErrorList2.Clear();
        OnPropertyChanged(nameof(PasswordErrors2));

        var UserValidator = new UserValidator();
        var PasswordValidator = new PasswordValidator();
        
        try {
            User user = new()
            {
                Email = Email,
                FirstName = Firstname,
                LastName = Lastname,
                BirthDate = BirthDate.DateTime,
            };

            var client = HttpClientFactory.CreateClient("CarInsuranceApi");
            var userInfo = UserValidator.Validate(user);
            var passwordChecked1 = PasswordValidator.Validate(Password1);
            var passwordChecked2 = PasswordValidator.Validate(Password2);

            if (userInfo.IsValid && passwordChecked1.IsValid && passwordChecked2.IsValid)
            {
                BirthDate = BirthDate.ToUniversalTime();
                var request = new RegisterRequest
                {
                    Email = Email,
                    FirstName = Firstname,
                    LastName = Lastname,
                    BirthDate = BirthDate.UtcDateTime,
                    Password1 = Password1,
                    Password2 = Password2
                };

                var response = await client.PostAsJsonAsync($"api/User/register", request);
                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    MessageText = string.Format(json);
                    return;
                }

                MessageText = "User registered succesfuly.";
                
                OnRegistrationSuccess?.Invoke(Email);
            }
            else
            {
                foreach (var error in userInfo.Errors)
                {
                    switch (error.PropertyName)
                    {
                        case "Email":
                            _emailErrorList.Add(error.ErrorMessage);
                            break;
                        case "FirstName":
                            _firstNameErrorList.Add(error.ErrorMessage);
                            break;
                        case "LastName":
                            _lastNameErrorList.Add(error.ErrorMessage);
                            break;
                        case "BirthDate":
                            _dateErrorList.Add(error.ErrorMessage);
                            break;
                    }
                }
                foreach (var error in passwordChecked1.Errors)
                {
                    _passwordErrorList1.Add(error.ErrorMessage);
                }
                foreach (var error in passwordChecked2.Errors)
                {
                    _passwordErrorList2.Add(error.ErrorMessage);
                }

                OnPropertyChanged(nameof(EmailErrors));
                OnPropertyChanged(nameof(FirstNameErrors));
                OnPropertyChanged(nameof(LastNameErrors));
                OnPropertyChanged(nameof(DateErrors));
                OnPropertyChanged(nameof(PasswordErrors1));
                OnPropertyChanged(nameof(PasswordErrors2));
            }
        } catch (Exception ex) {
            MessageText = ex.Message;
        }
    }
}