﻿using System;
using System.Collections.Generic;
using System.Text;

namespace S_Nav.Navigation
{
    class TestLoadFloorPoints
    {
        public static List<FloorPoint> LoadTestFloorPoints()
        {
            List<FloorPoint> points = new List<FloorPoint>();

            FloorPoint g1 = new FloorPoint("G-1");
            FloorPoint g2 = new FloorPoint("G-2");
            FloorPoint g3 = new FloorPoint("G-3");
            FloorPoint g4 = new FloorPoint("G-4");

            FloorPoint e1 = new FloorPoint("E-1");
            FloorPoint e2 = new FloorPoint("E-2");

            g2.addConnections(g1, g3, e2);
            g4.addConnections(g3);
            e1.addConnections(e2);

            points.Add(g1);
            points.Add(g2);
            points.Add(g3);
            points.Add(g4);

            points.Add(e1);
            points.Add(e2);

            return points;
        }
    }
}
