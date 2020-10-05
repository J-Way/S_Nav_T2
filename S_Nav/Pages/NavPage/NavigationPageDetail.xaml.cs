using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using SkiaSharp;
using SkiaSharp.Views.Forms;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using S_Nav.Firebase;
using System.Net.Http;
using System.Threading.Tasks;

namespace S_Nav
{

    [DesignTimeVisible(true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NavigationPageDetail : ContentPage
    {
        FirebaseConnection firebaseConnection = new FirebaseConnection();
        HttpClient httpClient = new HttpClient();
        String currentLocation, destinationLocation;

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

        SKBitmap image;

        // creates detail page
        // dont touch this
        public NavigationPageDetail()
        {
            InitializeComponent();
            currentLocation = Preferences.Get("curLoc", null);
            destinationLocation = Preferences.Get("destLoc", null);
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

            canvas.Scale(1, 1);

            //image = await SetFloorPlan(width,height,canvas);
            //    //canvas.DrawBitmap(image, new SKRect(0, 0, width, height));

            canvas.Save();

            // Calls routing
            if (currentLocation != null)
            {
                List<MapPoint> points = await firebaseConnection.GetFloorPoints("TRAE2", width, height);
                LoadPoints pointLoader = new LoadPoints();
                List<MapPoint> points2 = pointLoader.loadPoints(width, height);

                

                foreach (var point in points)
                {
                    SKPoint t = point.getPointLocation();
                    t.X = (int)t.X;
                    t.Y = (int)t.Y;
                    canvas.DrawPoint(t, redStroke);
                }
            }
            
        }


        // try to call only when loading new floor
        // (currently the same static image)
        private async Task<SKBitmap> SetFloorPlan(int width, int height, SKCanvas canvas)
        {
            //
            var placeholderInput = "TRAE2";
            // might need to make this function async if given await error
            Uri image_uri = await firebaseConnection.GetImage(placeholderInput);

            string url = image_uri.ToString();
            try
            {
                using (Stream stream = await httpClient.GetStreamAsync(url))
                using (MemoryStream memStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memStream);
                    memStream.Seek(0, SeekOrigin.Begin);

                    image = SKBitmap.Decode(memStream);

                    return image;
                };
            }
            catch
            {
            }

            return null;
            //


            // Bitmap
            //Assembly assembly = GetType().GetTypeInfo().Assembly;
            //using (Stream stream = assembly.GetManifestResourceStream(image_uri.ToString()))
            //{
            //    image = SKBitmap.Decode(stream);
            //}
        }

        private List<MapPoint> calculateRoute(List<MapPoint> givenPoints)
        {
            List<MapPoint> routePoints = new List<MapPoint>();
            routePoints.Add(givenPoints.Find(i => i.getPointName() == currentLocation));
            MapPoint endPoint = givenPoints.Find(i => i.getPointName() == destinationLocation);
            // since we navigate by hall points, find all of them
            List<MapPoint> hallPoints = new List<MapPoint>();
            foreach(MapPoint p in givenPoints)
            {
                if (p.getPointName().Contains("hall"))
                {
                    hallPoints.Add(p);
                }
            }

            bool findMasterRoom = true;

            if (currentLocation.Length <= 4 || destinationLocation.Length <= 4)
            {
                // set the first map point
                MapPoint firstHallPoint = new MapPoint(new SKPoint(0, 0));
                firstHallPoint = getFirstHallPoint(firstHallPoint, routePoints[0].getPointLocation().X,
                                 routePoints[0].getPointLocation().Y, hallPoints);

                routePoints.Add(firstHallPoint);
                
                while (routePoints[routePoints.Count - 2] != routePoints[routePoints.Count - 1])
                {
                    // handle navigating to rooms which contain multiple subrooms
                    if (findMasterRoom && endPoint.getPointName().Length > 4)
                    {
                        MapPoint masterRoom = givenPoints.Find(i => i.getPointName() == endPoint.getPointName().Substring(0, 4));
                        while (routePoints[routePoints.Count - 2] != routePoints[routePoints.Count - 1])
                        {
                            routePoints.Add(getNextPoint(routePoints[routePoints.Count - 1], masterRoom, hallPoints));
                        }
                        routePoints.Add(masterRoom);
                    }
                    else
                    {
                        findMasterRoom = false;
                    }
                    routePoints.Add(getNextPoint(routePoints[routePoints.Count - 1], endPoint, hallPoints));
                }
            }
            
            else
            {
                routePoints.Add(givenPoints.Find(i => i.getPointName() == endPoint.getPointName().Substring(0, 4)));
            }
            routePoints.Add(endPoint);
            
            return routePoints;
        }

        // 
        // Finds the hall point closest to start point
        //
        MapPoint getFirstHallPoint(MapPoint nextPoint, float startX, float startY, List<MapPoint> hallPoints)
        {
            float difX, difY, curX, curY;

            foreach (MapPoint p in hallPoints)
            {
                // get difference
                difX = p.getPointLocation().X - startX;
                difY = p.getPointLocation().Y - startY;

                curX = nextPoint.getPointLocation().X - startX;
                curY = nextPoint.getPointLocation().Y - startY;

                // only want positive values
                if (difX < 0)
                    difX *= -1;
                if (difY < 0)
                    difY *= -1;
                if (curX < 0)
                    curX *= -1;
                if (curY < 0)
                    curY *= -1;

                // maybe replace with OR
                //      might have instance where X is closer, Y is farther
                if (difX <= curX && difY <= curY)
                {
                    nextPoint = p;
                }
            }
            hallPoints.Remove(nextPoint);
            return nextPoint;
        }

        // not currently accounting for curved halls
        MapPoint getNextPoint(MapPoint currentPoint, MapPoint endPoint, List<MapPoint> hallPoints)
        {
            float difX, difY, curX, curY;

            MapPoint nextPoint = null;

            foreach (MapPoint p in hallPoints)
            {
                difX = p.getPointLocation().X - endPoint.getPointLocation().X;
                difY = p.getPointLocation().Y - endPoint.getPointLocation().Y;

                curX = currentPoint.getPointLocation().X - endPoint.getPointLocation().X;
                curY = currentPoint.getPointLocation().Y - endPoint.getPointLocation().Y;

                // only want positive values
                if (difX < 0)
                    difX *= -1;
                if (difY < 0)
                    difY *= -1;
                if (curX < 0)
                    curX *= -1;
                if (curY < 0)
                    curY *= -1;

                if (currentPoint.getPointLocation().X == p.getPointLocation().X
                    || currentPoint.getPointLocation().Y == p.getPointLocation().Y)
                {
                    if (difX < curX || difY < curY)
                    {
                        nextPoint = p;
                        hallPoints.Remove(p);
                        return nextPoint;
                    }
                    else if (difX == curX && difY == curY)
                    {
                        nextPoint = p;
                        hallPoints.Remove(p);
                        return nextPoint;
                    }
                }
            }

            // failsafe return
            return currentPoint;
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