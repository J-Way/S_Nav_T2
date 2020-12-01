using SkiaSharp;

namespace S_Nav
{
    public class MapPoint
    {
        private string point_name { get; set; }
        private string pointDescription { get; set; }
        private bool isAccessible { get; set; }
        private SKPoint pointLocation { get; set; }

        public string GetPointName()
        {
            return this.point_name;
        }

        public void SetPointName(string name)
        {
            this.point_name = name;
        }

        // Unused
        /*
        public MapPoint(string pName, SKPoint pLocation)
        {
            this.point_name = pName;
            this.pointLocation = pLocation;
        }
        */

        public MapPoint(SKPoint pLocation)
        {
            this.pointLocation = pLocation;
        }

        public MapPoint(string pName, float x, float y) 
            : this(new SKPoint(x, y))
        {
            this.point_name = pName;
        }

        public MapPoint(string pName, float x, float y, bool accessible) 
            : this(pName, x, y)
        {
            this.isAccessible = accessible;
        }

        // Unused
        /*
        public MapPoint(string name)
        {
            this.point_name = name;
        }
        */

        // Unused
        /*
        public MapPoint()
        {
        }
        */


        public SKPoint GetPointLocation()
        {
            return this.pointLocation;
        }


        public int GetPointX(float width)
        {
            return (int) (this.pointLocation.X * width);
        }
        public int GetPointY(float height)
        {
            return (int) (this.pointLocation.Y * height);
        }

        public void SetPointY(float y)
        {
            this.pointLocation = new SKPoint(this.pointLocation.X, y);
        }

        public void SetPointX(float x)
        {
            this.pointLocation =new SKPoint(x, this.pointLocation.Y);
        }

        public bool getAccess() { return this.isAccessible; }
    }
}
