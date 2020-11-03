using System;
using System.Collections.Generic;
using System.Text;

namespace S_Nav.Navigation
{
    class FloorPoint
    {
        private string wing;
        private byte floor;
        private List<FloorPoint> connections;

        public FloorPoint(string cWing, byte cFloor)
        {
            wing = cWing;
            floor = cFloor;
        }

        public FloorPoint(string floorName)
        {
            if (floorName.Length < 2 || floorName.Length > 3)
                throw new ArgumentException($"Floor name [{floorName}] is not 2-3 characters");
            wing = floorName.Substring(0, floorName.Length - 1);
            floor = Convert.ToByte(floorName[floorName.Length - 1]);
        }

        public String getName()
        {
            return wing.ToString() + floor;
        }

        public List<FloorPoint> getConnections()
        {
            return connections;
        }

        public void addConnections(params FloorPoint[] fps)
        {
            foreach (FloorPoint fp in fps)
            {
                connections.Add(fp);
                if (!fp.connectsTo(this))
                {
                    fp.addConnections(this);
                }
            }
        }

        public bool connectsTo(FloorPoint fp)
        {
            return connections.Contains(fp);
        }
    }
}
