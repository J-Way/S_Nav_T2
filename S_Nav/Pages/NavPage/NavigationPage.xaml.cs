using S_Nav.Pages.NavPage;
using S_Nav.Pages.NavPage.Searches;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        // if this is called, something is going to get borked
        // find workaround for its single reference
        public NavigationPage()
        {
            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
            Detail = new NavigationPageDetail();
        }

        public NavigationPage(List<MapPoint> points)
        {
            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
            Detail = new NavigationPageDetail(points);
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