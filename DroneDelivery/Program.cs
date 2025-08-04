using DroneDelivery.Models;
using DroneDelivery.Services;
using DroneDelivery.Data;
using DroneDelivery.Utils;
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

            DeliveryData.Drones.Add(new Drone("DR-01", 10.0, 30.0));
            DeliveryData.Drones.Add(new Drone("DR-02", 6.0, 20.0));

            DeliveryData.ZonasProibidas.Add(new Location(6, 6));
            DeliveryData.ZonasProibidas.Add(new Location(9, 9));

            DeliveryData.Pacotes.Add(new Package(new Location(5, 5), 2.5, Priority.Alta));
            DeliveryData.Pacotes.Add(new Package(new Location(10, 10), 3.0, Priority.Media));
            DeliveryData.Pacotes.Add(new Package(new Location(3, 3), 1.0, Priority.Baixa));
            DeliveryData.Pacotes.Add(new Package(new Location(7, 2), 5.0, Priority.Alta));

            manager.ProcessDeliveries();

            ConsoleHelper.Encerrar("Simulação encerrada. Pressione ENTER para sair.");
        }
    }
}
