using DroneDelivery.Models;
using DroneDelivery.Utils;
using DroneDelivery.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DroneDelivery.Services
{
    public class DeliveryReport
    {
        private int totalEntregas = 0;
        private readonly Dictionary<string, double> pesoPorDrone = new Dictionary<string, double>();
        private readonly Dictionary<string, double> distanciaPorDrone = new Dictionary<string, double>();
        private readonly Dictionary<string, int> entregasPorDrone = new Dictionary<string, int>();
        private readonly List<Package> pacotesNaoEntregues = new List<Package>();

        public void RegistrarEntrega(string droneId, Package pacote, double distancia)
        {
            totalEntregas++;

            if (!pesoPorDrone.ContainsKey(droneId))
                pesoPorDrone[droneId] = 0;
            pesoPorDrone[droneId] += pacote.Weight;

            if (!distanciaPorDrone.ContainsKey(droneId))
                distanciaPorDrone[droneId] = 0;
            distanciaPorDrone[droneId] += distancia;

            if (!entregasPorDrone.ContainsKey(droneId))
                entregasPorDrone[droneId] = 1;
            else
                entregasPorDrone[droneId]++;

        }

        public void AdicionarNaoEntregue(Package pacote)
        {
            pacotesNaoEntregues.Add(pacote);
        }

        public void ExibirRelatorio()
        {
            ConsoleHelper.Info("\n===== RELATÓRIO FINAL =====");

            ConsoleHelper.Sucesso($"Total de entregas realizadas: {totalEntregas}");

            if (pesoPorDrone.Any())
            {
                var maisEficiente = pesoPorDrone.OrderByDescending(p => p.Value).First();
                ConsoleHelper.Sucesso($"Drone mais eficiente: {maisEficiente.Key} (Total entregue: {maisEficiente.Value}kg)");
            }

            // Estatísticas por drone
            ConsoleHelper.Info("\n--- Estatísticas por Drone ---");
            foreach (var drone in DeliveryData.Drones.OrderBy(d => d.Id))
            {
                string id = drone.Id;
                double peso = pesoPorDrone.ContainsKey(id) ? pesoPorDrone[id] : 0;
                double distancia = distanciaPorDrone.ContainsKey(id) ? distanciaPorDrone[id] : 0;
                int entregas = entregasPorDrone.ContainsKey(id) ? entregasPorDrone[id] : 0;

                double bateria = drone.Battery;

                Console.WriteLine($"Drone: {id}");
                Console.WriteLine($"  - Pacotes entregues: {entregas}");
                Console.WriteLine($"  - Peso total entregue: {peso}kg");
                Console.WriteLine($"  - Distância total percorrida: {distancia:F2} km");
                Console.WriteLine($"  - Bateria restante: {bateria:F1}%");
            }

            // Pacotes não entregues
            ConsoleHelper.Erro("\n--- Pacotes NÃO entregues ---");
            if (pacotesNaoEntregues.Count == 0)
            {
                ConsoleHelper.Sucesso("Todos os pacotes foram entregues!");
            }
            else
            {
                foreach (var pacote in pacotesNaoEntregues)
                {
                    string linha = $"- Destino: ({pacote.Destination.PosX}, {pacote.Destination.PosY}) Peso: {pacote.Weight}kg Prioridade: {pacote.DeliveryPriority}";

                    switch (pacote.DeliveryPriority)
                    {
                        case Priority.Alta:
                            ConsoleHelper.PrioridadeAlta(linha);
                            break;
                        case Priority.Media:
                            ConsoleHelper.PrioridadeMedia(linha);
                            break;
                        case Priority.Baixa:
                            ConsoleHelper.PrioridadeBaixa(linha);
                            break;
                        default:
                            Console.WriteLine(linha);
                            break;
                    }
                }
            }
        }
    }
}