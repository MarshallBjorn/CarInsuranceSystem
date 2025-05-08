using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure;

namespace App.ViewModels;

public partial class AuthPageViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _currentAuthView = new LoginViewModel();

    private bool switchBool = true;

    [RelayCommand]
    private void AuthSwitch()
    {
        switchBool ^= true;

        if (!switchBool) CurrentAuthView = new RegisterViewModel();
        else CurrentAuthView = new LoginViewModel();
    }
}