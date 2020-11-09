using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using Xamarin.Essentials;

namespace S_Nav.Navigation
{
    class FloorRoute
    {
        public String currentLocation { get; }
        public String destinationLocation { get; }

        private List<MapPoint> utilityPoints;
        private List<MapPoint> roomPoints;
        private List<MapPoint> traversalPoints;
        private List<MapPoint> hallwayPoints;

        private MapPoint startPoint;
        private MapPoint endPoint;

        public FloorRoute(List<List<MapPoint>> _givenPoints, string curLoc, string destLoc)
        {
            currentLocation = curLoc;
            destinationLocation = destLoc;

            utilityPoints = _givenPoints[0];
            roomPoints = _givenPoints[1];
            traversalPoints = _givenPoints[2];
            hallwayPoints = _givenPoints[3];

            startPoint = roomPoints.Find(i => i.GetPointName() == currentLocation);
            endPoint = roomPoints.Find(i => i.GetPointName() == destinationLocation);
        }

        public List<MapPoint> calculateRoute()
        {
            List<MapPoint> routePoints = new List<MapPoint>();

            // First point of route, immediately added
            routePoints.Add(startPoint);

            // If either start or end is not a sub-room
            if (currentLocation.Length > 4)
            {
                // Master room of first point
                MapPoint masterOfFirstPoint =
                    roomPoints.Find(i => i.GetPointName() == startPoint.GetPointName().Substring(0, 4));
                routePoints.Add(masterOfFirstPoint);

                // single line route. covers adjacent sub-room -> master room edge case
                if (masterOfFirstPoint.GetPointName() == endPoint.GetPointName())
                    return routePoints;
                
            }
            else if (currentLocation == endPoint.GetPointName().Substring(0, 4))
            {
                // alternative single line route. covers master room -> adjacent sub-room edge case
                routePoints.Add(endPoint);
                return routePoints;
            }
            
            // go to halls
            MapPoint firstHallPoint = getNearestHallPoint(startPoint.GetPointLocation());
            routePoints.Add(firstHallPoint);

            addRoutePoints(routePoints);

            // Destination point as last
            routePoints.Add(endPoint);

            return routePoints;
        }

        private void addRoutePoints(List<MapPoint> routePoints)
        {
            // If end point is sub room of last added point in route, don't add any more
            if (endPoint.GetPointName().Length > 4 &&
                endPoint.GetPointName().Substring(0, 4) == routePoints[routePoints.Count - 1].GetPointName())
                return;

            bool findMasterRoom = true; // of end point

            // TODO: Stairs
            while (routePoints[routePoints.Count - 2] != routePoints[routePoints.Count - 1])
            {
                if (findMasterRoom && endPoint.GetPointName().Length > 4)
                {
                    MapPoint masterRoom = roomPoints.Find(i =>
                        i.GetPointName() == endPoint.GetPointName().Substring(0, 4));

                    while (routePoints[routePoints.Count - 2] != routePoints[routePoints.Count - 1])
                    {
                        routePoints.Add(getNextPoint(
                            routePoints[routePoints.Count - 1],
                            masterRoom)
                        );
                    }
                    routePoints.Add(masterRoom);
                }
                else
                {
                    findMasterRoom = false;
                }
                routePoints.Add(getNextPoint(
                    routePoints[routePoints.Count - 1],
                    endPoint));
            }
        }

        private MapPoint getNearestHallPoint(SKPoint start)
        {
            MapPoint nearestPoint = new MapPoint(new SKPoint(0, 0));

            foreach (MapPoint p in hallwayPoints)
            {
                // Dist of hall point to start
                float dif = SKPoint.Distance(p.GetPointLocation(), start);

                // Dist of nearest point to start
                float cur = SKPoint.Distance(nearestPoint.GetPointLocation(), start);

                if (dif <= cur)
                    nearestPoint = p;
            }

            // hallwayPoints.Remove(nearestPoint); // too far, don't touch again
            return nearestPoint;
        }

        private MapPoint getNextPoint(MapPoint currentMapPoint, MapPoint endMapPoint)
        {
            SKPoint currentPoint = currentMapPoint.GetPointLocation();
            SKPoint endPoint = endMapPoint.GetPointLocation();
            MapPoint nextPoint = null;

            foreach (MapPoint p in hallwayPoints)
            {
                float dif = SKPoint.Distance(p.GetPointLocation(), endPoint);
                float cur = SKPoint.Distance(currentPoint, endPoint);

                // Align check
                if (Math.Abs(currentPoint.X - p.GetPointLocation().X) < 1
                    || Math.Abs(currentPoint.Y - p.GetPointLocation().Y) < 1)
                {
                    if (dif <= cur)
                    {
                        nextPoint = p;
                        hallwayPoints.Remove(p);
                        return nextPoint;
                    }
                }
                
                // TODO: else if curved hall check to next hall point
            }

            return currentMapPoint;
        }
    }
}
