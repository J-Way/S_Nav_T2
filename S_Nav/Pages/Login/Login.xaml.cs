using S_Nav.Pages.NavPage.Searches;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace S_Nav.Pages.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        public Login()
        {
            InitializeComponent();
            Preferences.Clear();
        }

        // does nothing currently
        private void Register_Clicked(object sender, EventArgs e)
        {
            //Navigation.PushModalAsync(new SearchRoom());
        }

        // does nothing currently
        private void Login_Clicked(object sender, EventArgs e)
        {
            //Navigation.PushModalAsync(new SearchRoom());
        }

        // add SearchRoom page to stack
        private void RegisterLater_Clicked(object sender, EventArgs e)
        {
            Preferences.Clear();

            Navigation.PushModalAsync(new SearchRoom());
        }
    }
}