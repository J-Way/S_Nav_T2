using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using SkiaSharp;
using SkiaSharp.Views.Forms;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Reflection;

namespace S_Nav
{

    [DesignTimeVisible(true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NavigationPageDetail : ContentPage
    {

        string currentLocation, destinationLocation;

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

            // Calls routing
            if (currentLocation != null)
            {
                //LoadPoints pointLoader = new LoadPoints();
                //List<MapPoint> points = pointLoader.loadPoints(width, height);
                //
                //points = calculateRoute(points);
                //drawRoute(points, canvas);
                //
                //canvas.DrawPoint(points[points.Count - 1].getPointLocation(), redStroke);
            }
            canvas.Save();

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



        private List<MapPoint> CalculateRoute(List<MapPoint> givenPoints)
        {
            List<MapPoint> routePoints = new List<MapPoint>
            {
                givenPoints.Find(i => i.GetPointName() == currentLocation)
            };
            MapPoint endPoint = givenPoints.Find(i => i.GetPointName() == destinationLocation);
            // since we navigate by hall points, find all of them
            List<MapPoint> hallPoints = new List<MapPoint>();
            foreach(MapPoint p in givenPoints)
            {
                if (p.GetPointName().Contains("hall"))
                {
                    hallPoints.Add(p);
                }
            }

            bool findMasterRoom = true;

            if (currentLocation.Length <= 4 || destinationLocation.Length <= 4)
            {
                // set the first map point
                MapPoint firstHallPoint = new MapPoint(new SKPoint(0, 0));
                firstHallPoint = GetFirstHallPoint(firstHallPoint, routePoints[0].GetPointLocation().X,
                                 routePoints[0].GetPointLocation().Y, hallPoints);

                routePoints.Add(firstHallPoint);
                
                while (routePoints[routePoints.Count - 2] != routePoints[routePoints.Count - 1])
                {
                    // handle navigating to rooms which contain multiple subrooms
                    if (findMasterRoom && endPoint.GetPointName().Length > 4)
                    {
                        MapPoint masterRoom = givenPoints.Find(i => i.GetPointName() == endPoint.GetPointName().Substring(0, 4));
                        while (routePoints[routePoints.Count - 2] != routePoints[routePoints.Count - 1])
                        {
                            routePoints.Add(GetNextPoint(routePoints[routePoints.Count - 1], masterRoom, hallPoints));
                        }
                        routePoints.Add(masterRoom);
                    }
                    else
                    {
                        findMasterRoom = false;
                    }
                    routePoints.Add(GetNextPoint(routePoints[routePoints.Count - 1], endPoint, hallPoints));
                }
            }
            
            else
            {
                routePoints.Add(givenPoints.Find(i => i.GetPointName() == endPoint.GetPointName().Substring(0, 4)));
            }
            routePoints.Add(endPoint);
            
            return routePoints;
        }

        // 
        // Finds the hall point closest to start point
        //
        MapPoint GetFirstHallPoint(MapPoint nextPoint, float startX, float startY, List<MapPoint> hallPoints)
        {
            float difX, difY, curX, curY;

            foreach (MapPoint p in hallPoints)
            {
                // get difference
                difX = p.GetPointLocation().X - startX;
                difY = p.GetPointLocation().Y - startY;

                curX = nextPoint.GetPointLocation().X - startX;
                curY = nextPoint.GetPointLocation().Y - startY;

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
        MapPoint GetNextPoint(MapPoint currentPoint, MapPoint endPoint, List<MapPoint> hallPoints)
        {
            float difX, difY, curX, curY;
            foreach (MapPoint p in hallPoints)
            {
                difX = p.GetPointLocation().X - endPoint.GetPointLocation().X;
                difY = p.GetPointLocation().Y - endPoint.GetPointLocation().Y;

                curX = currentPoint.GetPointLocation().X - endPoint.GetPointLocation().X;
                curY = currentPoint.GetPointLocation().Y - endPoint.GetPointLocation().Y;

                // only want positive values
                if (difX < 0)
                    difX *= -1;
                if (difY < 0)
                    difY *= -1;
                if (curX < 0)
                    curX *= -1;
                if (curY < 0)
                    curY *= -1;

                if (currentPoint.GetPointLocation().X == p.GetPointLocation().X
                    || currentPoint.GetPointLocation().Y == p.GetPointLocation().Y)
                {

                    MapPoint nextPoint;
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