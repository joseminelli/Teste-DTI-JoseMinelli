using DroneDelivery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneDelivery.Services
{
    public class DeliveryManager
    {
        private List<Drone> drones = new List<Drone>();
        private List<Package> packages = new List<Package>();

        public void AddDrone(Drone drone) => drones.Add(drone);
        public void AddPackage(Package package) => packages.Add(package);

        public void ProcessDeliveries()
        {
            Console.WriteLine("===== Iniciando simulação de entregas =====");

            // Ordenação por prioridade
            var sortedPackages = packages
                .OrderByDescending(p => p.DeliveryPriority)
                .ToList();

            foreach (var drone in drones)
            {
                Console.WriteLine($"\n[Drone {drone.Id}] Iniciando entregas...");

                double capacidadeRestante = drone.Capacity;
                double alcanceRestante = drone.MaxDistance;
                var pacotesSelecionados = new List<Package>();

                // adiciona pacotes que encaixem no limite de distancia(ida de volta) e de peso
                foreach (var pacote in sortedPackages.ToList())
                {
                    double distancia = drone.Position.DistanceTo(pacote.Destination) * 2;

                    if (pacote.Weight <= capacidadeRestante && distancia <= alcanceRestante)
                    {
                        pacotesSelecionados.Add(pacote);
                        capacidadeRestante -= pacote.Weight;
                        alcanceRestante -= distancia;
                        sortedPackages.Remove(pacote);
                    }
                }

                if (pacotesSelecionados.Count == 0)
                {
                    Console.WriteLine("Nenhum pacote foi atribuído a este drone.");
                    continue;
                }

                foreach (var pacote in pacotesSelecionados)
                {
                    Console.WriteLine($"Entregando pacote para ({pacote.Destination.PosX}, {pacote.Destination.PosY}) - Peso: {pacote.Weight}kg - Prioridade: {pacote.DeliveryPriority}");
                }

                Console.WriteLine($"[Drone {drone.Id}] Retornando à base.");
            }

            Console.WriteLine("\n===== Entregas finalizadas =====");

            Console.WriteLine($"Pacotes restantes: {sortedPackages.Count}");

            foreach (var pacote in sortedPackages)
            {
                Console.WriteLine($"Pacote NÃO entregue: Destino=({pacote.Destination.PosX}, {pacote.Destination.PosY}) Peso={pacote.Weight}kg Prioridade={pacote.DeliveryPriority}");
            }


        }

    }
}
