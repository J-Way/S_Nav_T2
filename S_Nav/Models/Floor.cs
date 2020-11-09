using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace S_Nav.Models
{
    class Floor
    {
        private string floorName;
        private List<string> rooms;

        public Floor(string name, string[] room)
        {
            this.floorName = name;
            this.rooms = room.ToList();
        }

        public string GetFloorName()
        {
            return this.floorName;
        }

        public List<string> GetRooms()
        {
            return this.rooms;
        }
    }
}
