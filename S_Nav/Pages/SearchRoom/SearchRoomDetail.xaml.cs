using S_Nav.Firebase;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace S_Nav.Pages.NavPage.Searches
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchRoomDetail : ContentPage
    {

        // Check viability in having a "loading" page to handle this query
        // allows removing this activity from memory if needed 
        // (only if garbage collection starts removing again)
        FirebaseConnection firebaseConnection = new FirebaseConnection();
        List<MapPoint> points;

        public SearchRoomDetail()
        {
            InitializeComponent();

            populatePicker(curLocPicker);
            populatePicker(destLocPicker);

            curLocPicker.SelectedItem = curLocPicker.Items[0];
            destLocPicker.SelectedItem = curLocPicker.Items[1];
        }

        void populatePicker(Picker picker)
        {
            // will definitely want to access these in a better way
            List<String> pointNames = new List<string>();
            LoadPoints lp = new LoadPoints();
            pointNames = lp.loadRoomNames();

            foreach (String name in pointNames)
            {
                picker.Items.Add(name);
            }
        }

        private async void SearchRoute_Clicked(object sender, EventArgs e)
        {
            if(curLocPicker.SelectedItem == null || destLocPicker.SelectedItem == null)
            {
                lblErrorText.Text = "ERROR - Must have a room selected in both room pickers";   
            }
            else
            {
                String curRoomText = curLocPicker.SelectedItem.ToString();
                String destRoomText = destLocPicker.SelectedItem.ToString();

                Preferences.Clear(); // failsafe, shouldn't be needed

                Preferences.Set("curLoc", curRoomText);
                Preferences.Set("destLoc", destRoomText);

                points = await firebaseConnection.GetFloorPoints("TRAE2");

                NavigationPage routePage = new NavigationPage(points);

                await Navigation.PushModalAsync(routePage);
            }
        }
    }
}