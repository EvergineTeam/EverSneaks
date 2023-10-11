using EverSneaks.MAUI.ViewModels;

namespace EverSneaks.MAUI
{
    public partial class MainPage : ContentPage
    {
        private readonly MyApplication evergineApplication;

        public MainPage()
        {
            InitializeComponent();
            this.evergineApplication = new MyApplication();
            this.evergineView.Application = this.evergineApplication;
            this.BindingContext = new MainPageViewModel(this.evergineView);
        }
    }
}