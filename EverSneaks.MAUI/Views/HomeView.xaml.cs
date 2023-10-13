using EverSneaks.MAUI.ViewModels;

namespace EverSneaks.MAUI.Views;

public partial class HomeView : ContentPage
{
	public HomeView()
	{
		InitializeComponent();
		BindingContext = new HomeViewModel();
	}
}