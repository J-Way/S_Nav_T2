using System;
using System.Collections.Generic;
using S_Nav.Models;

namespace S_Nav.Navigation
{
    class FloorPoint
    {
        public string wing { get; }
        public byte floor { get; }

        private List<FloorPoint> connections;

        public FloorPoint(string cWing, byte cFloor)
        {
            wing = cWing;
            floor = cFloor;
            connections = new List<FloorPoint>();
        }

        public FloorPoint(string floorName)
        {
            if (floorName.Length < 3 || floorName.Length > 4)
                throw new ArgumentException($"Floor name [{floorName}] is not 3-4 characters");

            string[] fn = floorName.Split('-');
            wing = fn[0];
            floor = Convert.ToByte(fn[1]);
            connections = new List<FloorPoint>();
        }

        public FloorPoint(Floor _floor)
        {
            string floorName = _floor.GetFloorName();
            if (floorName.Length < 3 || floorName.Length > 4)
                throw new ArgumentException($"Floor name [{floorName}] is not 3-4 characters");

            string[] fn = floorName.Split('-');
            wing = fn[0];
            floor = Convert.ToByte(fn[1]);
        }

        public String getName()
        {
            return wing + floor;
        }

        public String getFBName()
        {
            return $"TRA-{wing}-{floor}";
        }

        public List<FloorPoint> getConnections()
        {
            return connections;
        }

        public void addConnections(params FloorPoint[] fps)
        {
            foreach (FloorPoint fp in fps)
            {
                if (!connectsTo(fp))
                    connections.Add(fp);

                if (!fp.connectsTo(this))
                    fp.addConnections(this);
            }
        }

        public bool connectsTo(FloorPoint fp)
        {
            return connections.Contains(fp);
        }
    }
}
