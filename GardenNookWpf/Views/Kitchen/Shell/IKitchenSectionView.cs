using System.Threading.Tasks;

namespace GardenNookWpf.Views.Kitchen.Shell
{
    public interface IKitchenSectionView
    {
        bool IsBusy { get; }

        Task ActivateAsync();

        void Deactivate();
    }
}
