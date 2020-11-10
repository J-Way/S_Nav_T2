using System;
using System.Collections.Generic;
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
            Queue<List<FloorPoint>> queue = new Queue<List<FloorPoint>>();
            List<string> visited = new List<string>();
            List<FloorPoint> initialPath = new List<FloorPoint>();
            initialPath.Add(startPoint);
            queue.Enqueue(initialPath);
            
            while (queue.Count > 0)
            {
                List<FloorPoint> path = queue.Dequeue();
                FloorPoint fp = path[path.Count - 1];

                if (fp == endPoint)
                    return path;
                
                if (!visited.Contains(fp.getName()))
                {
                    fp.getConnections().ForEach(c =>
                    {
                        List<FloorPoint> newPath = new List<FloorPoint>(path);
                        newPath.Add(c);
                        queue.Enqueue(newPath);
                    });

                    visited.Add(fp.getName());
                }
            }

            return null;
        }
    }
}
