using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Entities;

namespace App.ViewModels.AuthPageViewModels
{
    public partial class UserPageViewModel : ViewModelBase
    {
        public Action<string>? OnChangePassword { get; set; }

        [ObservableProperty] private User _user;
        [ObservableProperty] private string _messageText = "";
        [ObservableProperty] private string _welcomeMessage;

        private bool _isReadOnly = true;
        private readonly HttpClient _client;

        public UserPageViewModel()
        {
            // Ensure AppState.LoggedInUser is handled if it can be null
            _user = AppState.LoggedInUser ?? throw new ArgumentNullException("XD"); // Or throw, depending on desired behavior
            WelcomeMessage = $"Welcome {User.FirstName}";

            _client = HttpClientFactory.CreateClient("CarInsuranceApi");
        }

        public bool IsReadOnly
        {
            get => _isReadOnly;
            set
            {
                // No longer need to notify BorderBrush change from here
                // if styles are handling it based on TextBox.IsReadOnly
                if (SetProperty(ref _isReadOnly, value))
                {
                    OnPropertyChanged(nameof(EditButtonText));
                }
            }
        }

        public string EditButtonText => IsReadOnly ? "Edit" : "Save";

        // This property is no longer needed if styles control the TextBox BorderBrush
        // public IBrush BorderBrush => IsReadOnly ? Brushes.Transparent : Brushes.Gray;

        [RelayCommand]
        public async Task ToggleEdit()
        {
            if (IsReadOnly)
            {
                IsReadOnly = false;
            }
            else
            {
                try
                {
                    var response = await _client.PutAsJsonAsync("api/User/update", User);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageText = "User updated successfully.";
                        AppState.LoggedInUser = User;
                    }
                    else
                    {
                        MessageText = $"Failed to update user: {response.ReasonPhrase}";
                    }
                }
                catch (Exception ex)
                {
                    MessageText = $"Error: {ex.Message}";
                }

                await Task.Delay(3000);
                MessageText = string.Empty;
                IsReadOnly = true;
            }
        }

        [RelayCommand]
        private void ChangePassword()
        {
            OnChangePassword?.Invoke(User.Email);
        }
    }
}