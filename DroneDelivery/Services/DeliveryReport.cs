using DroneDelivery.Models;
using DroneDelivery.Utils;
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
            ConsoleHelper.Info("\n===== RELATÓRIO FINAL =====");
            ConsoleHelper.Sucesso($"Total de entregas realizadas: {totalEntregas}");

            if (pesoPorDrone.Any())
            {
                var maisEficiente = pesoPorDrone.OrderByDescending(p => p.Value).First();
                ConsoleHelper.Sucesso($"Drone mais eficiente: {maisEficiente.Key} (Total entregue: {maisEficiente.Value}kg)");
            }

            ConsoleHelper.Info("\nDistância total por drone:");
            foreach (var d in distanciaPorDrone)
            {
                Console.WriteLine($"- {d.Key}: {d.Value:F2} km");
            }

            ConsoleHelper.Erro("\nPacotes não entregues:");
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
