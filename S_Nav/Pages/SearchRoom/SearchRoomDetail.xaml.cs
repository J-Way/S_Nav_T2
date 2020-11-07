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
        LoadPoints lp = new LoadPoints();

        public SearchRoomDetail()
        {
            InitializeComponent();

            populateWingList(curWingPicker);
            populateWingList(destWingPicker);
        }

        

        void populateWingList(Picker picker)
        {
            List<string> roomNames = lp.loadWingNames();

            foreach (var item in roomNames)
            {
                picker.Items.Add(item);
            }
        }

        void populateRoomPicker(Picker picker)
        {
            // will definitely want to access these in a better way
            List<string> pointNames = lp.loadRoomNames();

            foreach (String name in pointNames)
            {
                picker.Items.Add(name);
            }
        }

        private void SearchRoute_Clicked(object sender, EventArgs e)
        {
            if(destRoomPicker.SelectedItem == null || curRoomPicker.SelectedItem == null)
            {
                lblErrorText.Text = "ERROR - Must have a room selected in both room pickers";   
            }
            else
            {
                Preferences.Clear(); // failsafe, shouldn't be needed

                Preferences.Set("curWing", curWingPicker.SelectedItem.ToString());
                Preferences.Set("curLoc", curRoomPicker.SelectedItem.ToString());

                Preferences.Set("destWing", destWingPicker.SelectedItem.ToString());
                Preferences.Set("destLoc", destRoomPicker.SelectedItem.ToString());

                bool isRouting = true;

                Navigation.PushModalAsync(new NavigationPage(isRouting));
            }
        }

        private void curWingPicker_Unfocused(object sender, FocusEventArgs e)
        {
            if (curWingPicker.SelectedIndex != -1)
            {
                populateRoomPicker(curRoomPicker);
                curRoomPicker.IsEnabled = true;
            }
        }
        private void destWingPicker_Unfocused(object sender, FocusEventArgs e)
        {
            if(destWingPicker.SelectedIndex != -1)
            {
                populateRoomPicker(destRoomPicker);
                destRoomPicker.IsEnabled = true;
            }
        }
    }
}