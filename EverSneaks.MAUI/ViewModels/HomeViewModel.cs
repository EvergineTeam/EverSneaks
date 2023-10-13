using System.Windows.Input;

namespace EverSneaks.MAUI.ViewModels
{
    internal class HomeViewModel
    {
        public ICommand GoToGalleryCommand { get; }

        public HomeViewModel()
        {
            this.GoToGalleryCommand = new Command(async () => await Shell.Current.GoToAsync("gallery"));
        }
    }
}