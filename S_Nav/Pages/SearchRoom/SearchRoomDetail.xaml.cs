using S_Nav.Firebase;
using S_Nav.Models;
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
        string floorFile;
        List<Floor> floors;

        public SearchRoomDetail()
        {
            InitializeComponent();

            // PopulateRoomPicker(curWingPicker, curRoomPicker, destWingPicker, destRoomPicker);
            PopulateWingPickers();
        }

        //////////
        // this is called before any location data is loaded
        async void PopulateWingPickers()
        {
            floors = await firebaseConnection.GetFloors();

            foreach (var item in floors)
            {
                curWingPicker.Items.Add(item.GetFloorName());
                destWingPicker.Items.Add(item.GetFloorName());
            }

            curWingPicker.SelectedIndex = 0;
            destWingPicker.SelectedIndex = 0;

            curWingPicker.IsEnabled = true;
            destWingPicker.IsEnabled = true;
        }

        async void PopulateRoomPicker(Picker picker, string curFloor)
        {
            foreach (var item in floors)
            {
                // I'm not happy with this solution either
                if (item.GetFloorName() == curFloor)
                {
                    var rooms = item.GetRooms();
                    foreach (var r in rooms)
                    {
                        picker.Items.Add(r);
                    }
                }

            }
            picker.SelectedItem = picker.Items[0];
        }

        private async void SearchRoute_Clicked(object sender, EventArgs e)
        {
            if(curRoomPicker.SelectedItem == null || destRoomPicker.SelectedItem == null)
            {
                lblErrorText.Text = "ERROR - Must have a room selected in both room pickers";   
            }
            else
            {
                Preferences.Set("curLoc", curRoomPicker.SelectedItem.ToString());
                Preferences.Set("curWing", curWingPicker.SelectedItem.ToString());
                Preferences.Set("destLoc", destRoomPicker.SelectedItem.ToString());

                NavigationPage routePage = new NavigationPage(true);

                await Navigation.PushModalAsync(routePage);
            }
        }

        private void CurWingPicker_Unfocused(object sender, FocusEventArgs e)
        {
            curRoomPicker.Items.Clear();
            if (!curWingPicker.SelectedItem.ToString().Contains("Select"))
            {
                curRoomPicker.IsEnabled = true;
                PopulateRoomPicker(curRoomPicker, curWingPicker.SelectedItem.ToString());
            }
            else
            {
                curRoomPicker.IsEnabled = false;
            }
        }

        private void DestWingPicker_Unfocused(object sender, FocusEventArgs e)
        {
            destRoomPicker.Items.Clear();
            if (!destWingPicker.SelectedItem.ToString().Contains("Select"))
            {
                destRoomPicker.IsEnabled = true;
                PopulateRoomPicker(destRoomPicker, destWingPicker.SelectedItem.ToString());
            }
            else
            {
                destRoomPicker.IsEnabled = false;
            }
        }
    }
}