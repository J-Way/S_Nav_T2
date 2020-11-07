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
        // 
        // Will likely need to move data loading to a prior blank activity to avoid user holdups
        //
        FirebaseConnection firebaseConnection = new FirebaseConnection();
        List<MapPoint> points;
        string floorFile;

        public SearchRoomDetail()
        {
            InitializeComponent();

            populatePicker(curRoomPicker, destRoomPicker);
        }

        async void populatePicker(Picker curPicker, Picker destPicker)
        {
            points = await firebaseConnection.GetFloorPoints("TRAE2");

            foreach (var p in points)
            {
                curPicker.Items.Add(p.getPointName());
                destPicker.Items.Add(p.getPointName());
            }

            curPicker.SelectedItem = curPicker.Items[0];
            destPicker.SelectedItem = curPicker.Items[0];
        }

        private async void SearchRoute_Clicked(object sender, EventArgs e)
        {
            if(curRoomPicker.SelectedItem == null || destRoomPicker.SelectedItem == null)
            {
                lblErrorText.Text = "ERROR - Must have a room selected in both room pickers";   
            }
            else
            {
                String curRoomText = curRoomPicker.SelectedItem.ToString();
                String destRoomText = destRoomPicker.SelectedItem.ToString();

                Preferences.Set("curLoc", curRoomText);
                Preferences.Set("destLoc", destRoomText);

                floorFile = "TRA-E-2.png";

                NavigationPage routePage = new NavigationPage(points, floorFile);

                await Navigation.PushModalAsync(routePage);
            }
        }
    }
}