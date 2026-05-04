using System.Threading.Tasks;

namespace GardenNookWpf.Views.Shell.Controllers
{
    public interface IMainSectionController
    {
        bool IsBusy { get; }

        Task ActivateAsync();

        void Deactivate();
    }
}
