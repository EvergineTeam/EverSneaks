using EverSneaks.MAUI.Views;

namespace EverSneaks.MAUI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            InitializeRoutes();
        }

        private void InitializeRoutes()
        {
            Routing.RegisterRoute("detail", typeof(SneakersDetailsView));
        }
    }
}