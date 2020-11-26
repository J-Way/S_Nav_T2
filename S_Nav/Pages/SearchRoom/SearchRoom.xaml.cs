using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace S_Nav.Pages.NavPage.Searches
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchRoom : MasterDetailPage
    {
        public SearchRoom()
        {
            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (!(e.SelectedItem is SearchRoomMasterMenuItem item))
                return;
            else
            {
                if (item.Id == 0)
                {
                    Preferences.Clear(); // shouldn't try to display any route
                    Navigation.PushModalAsync(new NavigationPage());
                }
                else if (item.Id == 1)
                {
                    Navigation.PopModalAsync();
                }
            }
        }
    }
}