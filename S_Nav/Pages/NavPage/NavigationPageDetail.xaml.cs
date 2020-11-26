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
        bool isRouting = false, isDrawing = false, needsAccess;

        private FirebaseConnection firebaseConnection;

        // image being loaded
        SKBitmap image;

        string floorFile;

        List<MapPoint> pointsToDraw;


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
            StrokeWidth = 30,
            Color = SKColors.Lime
        };
        SKPaint redStroke = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 30,
            Color = SKColors.OrangeRed
        };

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

        public NavigationPageDetail(List<MapPoint> p)
        {
            InitializeComponent();
            currentWing = Preferences.Get("curWing", null);

            isRouting = false;
            isDrawing = true;
            floorFile = $"TRA-{currentWing}.png";

            pointsToDraw = p;
        }
        
        public NavigationPageDetail(bool routing)
        {
            InitializeComponent();
            currentLocation = Preferences.Get("curLoc", null);
            currentWing = Preferences.Get("curWing", null);
            destinationWing = Preferences.Get("destWing", null);
            destinationLocation = Preferences.Get("destLoc", null);

            isRouting = routing;

            if (isRouting)
            {
                floorFile = $"TRA-{currentWing}.png";

                firebaseConnection = new FirebaseConnection();
            }
        }
        public NavigationPageDetail(string file)
        {
            InitializeComponent();
            currentLocation = Preferences.Get("curLoc", null);
            floorFile = file;
            if (floorFile.Contains("G"))
            {
                GWingButtonLayout();
            }
            else if (floorFile.Contains("E"))
            {
                EWingButtonLayout();
            }
        }

        ///     handles / calls all the drawing
        ///     if you need to refresh / reset, call invalidate in
        ///         NavigationPageDetail()
        private async void canvas_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKSurface surface = e.Surface; // screen
            SKCanvas canvas = surface.Canvas; // drawable screen

            int width = e.Info.Width; // screen dimensions
            int height = e.Info.Height;

            Preferences.Set("screen_width", width);
            Preferences.Set("screen_height", height);

            canvas.Scale(1, 1);

            SetFloorPlan(floorFile);
            canvas.DrawBitmap(image, new SKRect(0, 0, width, height));

            if (isRouting)
            {
                if (currentLocation != null)
                {
                    Console.WriteLine("Time to route!");

                    List<MapPoint> routePoints = await StartRouting();

                    NavigationPage drawPage = new NavigationPage(routePoints);

                    await Navigation.PushModalAsync(drawPage);

                    //DrawRoute(routePoints, canvas);
                    //
                    //canvas.DrawPoint(routePoints[routePoints.Count - 1].GetPointLocation(), redStroke);
                }
            }
            else if (isDrawing)
            {
                DrawRoute(pointsToDraw, canvas);

                canvas.DrawPoint(pointsToDraw[pointsToDraw.Count - 1].GetPointLocation(), redStroke);

                isDrawing = false;

                canvas.Save();
            }
        }

        private async Task<List<MapPoint>> StartRouting()
        {
            List<MapPoint> routePoints = new List<MapPoint>();

            // TODO: Replace floor point loader to Firebase fetch
            CrossWingRoute cwRoute = new CrossWingRoute(TestLoadFloorPoints.LoadTestFloorPoints());
            //List<FloorPoint> cwPoints = cwRoute.CalculateRoute();

            // can change to cwRoute start / end later
            if (currentWing.Equals(destinationWing))
            {
                List<List<MapPoint>> mapPoints = await firebaseConnection.GetFloorPoints2("TRA-"+currentWing);

                FloorRoute route = new FloorRoute(mapPoints, currentLocation, destinationLocation);
                List<MapPoint> floorRoutePoints = route.CalculateRoute();
                foreach (var frp in floorRoutePoints)
                {
                    Console.WriteLine($"--- [FRP] {frp.GetPointName()}");
                }
                routePoints.AddRange(floorRoutePoints);

                return routePoints;
            }
            else if (!currentWing.Equals(destinationWing))
            {

                List<List<MapPoint>> mapPoints = await firebaseConnection.GetFloorPoints2("TRA-" + currentWing);
                foreach (var item in cwRoute.floorPoints)
                {
                    if(currentWing.Substring(0,1) != destinationWing.Substring(0,1) && currentWing.Contains("2"))
                    {
                        destinationLocation = mapPoints[4][0].GetPointName();
                        Preferences.Set("curLoc", destinationLocation);
                        break;
                    }
                    else if (currentWing.Contains(item.wing) && currentWing.Contains(item.floor.ToString()))
                    {
                        needsAccess = Preferences.Get("accessibility", false);
                        if(!needsAccess)
                            destinationLocation = mapPoints[2][0].GetPointName();
                        else
                            foreach (var x in mapPoints[2])
                            {
                                if (x.getAccess())
                                {
                                    destinationLocation = x.GetPointName();
                                    break;
                                }
                            }
                        
                        Preferences.Set("curLoc", destinationLocation);
                        break;
                    }
                }

                FloorRoute route = new FloorRoute(mapPoints, currentLocation, destinationLocation);
                List<MapPoint> floorRoutePoints = route.CalculateRoute();
                foreach (var frp in floorRoutePoints)
                {
                    Console.WriteLine($"--- [FRP] {frp.GetPointName()}");
                }
                routePoints.AddRange(floorRoutePoints);

                isRouting = false;

                return routePoints;
            }


            //
            // I don't really know what you're trying here, let's revert back to it when 
            // we aren't as time contrained - Jordan.
            //
            // TODO: Extract to FloorRoute
            /*
            for (int i = 0; i < cwPoints.Count; i++)
            {
                FloorPoint cwp = cwPoints[i];
                Console.WriteLine($"----- [FP] {cwp.getName()}");
                List<List<MapPoint>> mapPoints = await firebaseConnection.GetFloorPoints2(cwp.getFBName());
                // List<MapPoint> traversalPoints = mapPoints[2];

                string src = "",
                       dst = "";

                if (i > 0)
                {
                    FloorPoint cwpPrev = cwPoints[i - 1]; // prev floor

                    if (cwpPrev.wing == cwp.wing)
                    {
                        // TODO: Remove hardcoded, find closest stairs from src if it exists
                        // use `traversalPoints` to search
                        if (cwp.wing == "E")
                            src = "stairsTopLeft";
                        else if (cwp.wing == "G")
                            src = "stairsBottom";
                    }
                    else if (cwp.connectsTo(cwpPrev))
                        src = $"hall{cwp.wing}{cwpPrev.wing}";
                }
                else
                    src = currentLocation; // starting point

                if (i < cwPoints.Count - 1) // not last
                {
                    FloorPoint cwpNext = cwPoints[i + 1]; // next floor

                    // Covers stairs
                    if (cwp.wing == cwpNext.wing)
                    {
                        // TODO: Remove hardcoded, find closest stairs from src if it exists
                        if (cwp.wing == "E")
                            dst = "stairsTopLeft";
                        else if (cwp.wing == "G")
                            dst = "stairsBottom";
                    }
                    // Covers cross-wing
                    else if (cwp.connectsTo(cwpNext)) // already implies cwp.wing != cwpNext.wing
                        dst = $"hall{cwp.wing}{cwpNext.wing}";
                }
                else
                    dst = destinationLocation;
                

                FloorRoute route = new FloorRoute(mapPoints, src, dst);
                List<MapPoint> floorRoutePoints = route.CalculateRoute();
                foreach (var frp in floorRoutePoints)
                {
                    Console.WriteLine($"--- [FRP] {frp.GetPointName()}");
                }
                routePoints.AddRange(floorRoutePoints);
            }
            */
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

        //GButtons View Function
        private void GWingButtonLayout()
        {
            // These might not actually work
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
            // I don't know if these actually work
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
            Preferences.Set("curWing", "E-1");

            NavigationPage routePage = new NavigationPage(true);
            await Navigation.PushModalAsync(routePage);
        }

        private async void ShowE2()
        {
            FloorUpButton.IsVisible = false;
            Preferences.Set("curWing", "E-2");

            NavigationPage routePage = new NavigationPage(true);
            await Navigation.PushModalAsync(routePage);
        }

        private async void ShowG1()
        {
            FloorDownButton.IsVisible = false;
            Preferences.Set("curWing", "G-1");

            NavigationPage routePage = new NavigationPage(true);
            await Navigation.PushModalAsync(routePage);
        }

        private async void ShowG2()
        {
            FloorUpButton.IsVisible = false;
            Preferences.Set("curWing", "G-2");

            NavigationPage routePage = new NavigationPage(true);
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