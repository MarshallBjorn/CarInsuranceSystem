using Core.Entities;

namespace App.ViewModels.FirmPageViewModels;

public partial class FirmViewModel : ViewModelBase
{
    public Firm Firm { get; }

    public FirmViewModel(Firm firm)
    {
        Firm = firm;
    }
}