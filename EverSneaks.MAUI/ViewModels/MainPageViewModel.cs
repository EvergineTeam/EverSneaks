using EverSneaks.MAUI.Evergine;
using EverSneaks.Services;
using System.Windows.Input;

namespace EverSneaks.MAUI.ViewModels
{
    public class MainPageViewModel
    {
        public ICommand ColorCommand { get; set; }

        private EvergineView evergineView;
        private ControllerService controllerService;

        public MainPageViewModel(EvergineView evergineView) 
        {
            this.evergineView = evergineView;
            this.controllerService = this.evergineView.Application.Container.Resolve<ControllerService>();
            this.ColorCommand = new Command<string>(
                (color) =>
                {
                    switch (color)
                    {
                        case "red":
                            this.controllerService.Color = ControllerService.SneakerColor.Red;
                            break;
                        case "green":
                            this.controllerService.Color = ControllerService.SneakerColor.Orange;
                            break;
                        case "blue":
                            this.controllerService.Color = ControllerService.SneakerColor.Blue;
                            break;
                        case "gray":                            
                        default:
                            this.controllerService.Color = ControllerService.SneakerColor.Gray;
                            break;
                    }                    
                    
                });
        }
    }
}
