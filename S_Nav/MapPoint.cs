using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace S_Nav
{
    class MapPoint
    {
        String pointName;
        String pointDescription; // placeholder for things like bathroom, office, etc.

        public SKPoint pointLocation; // remove public modifier after testing

        public String getPointName()
        {
            return this.pointName;
        }

        public MapPoint(String pName, SKPoint pLocation)
        {
            this.pointName = pName;
            this.pointLocation = pLocation;
        }

        public MapPoint(SKPoint pLocation)
        {
            this.pointLocation = pLocation;
        }

        public SKPoint getPointLocation()
        {
            return this.pointLocation;
        }
    }
}
