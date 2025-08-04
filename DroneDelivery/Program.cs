using DroneDelivery.Models;
using DroneDelivery.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneDelivery
{
    class Program
    {
        static void Main(string[] args)
        {
            var manager = new DeliveryManager();


            manager.AddDrone(new Drone("DR-01", 10.0, 30.0));
            manager.AddDrone(new Drone("DR-02", 6.0, 20.0));

            manager.AddPackage(new Package(new Location(5, 5), 2.5, Priority.Alta));
            manager.AddPackage(new Package(new Location(10, 10), 3.0, Priority.Media));
            manager.AddPackage(new Package(new Location(3, 3), 1.0, Priority.Baixa));
            manager.AddPackage(new Package(new Location(7, 2), 5.0, Priority.Alta));

            manager.ProcessDeliveries();

            Console.ReadLine();
        }
    }
}
