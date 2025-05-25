using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Entities;

namespace App.ViewModels.CarPageViewModels;

public partial class InsuranceViewModel : ViewModelBase
{
    public Insurance ThisInsurance { get; }

    [ObservableProperty]
    public string? _displayName;

    public InsuranceViewModel(Insurance insurance)
    {
        ThisInsurance = insurance;
        DisplayNameCreator();
    }

    public void DisplayNameCreator()
    {
        var firm = ThisInsurance.Firm ?? throw new Exception("No firm asigned");

        DisplayName = $"{firm.Name} {ThisInsurance.Type}";
    }
}