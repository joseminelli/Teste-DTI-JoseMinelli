using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneDelivery.Models
{
    public enum DroneState { Idle, Carregando, EmVoo, Entregando, Retornando }

    public class Drone
    {
        public string Id { get; set; }
        public double Capacity { get; set; }
        public double MaxDistance { get; set; }
        public double Battery { get; set; }
        public Location Position { get; set; }
        public DroneState State { get; set; }

        public Drone(string id, double capacity, double maxDistance)
        {
            Id = id;
            Capacity = capacity;
            MaxDistance = maxDistance;
            Battery = 100.0;
            Position = new Location(0, 0);
            State = DroneState.Idle;
        }
    }
}
