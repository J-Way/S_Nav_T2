using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using SkiaSharp;
using SkiaSharp.Views.Forms;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Threading.Tasks;
using S_Nav.Firebase;
using S_Nav.Navigation;

namespace S_Nav
{

    [DesignTimeVisible(true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NavigationPageDetail : ContentPage
    {

        string currentWing, currentLocation, destinationWing, destinationLocation;
        bool isRouting = false;

        private FirebaseConnection firebaseConnection;

        // temporary route colour
        SKPaint routeColour = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 5,
            Color = SKColors.Magenta
        };

        // spare colours, try to use "strokes" unless
        // you know what you're doing
        SKPaint blackStroke = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 10,
            Color = SKColors.Black
        };
        SKPaint greenStroke = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 15,
            Color = SKColors.SeaGreen
        };
        SKPaint redStroke = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 15,
            Color = SKColors.IndianRed
        };


        // image being loaded
        SKBitmap image;

        string floorFile;

        // creates detail page
        // dont touch this
        public NavigationPageDetail()
        {
            InitializeComponent();
            currentLocation = Preferences.Get("curLoc", null);
            destinationLocation = Preferences.Get("destLoc", null);
            floorFile = "TRA-E-2.png";
            EWingButtonLayout();
        }

        public NavigationPageDetail(List<MapPoint> p, string file)
        {
            InitializeComponent();
            currentLocation = Preferences.Get("curLoc", null);
            floorFile = file;
        }
        
        public NavigationPageDetail(bool routing)
        {
            InitializeComponent();
            currentLocation = Preferences.Get("curLoc", null);
            currentWing = Preferences.Get("curWing", null);

            isRouting = routing;

            if (isRouting)
            {
                floorFile = "TRA-"  + currentWing + ".png";

                firebaseConnection = new FirebaseConnection();
            }
        }

        ///     handles / calls all the drawing
        ///     if you need to refresh / reset, call invalidate in
        ///         NavigationPageDetail()
        private void canvas_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        { 
            SKSurface surface = e.Surface; // screen
            SKCanvas canvas = surface.Canvas; // drawable screen

            int width = e.Info.Width; // screen dimensions
            int height = e.Info.Height;

            canvas.Scale(1, 1);

            SetFloorPlan(floorFile);
            canvas.DrawBitmap(image, new SKRect(0, 0, width, height));

            if (isRouting)
            {
                if (currentLocation != null)
                {
                    Console.WriteLine("Time to route!");

                    List<MapPoint> routePoints = StartRouting().Result;

                    DrawRoute(routePoints, canvas);

                    canvas.DrawPoint(routePoints[routePoints.Count - 1].GetPointLocation(), redStroke);
                }
            }
            canvas.Save();
        }

        private async Task<List<MapPoint>> StartRouting()
        {
            List<MapPoint> routePoints = new List<MapPoint>();

            // TODO: Replace floor point loader to Firebase fetch
            CrossWingRoute cwRoute = new CrossWingRoute(TestLoadFloorPoints.LoadTestFloorPoints());
            List<FloorPoint> cwPoints = cwRoute.calculateRoute();
            Console.WriteLine("Multi floors routed");

            // TODO: Remove temp
            var routePairs = new List<(string src, string dst)>
            {
                ("E101", "stairsTopLeft"),
                ("stairsTopLeft", "hallEG"),
                ("hallGE", "stairsBottom"),
                ("stairsBottom", "G101")
            };

            for (int i = 0; i < cwPoints.Count; i++)
            {
                Console.WriteLine($"----- [FP] {cwPoints[i].getName()}");
                List<List<MapPoint>> mapPoints = await firebaseConnection.GetFloorPoints2(cwPoints[i].getFBName());
                FloorRoute route = new FloorRoute(mapPoints, routePairs[i].src, routePairs[i].dst);
                List<MapPoint> floorRoutePoints = route.calculateRoute();
                foreach (var frp in floorRoutePoints)
                {
                    Console.WriteLine($"--- [FRP] {frp.GetPointName()}");
                }
                routePoints.AddRange(floorRoutePoints);
            }

            return routePoints;
        }

        // try to call only when loading new floor
        // (currently the same static image)
        private void SetFloorPlan(string file)
        {
            // Bitmap
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            String resourceId = "S_Nav.Media.Images.TRA." + file;
            using (Stream stream = assembly.GetManifestResourceStream(resourceId))
            {
                image = SKBitmap.Decode(stream);
            }
        }


        public NavigationPageDetail(string file)
        {
            InitializeComponent();
            currentLocation = Preferences.Get("curLoc", null);
            floorFile = file;
            if (floorFile.Contains("G")){
                GWingButtonLayout();
            } else if (floorFile.Contains("E"))
            {
                EWingButtonLayout();
            }
        }

        //GButtons View Function
        private void GWingButtonLayout()
        {
            WingLeftButton.IsVisible = false;
            WingUpButton.IsVisible = false;
            WingRightButton.Text = "E Wing";
            WingDownButton.IsVisible = false;
            if (floorFile.Contains("1"))
            {
                FloorDownButton.IsVisible = false;
                FloorUpButton.IsVisible = true;
            } else {
                FloorDownButton.IsVisible = true;
                FloorUpButton.IsVisible = false;
            }
        }

        //EButton View Function
        private void EWingButtonLayout()
        {
            WingLeftButton.IsVisible = true;
            WingUpButton.IsVisible = true;
            WingDownButton.IsVisible = false;
            WingRightButton.Text = "C Wing";
            WingUpButton.Text = "B Wing";
            WingLeftButton.Text = "G Wing";
            
            if (floorFile.Contains("1"))
            {
                FloorDownButton.IsVisible = false;
                FloorUpButton.IsVisible = true;
            }
            else
            {
                FloorDownButton.IsVisible = true;
                FloorUpButton.IsVisible = false;

            }
        }

        private void DownClicked(object sender, EventArgs e)
        {
            image.Reset();
            if (floorFile.Contains("G") && !floorFile.Contains("1"))
            {
                ShowG1();
            }
            else if (floorFile.Contains("E") && !floorFile.Contains("1"))
            {
                ShowE1();
            }
        }

        private void UpClicked(object sender, EventArgs e)
        {
            Console.WriteLine("Up Clicked");
            image.Reset();
            if (floorFile.Contains("G") && floorFile.Contains("1"))
            {
                ShowG2();
            } else if (floorFile.Contains("E") && floorFile.Contains("1"))
            {
                ShowE2();
            }  
        }

        private void WingLeftClicked(object sender, EventArgs e)
        {
            if (floorFile.Equals("TRA-E-2.png"))
            {
                ShowG2();
            }
        }

        private void WingRightClicked(object sender, EventArgs e)
        {
            if (floorFile.Equals("TRA-G-2.png"))
            {
                ShowE2();
            }
        }

        private async void ShowE1()
        {
            NavigationPage routePage = new NavigationPage("TRA-E-1.png");
            await Navigation.PushModalAsync(routePage);
        }

        private async void ShowE2()
        {
            FloorUpButton.IsVisible = false;
            NavigationPage routePage = new NavigationPage("TRA-E-2.png");
            await Navigation.PushModalAsync(routePage);
        }

        private async void ShowG1()
        {
            FloorDownButton.IsVisible = false;
            NavigationPage routePage = new NavigationPage("TRA-G-1.png");
            await Navigation.PushModalAsync(routePage);
        }

        private async void ShowG2()
        {
            FloorUpButton.IsVisible = false;
            NavigationPage routePage = new NavigationPage("TRA-G-2.png");
            await Navigation.PushModalAsync(routePage);
        }
        //
        // Takes a list of points and  draws lines between them
        //
        private void DrawRoute(List<MapPoint> points, SKCanvas canvas)
        {
            for (int i = 0; i < points.Count - 1; i++) // count produces higher value than max index
            {
                if (i == 0)
                {
                    canvas.DrawPoint(points[i].GetPointLocation(), greenStroke);
                }
                else if (i < points.Count - 1) {
                    canvas.DrawPoint(points[i].GetPointLocation(), blackStroke); // not super necessary, illustrating where points were found
                }
                canvas.DrawLine(points[i].GetPointLocation(), points[i + 1].GetPointLocation(), routeColour);
            }
        }
    }
}