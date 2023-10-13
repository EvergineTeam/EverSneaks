using EverSneaks.MAUI.ViewModels;

namespace EverSneaks.MAUI.Views;

public partial class GalleryView : ContentPage
{
	public GalleryView()
	{
		InitializeComponent();
		this.BindingContext = new GalleryViewModel();
	}
}