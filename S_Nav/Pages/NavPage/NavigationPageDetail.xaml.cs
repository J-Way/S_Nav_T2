﻿using System;
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

        String currentLocation, destinationLocation;
        List<MapPoint> points;

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
            floorFile = "S_Nav.TRAE1.jpg";
            EWingButtonLayout();
        }

        public NavigationPageDetail(List<MapPoint> p, string file)
        {
            InitializeComponent();
            currentLocation = Preferences.Get("curLoc", null);

            points = null;
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

            setFloorPlan(floorFile);
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

        public NavigationPageDetail(string file)
        {
            InitializeComponent();
            currentLocation = Preferences.Get("curLoc", null);
            floorFile = file;
            if (floorFile.Equals("S_Nav.TRAG1.jpg") || floorFile.Equals("S_Nav.TRAG2.jpg")){
                GWingButtonLayout();
            } else if (floorFile.Equals("S_Nav.TRAE1.jpg") || floorFile.Equals("S_Nav.TRAE2.jpg"))
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
            if (floorFile.Equals("S_Nav.TRAG1.jpg"))
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
            
            if (floorFile.Equals("S_Nav.TRAE1.jpg"))
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


        private async void DownClicked(object sender, EventArgs e)
        {
            Console.WriteLine("Down Clicked");
            image.Reset();
            if (floorFile.Equals("S_Nav.TRAG2.jpg"))
            {
                showG1();
            }
            else if (floorFile.Equals("S_Nav.TRAE2.jpg"))
            {
                showE1();
            }
        }

        private async void UpClicked(object sender, EventArgs e)
        {
            Console.WriteLine("Up Clicked");
            image.Reset();
            if (floorFile.Equals("S_Nav.TRAG1.jpg"))
            {
                showG2();
            } else if (floorFile.Equals("S_Nav.TRAE1.jpg"))
            {
                showE2();
            }  
        }

        private async void WingLeftClicked(object sender, EventArgs e)
        {
            image.Reset();
            if (floorFile.Equals("S_Nav.TRAE1.jpg"))
            {
                showG1();
            }
            else if (floorFile.Equals("S_Nav.TRAE2.jpg"))
            {
                showG2();
            }
        }

        private async void WingRightClicked(object sender, EventArgs e)
        {
            image.Reset();
            if (floorFile.Equals("S_Nav.TRAG1.jpg"))
            {
                showE1();
            }
            else if (floorFile.Equals("S_Nav.TRAG2.jpg"))
            {
                showE2();
            }
        }

        private async void showE1()
        {
           
            setFloorPlan("S_Nav.TRAE1.jpg");
            NavigationPage routePage = new NavigationPage("S_Nav.TRAE1.jpg");
            await Navigation.PushModalAsync(routePage);
        }

        private async void showE2()
        {
            FloorUpButton.IsVisible = false;
            setFloorPlan("S_Nav.TRAE2.jpg");
            NavigationPage routePage = new NavigationPage("S_Nav.TRAE2.jpg");
            await Navigation.PushModalAsync(routePage);
        }

        private async void showG1()
        {
            FloorDownButton.IsVisible = false;
            setFloorPlan("S_Nav.TRAG1.jpg");
            NavigationPage routePage = new NavigationPage("S_Nav.TRAG1.jpg");
            await Navigation.PushModalAsync(routePage);
        }

        private async void showG2()
        {
            FloorUpButton.IsVisible = false;
            setFloorPlan("S_Nav.TRAG2.jpg");
            NavigationPage routePage = new NavigationPage("S_Nav.TRAG2.jpg");
            await Navigation.PushModalAsync(routePage);
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