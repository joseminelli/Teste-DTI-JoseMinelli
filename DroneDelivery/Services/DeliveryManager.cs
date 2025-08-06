using DroneDelivery.Data;
using DroneDelivery.Models;
using DroneDelivery.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DroneDelivery.Services
{
    public class DeliveryManager
    {
        private readonly DeliveryReport report = new DeliveryReport();

        private readonly List<Drone> drones;
        private readonly List<Package> pacotes;
        private readonly List<Location> zonasProibidas;

        public DeliveryManager(List<Drone> drones, List<Package> pacotes, List<Location> zonasProibidas)
        {
            this.drones = drones;
            this.pacotes = pacotes;
            this.zonasProibidas = zonasProibidas;
        }

        public DeliveryReport ProcessDeliveries()
        {
            ConsoleHelper.Info("===== Iniciando simulação de entregas =====");
            var inicio = DateTime.Now;

            var pacotesInvalidos = pacotes.Where(p =>
                p.Weight <= 0 ||
                p.Destination.PosX < 0 ||
                p.Destination.PosY < 0
            ).ToList();

            foreach (var p in pacotesInvalidos)
            {
                ConsoleHelper.Erro($"Pacote inválido: destino=({p.Destination.PosX},{p.Destination.PosY}), peso={p.Weight}kg");
            }

            var pacotesValidos = pacotes
                .Except(pacotesInvalidos)
                .Where(p => !zonasProibidas.Any(z => z.PosX == p.Destination.PosX && z.PosY == p.Destination.PosY))
                .ToList();

            // Ordenação inteligente
            var sortedPackages = pacotesValidos
                .OrderByDescending(p => p.DeliveryPriority)
                .ThenBy(p => DeliveryData.CentroDistribuicao.DistanceTo(p.Destination))
                .ThenByDescending(p => p.Weight)
                .ToList();


            // Constantes para simulação
            const double consumoPorUnidadeDistancia = 0.5; // 0.5% de bateria por unidade de distância
            const int tempoCarregamentoMs = 2000;           // tempo simulado para carregar bateria

            while (sortedPackages.Count > 0)
            {
                bool algumDroneAtuou = false;

                foreach (var drone in drones)
                {
                    if (sortedPackages.Count == 0)
                        break;

                    ConsoleHelper.Info($"\n[Drone {drone.Id}] Iniciando operação...");

                    // Se bateria insuficiente para ir e voltar à próxima entrega mais próxima, recarrega
                    var pacoteMaisProximo = sortedPackages
                        .OrderBy(p => drone.Position.DistanceTo(p.Destination))
                        .First();

                    double distanciaIdaVolta = drone.Position.DistanceTo(pacoteMaisProximo.Destination) * 2;
                    double consumoEstimado = distanciaIdaVolta * consumoPorUnidadeDistancia;

                    if (drone.Battery < consumoEstimado)
                    {
                        drone.State = DroneState.Carregando;
                        ConsoleHelper.States($"[Drone {drone.Id}] Estado: {DroneState.Carregando} - Recarregando bateria...");
                        Thread.Sleep(tempoCarregamentoMs);
                        drone.Battery = 100.0;
                        ConsoleHelper.States($"[Drone {drone.Id}] Bateria recarregada.");
                    }

                    drone.State = DroneState.Idle;
                    double capacidadeRestante = drone.Capacity;
                    double alcanceRestante = drone.MaxDistance;
                    var pacotesSelecionados = new List<(Package, double)>();

                    foreach (var pacote in sortedPackages.ToList())
                    {
                        double distancia = drone.Position.DistanceTo(pacote.Destination) * 2; // ida e volta
                        double consumoBateria = distancia * consumoPorUnidadeDistancia;

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
                        ConsoleHelper.Alerta($"[Drone {drone.Id}] Nenhum pacote pode ser atribuído nesta viagem.");
                        continue;
                    }

                    // Estados do drone na viagem
                    drone.State = DroneState.EmVoo;
                    ConsoleHelper.States($"[Drone {drone.Id}] Estado: {DroneState.EmVoo}");
                    Thread.Sleep(500);

                    drone.State = DroneState.Entregando;
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

                    drone.State = DroneState.Retornando;
                    ConsoleHelper.States($"[Drone {drone.Id}] Estado: {DroneState.Retornando}");
                    Thread.Sleep(500);

                    // Simula retorno à base
                    drone.Position = new Location(0, 0);
                    drone.State = DroneState.Idle;

                    algumDroneAtuou = true;
                }

                if (!algumDroneAtuou)
                {
                    // Nenhum drone conseguiu fazer entregas nesta rodada - evita loop infinito
                    ConsoleHelper.Alerta("Nenhum drone pôde entregar pacotes restantes. Encerrando.");
                    break;
                }
            }

            var duracao = DateTime.Now - inicio;
            ConsoleHelper.Info($"\nTempo total da simulação: {duracao.TotalSeconds:F1} segundos");

            return report;
        }
    }
}
