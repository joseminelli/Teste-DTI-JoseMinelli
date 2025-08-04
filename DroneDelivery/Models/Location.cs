using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneDelivery.Models
{
    public class Location
    {
        public int PosX { get; set; }
        public int PosY{ get; set; }

        public Location(int x, int y)
        {
            PosX = x;
            PosY = y;
        }

        public double DistanceTo(Location newLoc)
        {
            int distX = PosX - newLoc.PosX;
            int distY = PosY - newLoc.PosY;
            return Math.Sqrt(distX * distX + distY * distY);
        }
    }
}
