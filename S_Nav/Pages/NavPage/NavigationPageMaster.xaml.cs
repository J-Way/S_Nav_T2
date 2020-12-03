using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace S_Nav
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NavigationPageMaster : ContentPage
    {
        public ListView ListView;

        public NavigationPageMaster()
        {
            InitializeComponent();

            BindingContext = new NavigationPageMasterViewModel();
            ListView = MenuItemsListView;
        }

        class NavigationPageMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<NavigationPageMasterMenuItem> MenuItems { get; set; }

            public NavigationPageMasterViewModel()
            {
                MenuItems = new ObservableCollection<NavigationPageMasterMenuItem>(new[]
                {
                    new NavigationPageMasterMenuItem { Id = 0, Title = "Search Rooms" }
                });
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}