using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace App.ViewModels.CarPageViewModels;

public partial class InsuranceViewModel : ViewModelBase
{
    public InsuranceType ThisInsurance { get; }

    [ObservableProperty]
    public string? _displayName;

    public InsuranceViewModel(InsuranceType insuranceType)
    {
        ThisInsurance = insuranceType;
        DisplayNameCreator();
    }

    public void DisplayNameCreator()
    {
        var firm = ThisInsurance.FirmInsuranceDto ?? throw new Exception("No firm asigned");

        DisplayName = $"{firm.Name} {ThisInsurance.Firm}";
    }
}