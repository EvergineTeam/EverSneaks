using EverSneaks.MAUI.Views;
using System.Windows.Input;

namespace EverSneaks.MAUI.ViewModels
{
    internal class GalleryViewModel
    {
        public ICommand GoToDetailCommand { get; }

        public GalleryViewModel()
        {
            this.GoToDetailCommand = new Command(
                                        execute: async () => await Shell.Current.GoToAsync("detail"),
                                        canExecute: () => Shell.Current.CurrentPage is GalleryView);
        }
    }
}