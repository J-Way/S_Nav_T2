using System;
using Firebase.Database;
using System.Threading.Tasks;
using Firebase.Storage;
using Firebase.Auth;
using System.Collections.Generic;
using System.Net;
using Firebase.Database.Query;
using Newtonsoft.Json;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;

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

        // could merge into one function with GetImage later
        public async Task<List<MapPoint>> GetFloorPoints(string floor, int width, int height)
        {
            List<MapPoint> mapPoints = new List<MapPoint>();

            // switch to map point list after figuring out point name issue
            var items = await (firebaseDB.Child("FLOOR_DATA").Child(floor).Child("FLOOR_POINTS").OnceAsync<List<object>>());
            foreach (var item in items)
            {
                foreach (var curPoint in item.Object)
                {
                    var test = JObject.Parse(curPoint.ToString()).GetEnumerator();
                    
                    // This is a horrible implementation and should be replaced
                    test.MoveNext();
                    var name = test.Current.Value.ToString();

                    test.MoveNext();
                    var x = float.Parse(test.Current.Value.ToString());

                    test.MoveNext();
                    var y = float.Parse(test.Current.Value.ToString());

                    mapPoints.Add(new MapPoint(name, width * x, height * y));
                }
            }

            return mapPoints;
        }

        public async Task<List<MapPoint>> GetFloorPoints(string floor)
        {
            List<MapPoint> mapPoints = new List<MapPoint>();

            // switch to map point list after figuring out point name issue
            var items = await (firebaseDB.Child("FLOOR_DATA").Child(floor).Child("FLOOR_POINTS").OnceAsync<List<object>>());
            foreach (var item in items)
            {
                foreach (var curPoint in item.Object)
                {
                    var test = JObject.Parse(curPoint.ToString()).GetEnumerator();

                    // This is a horrible implementation and should be replaced
                    test.MoveNext();
                    var name = test.Current.Value.ToString();

                    test.MoveNext();
                    var x = float.Parse(test.Current.Value.ToString());

                    test.MoveNext();
                    var y = float.Parse(test.Current.Value.ToString());

                    //mapPoints.Add(new MapPoint(name, width * x, height * y));
                    mapPoints.Add(new MapPoint(name, x, y));
                }
            }

            return mapPoints;
        }

        public static async Task<string> AnonLogin()
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyDlrs12gBooCtCtg6SXVG2xP3BE-jYXk7g"));
            var auth = await authProvider.SignInAnonymouslyAsync();
            return auth.FirebaseToken;
        }
    }
}
