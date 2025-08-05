using NUnit.Framework;
using DroneDelivery.Models;
using DroneDelivery.Services;
using DroneDelivery.Utils;
using DroneDelivery.Data;
using System.Collections.Generic;

namespace DroneDelivery.Tests
{
    public class DeliveryManagerTests
    {
        [Test]
        public void ProcessDeliveries_PacoteComPesoZero_NaoDeveEntregar()
        {
            var drones = new List<Drone> { new Drone("DR-1", 10, 30) };
            var pacotes = new List<Package> { new Package(new Location(5, 5), 0.0, Priority.Alta) };
            var zonasProibidas = new List<Location>();

            var manager = new DeliveryManager(drones, pacotes, zonasProibidas);
            var report = manager.ProcessDeliveries();

            Assert.That(report.GetTotalEntregas, Is.EqualTo(0));
        }

        [Test]
        public void ProcessDeliveries_PacoteEmZonaProibida_NaoDeveEntregar()
        {
            var drones = new List<Drone> { new Drone("DR-1", 10, 30) };
            var pacotes = new List<Package> { new Package(new Location(9, 9), 2.0, Priority.Media) };
            var zonasProibidas = new List<Location> { new Location(9, 9) };

            var manager = new DeliveryManager(drones, pacotes, zonasProibidas);
            var report = manager.ProcessDeliveries();

            Assert.That(report.GetTotalEntregas, Is.EqualTo(0));
        }

        [Test]
        public void ProcessDeliveries_DroneRecarregaParaEntregarTodosPacotes()
        {
            // Drone com bateria limitada para só uma viagem, mas deve recarregar para entregar todos
            var drone = new Drone("DR-1", 5, 100);
            drone.Battery = 10; // bateria baixa inicialmente

            var drones = new List<Drone> { drone };
            var pacotes = new List<Package>
            {
                new Package(new Location(10, 0), 3.0, Priority.Alta),
                new Package(new Location(10, 0), 3.0, Priority.Media)
            };

            var zonasProibidas = new List<Location>();

            var manager = new DeliveryManager(drones, pacotes, zonasProibidas);
            var report = manager.ProcessDeliveries();

            // Todos pacotes devem ser entregues em duas viagens, com recarga entre elas
            Assert.That(report.GetTotalEntregas, Is.EqualTo(2));
        }

        [Test]
        public void ProcessDeliveries_PacotesSuperamCapacidade_EntreguesEmMultiplasViagens()
        {
            var drones = new List<Drone> { new Drone("DR-1", 5, 100) };
            var pacotes = new List<Package>
            {
                new Package(new Location(10, 0), 3.0, Priority.Alta),
                new Package(new Location(10, 0), 3.0, Priority.Baixa)
            };

            var zonasProibidas = new List<Location>();

            var manager = new DeliveryManager(drones, pacotes, zonasProibidas);
            var report = manager.ProcessDeliveries();

            // Mesmo superando capacidade, ambos pacotes devem ser entregues em viagens separadas
            Assert.That(report.GetTotalEntregas, Is.EqualTo(2));
        }

        [Test]
        public void ProcessDeliveries_PacotesComDistanciaMaiorQueAlcance_NaoSaoEntregues()
        {
            var drones = new List<Drone> { new Drone("DR-1", 10, 10) };
            var pacotes = new List<Package>
            {
                new Package(new Location(20, 0), 2.0, Priority.Alta)
            };

            var zonasProibidas = new List<Location>();

            var manager = new DeliveryManager(drones, pacotes, zonasProibidas);
            var report = manager.ProcessDeliveries();

            Assert.That(report.GetTotalEntregas, Is.EqualTo(0));
        }

    }
}
