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
using S_Nav.Navigation;

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
            //firebaseFiles = new FirebaseStorage(fileLink, fileOptions);
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
            try
            {
                var items = await firebaseDB.Child("FLOORS").OrderByKey().OnceAsync<object>();

                List<Floor> floors = new List<Floor>();

                foreach (var wing in items)
                {
                    floors.Add(new Floor(wing.Key.ToString(), JsonConvert.DeserializeObject<string[]>(wing.Object.ToString())));
                }

                return floors;
            }
            catch(Exception e)
            {
                throw (e);
            }
        }

        // Fetch data to be used for cross-wing. Pending rename
        public async Task<List<FloorPoint>> GetMacroMap()
        {
            List<FloorPoint> floorPoints = new List<FloorPoint>();

            var floors = await firebaseDB.Child("FLOOR_CONNECTIONS").OnceAsync<List<string>>();

            foreach (var floor in floors)
            {
                // Initiate floor
                FloorPoint fp = new FloorPoint(floor.Key.Substring(4));
                // Substring of 4 to omit TRA-, DAV-, HMC-. No expectation of routing to another campus

                // Auto add connections from previously added floor points
                List<FloorPoint> connectedFloors = floorPoints.FindAll(ofp => floor.Object.Contains(ofp.getFBName()));
                fp.addConnections(connectedFloors.ToArray());
                
                floorPoints.Add(fp);
            }

            return floorPoints;
        }

        public async Task<List<List<MapPoint>>> GetFloorPoints2(string floor)
        {
            List<List<MapPoint>> mapPoints = new List<List<MapPoint>>();

            var items = await firebaseDB.Child("FLOOR_DATA").Child(floor).Child("FLOOR_POINTS").OnceAsync<List<object>>();

            int height = Preferences.Get("screen_height", 0);
            int width = Preferences.Get("screen_width", 0);

            foreach (var item in items)
            {
                List<MapPoint> points = new List<MapPoint>();

                foreach (var curPoint in item.Object)
                {
                    var response = JObject.Parse(curPoint.ToString()).GetEnumerator();

                    bool accessible = false;
                    // This is a horrible implementation and should be replaced
                    response.MoveNext();

                    var name = response.Current.Value.ToString();

                    response.MoveNext();
                    var x = width * float.Parse(response.Current.Value.ToString());

                    response.MoveNext();
                    var y = height * float.Parse(response.Current.Value.ToString());

                    response.MoveNext();

                    if (response.Current.Key.ToString() == "isAccessible")
                    {
                        accessible = bool.Parse(response.Current.Value.ToString());
                    }

                    points.Add(new MapPoint(name, x, y, accessible));
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
