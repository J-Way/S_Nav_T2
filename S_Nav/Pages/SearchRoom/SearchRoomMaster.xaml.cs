using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace S_Nav.Pages.NavPage.Searches
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchRoomMaster : ContentPage
    {
        public ListView ListView;

        public SearchRoomMaster()
        {
            InitializeComponent();

            BindingContext = new SearchRoomMasterViewModel();
            ListView = MenuItemsListView;
        }

        class SearchRoomMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<SearchRoomMasterMenuItem> MenuItems { get; set; }

            public SearchRoomMasterViewModel()
            {
                MenuItems = new ObservableCollection<SearchRoomMasterMenuItem>(new[]
                {
                    new SearchRoomMasterMenuItem { Id = 0, Title = "View Map" }
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