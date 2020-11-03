using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace S_Nav.Navigation
{
    class CrossWingRoute
    {
        private FloorPoint startPoint;
        private FloorPoint endPoint;

        private List<FloorPoint> floorPoints;

        public CrossWingRoute(List<FloorPoint> _floorPoints)
        {
            floorPoints = _floorPoints;

            String curLoc = Preferences.Get("curLoc", null);
            String destLoc = Preferences.Get("destLoc", null);

            startPoint = floorPoints.Find(p =>
                p.getName() == curLoc.Substring(0, 2));
            endPoint = floorPoints.Find(p =>
                p.getName() == destLoc.Substring(0, 2));
        }

        public List<FloorPoint> calculateRoute()
        {
            List<FloorPoint> route = new List<FloorPoint>();

            // BFS
            Queue<FloorPoint> queue = new Queue<FloorPoint>();
            List<String> discovered = new List<string>();
            discovered.Add(startPoint.getName());
            queue.Enqueue(startPoint);
            route.Add(startPoint);
            while (queue.Count > 0)
            {
                FloorPoint fp = queue.Dequeue();

                if (fp == endPoint)
                {
                    route.Add(fp);
                    return route;
                }
                    
                fp.getConnections().ForEach(c =>
                {
                    if (!discovered.Contains(c.getName()))
                    {
                        discovered.Add(c.getName());
                        queue.Enqueue(c);
                    }
                });
            }
            // BFS END

            return route;
        }
    }
}
