using EverSneaks.MAUI.ViewModels;

namespace EverSneaks.MAUI.Views;

public partial class SneakersDetailsView : ContentPage
{
    private MyApplication evergineApplication;

    public SneakersDetailsView()
	{
		InitializeComponent();        
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        this.evergineApplication = new MyApplication();
        this.evergineView.Application = this.evergineApplication;
        this.BindingContext = new SneakersDetailsViewModel(this.evergineView);
    }

    private void OnColorTapped(object sender, TappedEventArgs args)
    {
        this.HideEveryCheckImage();
        var grid = (Grid)sender;
        var checkImage = (Image)grid.Children.Single(item => item is Image);
        checkImage.IsVisible = true;
    }

    private void HideEveryCheckImage()
    {
        this.selectedGrayImage.IsVisible = false;
        this.selectedBlueImage.IsVisible = false;
        this.selectedRedImage.IsVisible = false;
        this.selectedOrangeImage.IsVisible = false;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        this.evergineView.Handler.DisconnectHandler();
    }
}