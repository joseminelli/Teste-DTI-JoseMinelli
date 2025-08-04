using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneDelivery.Models
{
    public enum Priority { Baixa, Media, Alta }

    public class Package
    {
        public Location Destination { get; set; }
        public double Weight { get; set; }
        public Priority DeliveryPriority { get; set; }

        public Package(Location destination, double weight, Priority priority)
        {
            Destination = destination;
            Weight = weight;
            DeliveryPriority = priority;
        }
    }
}
