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
        Uri imageUri;
        string floorFile;

        public SearchRoomDetail()
        {
            InitializeComponent();

            populatePicker(curLocPicker);
            populatePicker(destLocPicker);
        }

        async void populatePicker(Picker picker)
        {
            points = await firebaseConnection.GetFloorPoints("TRAE2");

            foreach (var p in points)
            {
                picker.Items.Add(p.getPointName());
            }

            picker.SelectedItem = picker.Items[0];
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

                floorFile = "TRA-E-2.png";

                NavigationPage routePage = new NavigationPage(points, floorFile);

                await Navigation.PushModalAsync(routePage);
            }
        }
    }
}