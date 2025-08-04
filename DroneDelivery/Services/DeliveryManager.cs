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
        private readonly DeliveryReport report = new DeliveryReport();

        public void ProcessDeliveries()
        {
            ConsoleHelper.Info("===== Iniciando simulação de entregas =====");
            var inicio = DateTime.Now;

            var drones = DeliveryData.Drones;

            // --- Validação de pacotes inválidos ---
            var pacotesInvalidos = DeliveryData.Pacotes.Where(p =>
                p.Weight <= 0 ||
                p.Destination.PosX < 0 ||
                p.Destination.PosY < 0
            ).ToList();

            foreach (var p in pacotesInvalidos)
            {
                ConsoleHelper.Erro($"❌ Pacote inválido: destino=({p.Destination.PosX},{p.Destination.PosY}), peso={p.Weight}kg");
            }

            var pacotesValidos = DeliveryData.Pacotes
                .Except(pacotesInvalidos)
                .Where(p => !DeliveryData.ZonasProibidas.Any(z => z.PosX == p.Destination.PosX && z.PosY == p.Destination.PosY))
                .ToList();

            // --- Ordenação inteligente ---
            // Mais próximo e mais pesado tem prioridade
            var sortedPackages = pacotesValidos
                .OrderByDescending(p => p.DeliveryPriority)
                .ThenBy(p => drones.Min(d => d.Position.DistanceTo(p.Destination)))
                .ThenByDescending(p => p.Weight)
                .ToList();

            // --- Simulação de entregas por drone ---
            foreach (var drone in drones)
            {
                ConsoleHelper.Info($"\n[Drone {drone.Id}] Iniciando operação...");

                double capacidadeRestante = drone.Capacity;
                double alcanceRestante = drone.MaxDistance;
                var pacotesSelecionados = new List<(Package, double)>();

                foreach (var pacote in sortedPackages.ToList())
                {
                    double distancia = drone.Position.DistanceTo(pacote.Destination) * 2;
                    double consumoBateria = distancia * 2; // 0.5% por unidade de distância

                    if (pacote.Weight <= capacidadeRestante &&
                        distancia <= alcanceRestante &&
                         drone.Battery >= consumoBateria)
                    {
                        pacotesSelecionados.Add((pacote, distancia));
                        capacidadeRestante -= pacote.Weight;
                        alcanceRestante -= distancia;
                        drone.Battery -= consumoBateria;
                        sortedPackages.Remove(pacote);
                    }
                }

                if (pacotesSelecionados.Count == 0)
                {
                    ConsoleHelper.Alerta($"[Drone {drone.Id}] Nenhum pacote foi atribuído a este drone.");
                    continue;
                }

                // --- Estados do drone ---
                ConsoleHelper.States($"[Drone {drone.Id}] Estado: {DroneState.Carregando}");
                Thread.Sleep(500);

                ConsoleHelper.States($"[Drone {drone.Id}] Estado: {DroneState.EmVoo}");
                Thread.Sleep(500);

                ConsoleHelper.States($"[Drone {drone.Id}] Estado: {DroneState.Entregando}");

                foreach (var (pacote, distancia) in pacotesSelecionados)
                {
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
                            ConsoleHelper.Sucesso($"Entregando para ({pacote.Destination.PosX}, {pacote.Destination.PosY}) - {pacote.Weight}kg");
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

            var duracao = DateTime.Now - inicio;
            ConsoleHelper.Info($"\nTempo total da simulação: {duracao.TotalSeconds:F1} segundos");

            report.ExibirRelatorio();
        }
    }
}
