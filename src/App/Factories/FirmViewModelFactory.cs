using System;
using System.Net.Http;
using App.ViewModels;
using App.ViewModels.FirmPageViewModels;
using App.Views.FirmPageViews;
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
        var firmPageVm = AppState.ServiceProvider?.GetRequiredService<FirmPageViewModel>()
            ?? throw new ArgumentNullException(nameof(FirmPageViewModel));
        var clientFactory = AppState.ServiceProvider?.GetRequiredService<IHttpClientFactory>()
            ?? throw new ArgumentNullException(nameof(IHttpClientFactory));

        return new FirmViewModel(firm, firmPageVm, clientFactory);
    }

    public InsuranceTypeViewModel CreateInsuranceEdit(InsuranceType insuranceType, FirmViewModel firm)
    {
        var firmPageVm = AppState.ServiceProvider?.GetRequiredService<FirmPageViewModel>()
            ?? throw new ArgumentNullException(nameof(FirmPageViewModel));
       
        var clientFactory = AppState.ServiceProvider?.GetRequiredService<IHttpClientFactory>()
            ?? throw new ArgumentNullException(nameof(IHttpClientFactory));

        return new InsuranceTypeViewModel(insuranceType, firm, firmPageVm, clientFactory);
    }
}