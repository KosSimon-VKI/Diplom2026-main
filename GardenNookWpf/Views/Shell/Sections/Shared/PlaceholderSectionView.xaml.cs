using System.Threading.Tasks;
using System.Windows.Controls;

namespace GardenNookWpf.Views.Shell.Sections.Shared
{
    public partial class PlaceholderSectionView : UserControl, IMainSectionView
    {
        public PlaceholderSectionView(string title, string userRole)
        {
            InitializeComponent();

            TitleTextBlock.Text = title;
            RoleTextBlock.Text = $"Роль: {userRole}";
        }

        public bool IsBusy => false;

        public Task ActivateAsync()
        {
            return Task.CompletedTask;
        }

        public void Deactivate()
        {
        }
    }
}
