using System.Threading.Tasks;

namespace GardenNookWpf.Views.Shell
{
    public interface IMainSectionView
    {
        bool IsBusy { get; }

        Task ActivateAsync();

        void Deactivate();
    }
}
