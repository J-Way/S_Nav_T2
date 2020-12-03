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
using S_Nav.Pages.NavPage.Searches;

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
        }

        public NavigationPageDetail(List<MapPoint> p)
        {
            InitializeComponent();
            currentWing = Preferences.Get("curWing", null);
            destinationWing = Preferences.Get("destWing", null);

            isRouting = false;
            isDrawing = true;
            floorFile = $"TRA-{currentWing}.png";

            pointsToDraw = p;

            SetButtons(pointsToDraw[pointsToDraw.Count-1].GetPointName());
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
            progressBar.Progress = 0.33f;

            if (isRouting)
            {
                if (currentLocation != null)
                {
                    Console.WriteLine("Time to route!");

                    List<MapPoint> routePoints = await StartRouting();

                    NavigationPage drawPage = new NavigationPage(routePoints);

                    progressBar.Progress = 0.99f;

                    await Navigation.PushModalAsync(drawPage);
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

            List<FloorPoint> macroMap = await firebaseConnection.GetMacroMap();
            CrossWingRoute cwRoute = new CrossWingRoute(macroMap);
            List<FloorPoint> cwPoints = cwRoute.CalculateRoute();

            progressBar.Progress = 0.66f;

            // can change to cwRoute start / end later
            if (currentWing.Equals(destinationWing))
            {
                List<List<MapPoint>> mapPoints = await firebaseConnection.GetFloorPoints2("TRA-" + currentWing);

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
                    // Won't work once wing AA gets routed
                    if(currentWing.Substring(0,1) != destinationWing.Substring(0,1) && currentWing.Contains("2"))
                    {
                        destinationLocation = mapPoints[4][0].GetPointName();
                        Preferences.Set("curLoc", destinationLocation);
                        break;
                    }
                    else if (currentWing.Contains(item.wing) && currentWing.Contains(item.floor.ToString()))
                    {
                        needsAccess = Preferences.Get("accessibility", false);
                        if (!needsAccess)
                            destinationLocation = mapPoints[2][0].GetPointName();
                        else
                        {
                            foreach (var x in mapPoints[2])
                            {
                                if (x.getAccess())
                                {
                                    destinationLocation = x.GetPointName();
                                    break;
                                }
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

        private void SetButtons(string finalFloorPoint)
        {
            progressBar.IsVisible = false;
            lblProcessing.IsVisible = false;

            ReturnButton.IsEnabled = true;
            ReturnButton.IsVisible = true;
            if (currentWing.Equals(destinationWing))
            {
                TraversalButton.IsEnabled = false;
                TraversalButton.IsVisible = false;
            }
            else
            {
                if (finalFloorPoint.Contains("stair"))
                {
                    if (currentWing.Contains("1"))
                    {
                        TraversalButton.IsVisible = true;
                        TraversalButton.IsEnabled = true;
                        TraversalButton.Text = "Move Up a floor";
                        TraversalButton.Clicked += delegate
                        {
                            UpClicked();
                        };
                    }
                    else
                    {
                        TraversalButton.IsVisible = true;
                        TraversalButton.IsEnabled = true;
                        TraversalButton.Text = "Move Down a floor";
                        TraversalButton.Clicked += delegate
                        {
                            DownClicked();
                        };
                    }
                }
                // hallway connection
                else
                {
                    TraversalButton.IsVisible = true;
                    TraversalButton.IsEnabled = true;
                    TraversalButton.Text = "Move across wings";
                    TraversalButton.Clicked += delegate
                    {
                        WingClicked();
                    };
                }
            }
        }

        private void DownClicked()
        {
            if (floorFile.Contains("G") && !floorFile.Contains("1"))
            {
                ShowG1();
            }
            else if (floorFile.Contains("E") && !floorFile.Contains("1"))
            {
                ShowE1();
            }
        }

        private void UpClicked()
        {
            if (floorFile.Contains("G") && floorFile.Contains("1"))
            {
                ShowG2();
            } else if (floorFile.Contains("E") && floorFile.Contains("1"))
            {
                ShowE2();
            }  
        }

        private void WingClicked()
        {
            if (floorFile.Equals("TRA-E-2.png"))
            {
                ShowG2();
            }
            else
            {
                ShowE2();
            }
        }

        private void ReturnButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new SearchRoom());
        }

        private async void ShowE1()
        {
            Preferences.Set("curWing", "E-1");

            NavigationPage routePage = new NavigationPage(true);
            await Navigation.PushModalAsync(routePage);
        }

        private async void ShowE2()
        {
            TraversalButton.IsVisible = false;
            Preferences.Set("curWing", "E-2");

            NavigationPage routePage = new NavigationPage(true);
            await Navigation.PushModalAsync(routePage);
        }

        private async void ShowG1()
        {
            TraversalButton.IsVisible = false;
            Preferences.Set("curWing", "G-1");

            NavigationPage routePage = new NavigationPage(true);
            await Navigation.PushModalAsync(routePage);
        }

        private async void ShowG2()
        {
            TraversalButton.IsVisible = false;
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