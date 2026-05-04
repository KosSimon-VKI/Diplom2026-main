using System.Threading.Tasks;

namespace GardenNookWpf.Views.Kitchen.Shell.Controllers
{
    public interface IKitchenSectionController
    {
        bool IsBusy { get; }

        Task ActivateAsync();

        void Deactivate();
    }
}
