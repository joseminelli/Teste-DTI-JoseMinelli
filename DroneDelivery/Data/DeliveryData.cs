using DroneDelivery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneDelivery.Data
{
    public static class DeliveryData
    {
        public static List<Drone> Drones { get; } = new List<Drone>();
        public static List<Package> Pacotes { get; } = new List<Package>();
        public static List<Location> ZonasProibidas { get; } = new List<Location>();

        public static void Reset()
        {
            Drones.Clear();
            Pacotes.Clear();
            ZonasProibidas.Clear();
        }
    }
}
