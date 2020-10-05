using System;
using System.Collections.Generic;
using System.Text;

/*
 * FirebaseAuthentication.net
 * FirebaseDatabase.net
 * FirebaseStorage.net
 * 
 * Newtonsoft.Json // Questionable if needed
 * */

namespace S_Nav.Firebase
{
    class FirebaseConnection
    {
        // Test this out as a URI as well
        string firebaseLink = "https://fir-nav-fbd51.firebaseio.com/";


        FirebaseClient firebase = new FirebaseClient(firebaseLink, new FirebaseOptions { AuthTokenAsyncFactory = () => AnonLogin() });

        public async Task<Uri> GetImage(string floor)
        {
            // unless we start changing the data names / structures we can skip this
            //var query = await firebase.Child("FLOOR_DATA/"+floor).OnceAsync<object>();

            Uri image = new Uri(await new FirebaseStorage(firebaseLink).Child("Images").Child(floor + ".jpg").GetDownloadUrlAsync());

            // this could be shortened to a one line function, but leave as is for now
            return image;
        }

        public static async Task<string> AnonLogin()
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyDlrs12gBooCtCtg6SXVG2xP3BE-jYXk7g"));
            var auth = await authProvider.SignInAnonymouslyAsync();
            return auth.FirebaseToken;
        }
    }
}
