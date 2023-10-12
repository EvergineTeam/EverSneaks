using EverSneaks.MAUI.ViewModels;

namespace EverSneaks.MAUI.Views;

public partial class SneakersDetailsView : ContentPage
{
    private readonly MyApplication evergineApplication;

    public SneakersDetailsView()
	{
		InitializeComponent();
        this.evergineApplication = new MyApplication();
        this.evergineView.Application = this.evergineApplication;
        this.BindingContext = new SneakersDetailsViewModel(this.evergineView);
    }
}