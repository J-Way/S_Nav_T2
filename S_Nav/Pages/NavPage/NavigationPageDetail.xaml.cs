using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using S_Nav.Navigation;
using SkiaSharp;
using SkiaSharp.Views.Forms;


using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace S_Nav
{

    [DesignTimeVisible(true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NavigationPageDetail : ContentPage
    {

        string currentWing, currentLocation, destinationWing, destinationLocation;
        bool isRouting = false;

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

        //
        // placeholder constructor, only called in direct page access
        // i.e. no route data given
        //
        public NavigationPageDetail()
        {
            InitializeComponent();

            // Get("x", null), null is placeholder value if preference not found
            currentWing = Preferences.Get("curWing", null);
            currentLocation = Preferences.Get("curLoc", null);
            destinationWing = Preferences.Get("destWing", null);
            destinationLocation = Preferences.Get("destLoc", null);

            floorFile = "S_Nav.Media.Images.TRA.E.TRA-E-1.png";
        }

        public NavigationPageDetail(string file)
        {
            InitializeComponent();
            currentLocation = Preferences.Get("curLoc", null);
            floorFile = file;
        }

        public NavigationPageDetail(bool routing)
        {
            InitializeComponent();

            currentWing = Preferences.Get("curWing", null);
            currentLocation = Preferences.Get("curLoc", null);
            destinationWing = Preferences.Get("destWing", null);
            destinationLocation = Preferences.Get("destLoc", null);

            isRouting = routing;

            if (isRouting)
            {
                floorFile = "S_Nav.Media.Images.TRA." + currentWing.Substring(0, 1) + ".TRA-" + currentWing + ".png";
            }
        }

        ///     handles / calls all the drawing
        ///     if you need to refresh / reset, call invalidate in
        ///         NavigationPageDetail()
        private void canvas_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            setFloorPlan(floorFile);
            printFloorPlans(e);
        }

        private void printFloorPlans(SKPaintSurfaceEventArgs e)
        {
            SKSurface surface = e.Surface; // screen
            SKCanvas canvas = surface.Canvas; // drawable screen

            int width = e.Info.Width; // screen dimensions
            int height = e.Info.Height;

            canvas.Scale(1, 1);

            canvas.DrawBitmap(image, new SKRect(0, 0, width, height));

            if (isRouting)
            {
                LoadPoints pointLoader = new LoadPoints();
                List<List<MapPoint>> points = new List<List<MapPoint>>();

                if (currentLocation.Substring(1, 1) == "1")
                {
                    points = pointLoader.loadE1Points(width, height);
                }
                else
                {
                    points = pointLoader.loadE2Points(width, height);
                }

                // Calls routing
                if (currentLocation != null)
                {
                    List<MapPoint> routePoints = new List<MapPoint>();
                    Route route = new Route(points);
                    routePoints = route.calculateRoute(); // convert all given points to calculated route
                    drawRoute(routePoints, canvas);

                    canvas.DrawPoint(routePoints[routePoints.Count - 1].pointLocation, redStroke);
                }
            }
        }

        // try to call only when loading new floor
        // (currently the same static image)
        private void setFloorPlan(string blueprint)
        {
            // Bitmap
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            using (Stream stream = assembly.GetManifestResourceStream(blueprint))
            {
                image = SKBitmap.Decode(stream);
            }
        }

        private async void DownClicked(object sender, EventArgs e)
        {
            Console.WriteLine("Down Clicked");
            image.Reset();

            NavigationPage routePage = new NavigationPage("S_Nav.Media.Images.TRA.E.TRA-E-1.png");
            await Navigation.PushModalAsync(routePage);
        }

        private async void UpClicked(object sender, EventArgs e)
        {
            Console.WriteLine("Up Clicked");
            image.Reset();

            NavigationPage routePage = new NavigationPage("S_Nav.Media.Images.TRA.E.TRA-E-2.png");
            await Navigation.PushModalAsync(routePage);
        }
      
        //
        // Takes a list of points and  draws lines between them
        //
        private void drawRoute(List<MapPoint> points, SKCanvas canvas)
        {
            for (int i = 0; i < points.Count - 1; i++) // count produces higher value than max index
            {
                if (i == 0)
                {
                    canvas.DrawPoint(points[i].getPointLocation(), greenStroke);
                }
                else if (i < points.Count - 1) {
                    canvas.DrawPoint(points[i].getPointLocation(), blackStroke); // not super necessary, illustrating where points were found
                }
                canvas.DrawLine(points[i].getPointLocation(), points[i + 1].getPointLocation(), routeColour);
            }
        }     
    }
}