using System;
using App.ViewModels.FirmPageViewModels;
using Core.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace App.Factories;

public class FirmViewModelFactory : IFirmViewModelFactory
{
    public NewFirmViewModel CreateAdd()
    {
        var vm = AppState.ServiceProvider?.GetRequiredService<NewFirmViewModel>() ?? throw new ArgumentNullException();
        return vm;
    }

    public FirmViewModel CreateEdit(Firm firm)
    {
        var vm = new FirmViewModel(firm);
        return vm;
    }
}