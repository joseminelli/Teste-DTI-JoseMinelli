using DroneDelivery.Models;
using DroneDelivery.Utils;
using DroneDelivery.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DroneDelivery.Services
{
    public class DeliveryManager
    {
        private DeliveryReport report = new DeliveryReport();

        public void ProcessDeliveries()
        {
            ConsoleHelper.Info("===== Iniciando simulação de entregas =====");

            // Ordena os pacotes por prioridade
            var drones = DeliveryData.Drones;
            var sortedPackages = DeliveryData.Pacotes
                .OrderByDescending(p => p.DeliveryPriority)
                .ToList();

            foreach (var drone in drones)
            {
                ConsoleHelper.Info($"\n[Drone {drone.Id}] Iniciando operação...");

                double capacidadeRestante = drone.Capacity;
                double alcanceRestante = drone.MaxDistance;
                var pacotesSelecionados = new List<(Package, double)>();

                // Percorre os pacotes restantes para ver quais cabem no drone
                foreach (var pacote in sortedPackages.ToList())
                {
                    double distancia = drone.Position.DistanceTo(pacote.Destination) * 2;

                    if (pacote.Weight <= capacidadeRestante && distancia <= alcanceRestante)
                    {
                        pacotesSelecionados.Add((pacote, distancia));
                        capacidadeRestante -= pacote.Weight;
                        alcanceRestante -= distancia;
                        sortedPackages.Remove(pacote);
                    }
                }

                if (pacotesSelecionados.Count == 0)
                {
                    ConsoleHelper.Alerta("[Drone " + drone.Id + "] Nenhum pacote foi atribuído a este drone.");
                    continue;
                }

                ConsoleHelper.States($"[Drone {drone.Id}] Estado: {DroneState.Carregando}");
                Thread.Sleep(500);

                ConsoleHelper.States($"[Drone {drone.Id}] Estado: {DroneState.EmVoo}");
                Thread.Sleep(500);

                ConsoleHelper.States($"[Drone {drone.Id}] Estado: {DroneState.Entregando}");

                foreach (var (pacote, distancia) in pacotesSelecionados)
                {
                    // Imprime a entrega com cor baseada na prioridade
                    switch (pacote.DeliveryPriority)
                    {
                        case Priority.Alta:
                            ConsoleHelper.PrioridadeAlta($"Entregando para ({pacote.Destination.PosX}, {pacote.Destination.PosY}) - {pacote.Weight}kg - {pacote.DeliveryPriority}");
                            break;
                        case Priority.Media:
                            ConsoleHelper.PrioridadeMedia($"Entregando para ({pacote.Destination.PosX}, {pacote.Destination.PosY}) - {pacote.Weight}kg - {pacote.DeliveryPriority}");
                            break;
                        case Priority.Baixa:
                            ConsoleHelper.PrioridadeBaixa($"Entregando para ({pacote.Destination.PosX}, {pacote.Destination.PosY}) - {pacote.Weight}kg - {pacote.DeliveryPriority}");
                            break;
                        default:
                            ConsoleHelper.Sucesso($"Entregando para ({pacote.Destination.PosX}, {pacote.Destination.PosY}) - {pacote.Weight}kg - {pacote.DeliveryPriority}");
                            break;
                    }

                    report.RegistrarEntrega(drone.Id, pacote, distancia);
                    Thread.Sleep(500);
                }

                ConsoleHelper.States($"[Drone {drone.Id}] Estado: {DroneState.Retornando}");
                Thread.Sleep(500);

                drone.State = DroneState.Idle;
            }

            foreach (var pacote in sortedPackages)
                report.AdicionarNaoEntregue(pacote);

            report.ExibirRelatorio();
        }

    }
}
