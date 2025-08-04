using DroneDelivery.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DroneDelivery.Services
{
    public class DeliveryReport
    {
        private int totalEntregas = 0;
        private Dictionary<string, double> pesoPorDrone = new Dictionary<string, double>();
        private List<Package> pacotesNaoEntregues = new List<Package> { };
        private Dictionary<string, double> distanciaPorDrone = new Dictionary<string, double>();

        public void RegistrarEntrega(string droneId, Package pacote, double distancia)
        {
            totalEntregas++;

            if (!pesoPorDrone.ContainsKey(droneId))
                pesoPorDrone[droneId] = 0;
            pesoPorDrone[droneId] += pacote.Weight;

            if (!distanciaPorDrone.ContainsKey(droneId))
                distanciaPorDrone[droneId] = 0;
            distanciaPorDrone[droneId] += distancia;
        }


        public void AdicionarNaoEntregue(Package pacote)
        {
            pacotesNaoEntregues.Add(pacote);
        }

        public void ExibirRelatorio()
        {
            Console.WriteLine("\n===== RELATÓRIO FINAL =====");
            Console.WriteLine($"Total de entregas realizadas: {totalEntregas}");

            if (pesoPorDrone.Any())
            {
                var maisEficiente = pesoPorDrone.OrderByDescending(p => p.Value).First();
                Console.WriteLine($"Drone mais eficiente: {maisEficiente.Key} (Total entregue: {maisEficiente.Value}kg)");
            }

            Console.WriteLine("\nDistância total por drone:");
            foreach (var d in distanciaPorDrone)
            {
                Console.WriteLine($"- {d.Key}: {d.Value:F2} km");
            }

            Console.WriteLine("\nPacotes não entregues:");
            if (pacotesNaoEntregues.Count == 0)
            {
                Console.WriteLine("Todos os pacotes foram entregues!");
            }
            else
            {
                foreach (var pacote in pacotesNaoEntregues)
                {
                    Console.WriteLine($"- Destino=({pacote.Destination.PosX}, {pacote.Destination.PosY}) Peso={pacote.Weight}kg Prioridade={pacote.DeliveryPriority}");
                }
            }
        }
    }
}
