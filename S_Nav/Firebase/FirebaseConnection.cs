using System;
using Firebase.Database;
using System.Threading.Tasks;
using Firebase.Storage;
using Firebase.Auth;

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
        static string dbLink = "https://fir-nav-fbd51.firebaseio.com/";
        static string fileLink = "fir-nav-fbd51.appspot.com";

        static FirebaseOptions dbOptions = new FirebaseOptions { AuthTokenAsyncFactory = () => AnonLogin() };
        static FirebaseStorageOptions fileOptions = new FirebaseStorageOptions { AuthTokenAsyncFactory = () => AnonLogin() };


        FirebaseClient firebaseDB = new FirebaseClient(dbLink, dbOptions);
        FirebaseStorage firebaseFiles = new FirebaseStorage(fileLink, fileOptions);


        public async Task<Uri> GetImage(string floor)
        {
            // unless we start changing the data names / structures we can skip this
            //var query = await firebase.Child("FLOOR_DATA/"+floor).OnceAsync<object>();
            
            Uri image = new Uri(await firebaseFiles.Child("Images").Child(floor + ".jpg").GetDownloadUrlAsync());
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
