using App.ViewModels.FirmPageViewModels;
using Core.Entities;

namespace App.Factories;

public interface IFirmViewModelFactory
{
    NewFirmViewModel CreateAdd();
    FirmViewModel CreateEdit(Firm firm);
    InsuranceTypeViewModel CreateInsuranceEdit(InsuranceType insuranceType, FirmViewModel firm);
}