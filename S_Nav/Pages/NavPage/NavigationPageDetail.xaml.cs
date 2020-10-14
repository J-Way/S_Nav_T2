using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using S_Nav.Navigation;
using SkiaSharp;
using SkiaSharp.Views.Forms;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using S_Nav.Firebase;
using System.Net.Http;
using System.Threading.Tasks;
using System.Numerics;
using System.Reflection;

namespace S_Nav
{

    [DesignTimeVisible(true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NavigationPageDetail : ContentPage
    {

        String currentLocation;

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

        public NavigationPageDetail()
        {
            InitializeComponent();
        }

        // creates detail page
        // dont touch this
        public NavigationPageDetail(List<MapPoint> p, string file)
        {
            InitializeComponent();
            currentLocation = Preferences.Get("curLoc", null);
        }

        ///     handles / calls all the drawing
        ///     if you need to refresh / reset, call invalidate in
        ///         NavigationPageDetail()
        private void canvas_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            if (points.Count < 1)
            {
                return;
            }


            SKSurface surface = e.Surface; // screen
            SKCanvas canvas = surface.Canvas; // drawable screen

            int width = e.Info.Width; // screen dimensions
            int height = e.Info.Height;

            canvas.Scale(1, 1);

            setFloorPlan(floorFile);
            canvas.DrawBitmap(image, new SKRect(0, 0, width, height));

            canvas.Save();

            // Calls routing
            if (currentLocation != null)
            {
                LoadPoints pointLoader = new LoadPoints();
                List<MapPoint> points = pointLoader.loadPoints(width, height);

                Route route = new Route(points);
                points = route.calculateRoute(); // convert all given points to calculated route
                drawRoute(points, canvas);

                canvas.DrawPoint(points[points.Count - 1].pointLocation, redStroke);
            }
        }

        // try to call only when loading new floor
        // (currently the same static image)
        // try to call only when loading new floor
        // (currently the same static image)
        private void setFloorPlan(string file)
        {
            // Bitmap
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            String resourceId = "S_Nav.Media.Images." + file;
            using (Stream stream = assembly.GetManifestResourceStream(resourceId))
            {
                image = SKBitmap.Decode(stream);
            }
        }

        //
        // Takes a list of points and  draws lines between them
        //
        //private void drawRoute(List<MapPoint> points, SKCanvas canvas)
        //{
        //    for (int i = 0; i < points.Count - 1; i++) // count produces higher value than max index
        //    {
        //        if (i == 0)
        //        {
        //            canvas.DrawPoint(points[i].getPointLocation(), greenStroke);
        //        }
        //        else if (i < points.Count - 1) {
        //            canvas.DrawPoint(points[i].getPointLocation(), blackStroke); // not super necessary, illustrating where points were found
        //        }
        //        canvas.DrawLine(points[i].getPointLocation(), points[i + 1].getPointLocation(), routeColour);
        //    }
        //}
    }
}