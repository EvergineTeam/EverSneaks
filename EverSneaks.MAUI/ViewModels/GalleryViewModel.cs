using System.Windows.Input;

namespace EverSneaks.MAUI.ViewModels
{
    internal class GalleryViewModel
    {
        public ICommand GoToDetailCommand { get; }

        public GalleryViewModel()
        {
            this.GoToDetailCommand = new Command(async () => await Shell.Current.GoToAsync("detail"));
        }
    }
}