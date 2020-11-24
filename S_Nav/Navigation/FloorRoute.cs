using System;
using System.Collections.Generic;
using SkiaSharp;

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
        private List<MapPoint> wingConnector;

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

            if(_givenPoints.Count > 4)
                wingConnector = _givenPoints[4];

            startPoint = FindPoint(currentLocation);
            endPoint = FindPoint(destinationLocation);
        }

        private MapPoint FindPoint (string loc)
        {
             MapPoint point = roomPoints.Find(i => i.GetPointName() == loc) 
                              ?? traversalPoints.Find(i => i.GetPointName() == loc) 
                              ?? wingConnector.Find(i => i.GetPointName() == loc);
            
            // for any more alternatives, follow above

            return point;
        }

        public List<MapPoint> CalculateRoute()
        {
            List<MapPoint> routePoints = new List<MapPoint>();

            // First point of route, immediately added
            routePoints.Add(startPoint);

            // Same floor room routing
            if (roomPoints.Contains(startPoint) && roomPoints.Contains(endPoint))
            {
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
            }

            // go to halls
            MapPoint firstHallPoint = GetNearestHallPoint(startPoint.GetPointLocation());
            routePoints.Add(firstHallPoint);

            AddRoutePoints(routePoints);

            // Destination point as last
            routePoints.Add(endPoint);

            return routePoints;
        }

        private void AddRoutePoints(List<MapPoint> routePoints)
        {
            // If end point is sub room of last added point in route, don't add any more
            if (roomPoints.Contains(endPoint) &&
                endPoint.GetPointName().Length > 4 &&
                endPoint.GetPointName().Substring(0, 4) == routePoints[routePoints.Count - 1].GetPointName())
                return;

            bool findMasterRoom = roomPoints.Contains(endPoint); // of end point

            // TODO: Stairs
            while (routePoints[routePoints.Count - 2] != routePoints[routePoints.Count - 1])
            {
                if (findMasterRoom && endPoint.GetPointName().Length > 4)
                {
                    MapPoint masterRoom = roomPoints.Find(i =>
                        i.GetPointName() == endPoint.GetPointName().Substring(0, 4));

                    while (routePoints[routePoints.Count - 2] != routePoints[routePoints.Count - 1])
                    {
                        routePoints.Add(GetNextPoint(
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
                routePoints.Add(GetNextPoint(
                    routePoints[routePoints.Count - 1],
                    endPoint));
            }
        }

        private MapPoint GetNearestHallPoint(SKPoint start)
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

        private MapPoint GetNextPoint(MapPoint currentMapPoint, MapPoint endMapPoint)
        {
            SKPoint curPointLoc = currentMapPoint.GetPointLocation();
            SKPoint endPointLoc = endMapPoint.GetPointLocation();
            MapPoint nextPoint = null;

            foreach (MapPoint p in hallwayPoints)
            {
                float dif = SKPoint.Distance(p.GetPointLocation(), endPointLoc);
                float cur = SKPoint.Distance(curPointLoc, endPointLoc);

                // Align check
                if (Math.Abs(curPointLoc.X - p.GetPointLocation().X) < 1
                    || Math.Abs(curPointLoc.Y - p.GetPointLocation().Y) < 1)
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

        // Unused. To be used for cross-floor routing to another floor
        private MapPoint GetNearestStairs(MapPoint curMapPoint)
        {
            MapPoint nearest = traversalPoints[0];
            float nearDist = 2f; // beyond max, which is sqrt(2)

            List<MapPoint> flrTrvPoints = traversalPoints.FindAll(a =>
            {
                string name = a.GetPointName();
                return name.StartsWith("stairs") || name.StartsWith("elevator");
            });

            foreach (MapPoint mp in flrTrvPoints)
            {
                float dist = SKPoint.Distance(
                curMapPoint.GetPointLocation(),
                mp.GetPointLocation());

                if (dist < nearDist)
                {
                    nearest = mp;
                    nearDist = dist;
                }
            }

            return nearest;
        }

        // Unused. To be used for cross-floor routing to another wing
        private MapPoint GetNearestWingHall(MapPoint curMapPoint)
        {
            List<MapPoint> wngTrvPoints = traversalPoints.FindAll(a => a.GetPointName().StartsWith("hall"));
            MapPoint nearest = wngTrvPoints[0];

            if (wngTrvPoints.Count == 1) // shortcut
                return nearest;

            float nearDist = 2f;

            foreach (MapPoint mp in wngTrvPoints)
            {
                if (mp.GetPointName().StartsWith("stairs") ||
                    mp.GetPointName().StartsWith("elevator"))
                {
                    float dist = SKPoint.Distance(
                    curMapPoint.GetPointLocation(),
                    mp.GetPointLocation());

                    if (dist < nearDist)
                    {
                        nearest = mp;
                        nearDist = dist;
                    }
                }

            }

            return nearest;
        }
    }
}
