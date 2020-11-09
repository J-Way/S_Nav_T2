using System;
using Firebase.Database;
using System.Threading.Tasks;
using Firebase.Storage;
using Firebase.Auth;
using System.Collections.Generic;
using Firebase.Database.Query;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using Xamarin.Essentials;
using S_Nav.Models;

namespace S_Nav.Firebase
{
    class FirebaseConnection
    {
        // Test this out as a URI as well
        static string fileLink, dbLink, apiKey, debugMail, debugPw;

        static FirebaseOptions dbOptions;
        static FirebaseStorageOptions fileOptions;

        FirebaseClient firebaseDB;
        FirebaseStorage firebaseFiles;

        public FirebaseConnection()
        {
            FirebaseSetup();

            // these next 4 lines of code won't always need to exist concurrently
            dbOptions = new FirebaseOptions { AuthTokenAsyncFactory = () => AnonLogin() };
            fileOptions = new FirebaseStorageOptions { AuthTokenAsyncFactory = () => AnonLogin() };

            firebaseDB = new FirebaseClient(dbLink, dbOptions);
            firebaseFiles = new FirebaseStorage(fileLink, fileOptions);
        }

        public static void FirebaseSetup()
        {
            Assembly assembly = typeof(FirebaseConnection).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("S_Nav.Config.Config.json");

            using (var reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();
                JObject jsonData = JObject.Parse(json);

                foreach (var item in jsonData)
                {
                    switch (item.Key.ToString())
                    {
                        case "apiKey":
                            apiKey = item.Value.ToString();
                            break;
                        case "dbLink":
                            dbLink = item.Value.ToString();
                            break;
                        case "fileLink":
                            fileLink = item.Value.ToString();
                            break;
                        case "testEmail":
                            debugMail = item.Value.ToString();
                            break;
                        case "testPw":
                            debugPw = item.Value.ToString();
                            break;
                    }
                }
            }
        }

        public async Task<Uri> GetImage(string floor)
        {
            // unless we start changing the data names / structures we can skip this
            //var query = await firebase.Child("FLOOR_DATA/"+floor).OnceAsync<object>();
            
            Uri image = new Uri(await firebaseFiles.Child("Images").Child(floor + ".jpg").GetDownloadUrlAsync());
            return image;
        }

        public async Task<List<Floor>> GetFloors()
        {
            var items = await firebaseDB.Child("FLOORS").OrderByKey().OnceAsync<object>();

            List<Floor> floors = new List<Floor>();

            foreach (var wing in items)
            {
                floors.Add(new Floor(wing.Key.ToString(), JsonConvert.DeserializeObject<string[]>(wing.Object.ToString())));
            }

            return floors;
        }

        public async Task<List<MapPoint>> GetFloorPoints(string floor)
        {
            List<MapPoint> mapPoints = new List<MapPoint>();

            // switch to map point list after figuring out point name issue
            var items = await firebaseDB.Child("FLOOR_DATA").Child(floor).Child("FLOOR_POINTS").OnceAsync<object>();
            //foreach (var item in items)
            //{
            //    foreach (var curPoint in item.Object)
            //    {
            //        var point = JObject.Parse(curPoint.ToString()).GetEnumerator();
            //
            //        // This is a horrible implementation and should be replaced
            //        point.MoveNext();
            //        var name = point.Current.Value.ToString();
            //
            //        point.MoveNext();
            //        var x = float.Parse(point.Current.Value.ToString());
            //
            //        point.MoveNext();
            //        var y = float.Parse(point.Current.Value.ToString());
            //
            //        //mapPoints.Add(new MapPoint(name, width * x, height * y));
            //        mapPoints.Add(new MapPoint(name, x, y));
            //    }
            //}

            return mapPoints;
        }

        public async Task<List<List<MapPoint>>> GetFloorPoints2(string floor)
        {
            List<List<MapPoint>> mapPoints = new List<List<MapPoint>>();

            var items = await firebaseDB.Child("FLOOR_DATA").Child(floor).Child("FLOOR_POINTS").OnceAsync<List<object>>();
            foreach (var item in items)
            {
                List<MapPoint> points = new List<MapPoint>();

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
                    points.Add(new MapPoint(name, x, y));
                }

                mapPoints.Add(points);
            }
            return mapPoints;
        }

        public static async Task<string> AnonLogin()
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyDlrs12gBooCtCtg6SXVG2xP3BE-jYXk7g"));
            var auth = await authProvider.SignInAnonymouslyAsync();
            return auth.FirebaseToken;
        }

        public static async Task<string> CheckToken(string apiKey, string debugMail, string debugPw)
        {
            if (Preferences.Get("authToken", false))
                return Preferences.Get("authToken", "");

            return await DebugEmailLogin(apiKey, debugMail, debugPw);
        }

        public static async Task<string> DebugEmailLogin(string apiKey, string debugMail, string debugPw)
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
            var auth = await authProvider.SignInWithEmailAndPasswordAsync(debugMail, debugPw);

            Preferences.Set("authToken", auth.FirebaseToken);

            return auth.FirebaseToken;
        }
    }
}
