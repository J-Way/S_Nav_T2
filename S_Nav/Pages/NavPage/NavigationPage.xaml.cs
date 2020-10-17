
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace S_Nav
{
    /// <summary>
    ///  "Glue Page", pulls detail (main thing being displayed) and master (tab items together)
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NavigationPage : MasterDetailPage
    {
        public NavigationPage()
        {
            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
        }

        public NavigationPage(bool isRouting)
        {
            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
            Detail = new NavigationPageDetail(isRouting);
        }

        public NavigationPage(string floorFile)
        {
            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
            Detail = new NavigationPageDetail(floorFile);
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as NavigationPageMasterMenuItem;
            if (item == null)
                return;
            else
            {
                if (item.Id == 0)
                {
                    Navigation.PopModalAsync();
                }
                else if(item.Id == 1)
                {
                    // bad way of doing things
                    //  - find out why pop to root isn't working
                    Navigation.PopModalAsync();
                    Navigation.PopModalAsync();
                }
            }
        }
    }
}