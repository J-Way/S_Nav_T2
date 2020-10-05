using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace S_Nav
{
    class MapPoint
    {
        private string point_name { get; set; }
        private string pointDescription { get; set; }

        private SKPoint pointLocation { get; set; }

        public string getPointName()
        {
            return this.point_name;
        }

        public void setPointName(string name)
        {
            this.point_name = name;
        }

        public MapPoint(String pName, SKPoint pLocation)
        {
            this.point_name = pName;
            this.pointLocation = pLocation;
        }

        public MapPoint(SKPoint pLocation)
        {
            this.pointLocation = pLocation;
        }

        public MapPoint(String pName, float x, float y)
        {
            this.point_name = pName;
            this.pointLocation = new SKPoint(x, y);
        }

        public MapPoint()
        {
        }

        public SKPoint getPointLocation()
        {
            return this.pointLocation;
        }
    }
}
