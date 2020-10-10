using System;
using System.Numerics;

namespace S_Nav
{
    public class MapPoint
    {
        private string point_name { get; set; }
        private string pointDescription { get; set; }

        private Vector2 pointLocation { get; set; }

        public string getPointName()
        {
            return this.point_name;
        }

        public void setPointName(string name)
        {
            this.point_name = name;
        }

        public MapPoint(String pName, Vector2 pLocation)
        {
            this.point_name = pName;
            this.pointLocation = pLocation;
        }

        public MapPoint(Vector2 pLocation)
        {
            this.pointLocation = pLocation;
        }

        public MapPoint(String pName, float x, float y)
        {
            this.point_name = pName;
            this.pointLocation = new Vector2(x, y);
        }

        public MapPoint()
        {
        }

        public Vector2 getPointLocation()
        {
            return this.pointLocation;
        }


        public int getPointX(float width)
        {
            return (int) (this.pointLocation.X * width);
        }
        public int getPointY(float height)
        {
            return (int) (this.pointLocation.Y * height);
        }

        public void setPointY(float y)
        {
            this.pointLocation = new Vector2(this.pointLocation.X, y);
        }

        public void setPointX(float x)
        {
            this.pointLocation =new Vector2(x, this.pointLocation.Y);
        }
    }
}
