using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportationPropertyTesting.Domain
{
    /// <summary>
    /// Entity representing car / truck.
    /// Mileage only grows.
    /// PBT: monotonic sequence properties.
    /// </summary>
    public sealed class Vehicle
    {
        public string Model { get; }
        public FuelTank Tank { get; }
        public double Mileage { get; private set; }

        // Fuel consumption rate (liters per 100km)
        public double ConsumptionPer100Km { get; }

        public Vehicle(string model, FuelTank tank, double litersPer100Km)
        {
            if (litersPer100Km <= 0)
                throw new ArgumentException("Consumption must be positive.");

            Model = model;
            Tank = tank;
            ConsumptionPer100Km = litersPer100Km;
        }

        /// <summary>
        /// Property: driving increases mileage.
        /// Property: driving reduces fuel.
        /// </summary>
        public void Drive(Route route)
        {
            var fuelRequired = (route.DistanceKm / 100.0) * ConsumptionPer100Km;

            Tank.Consume(fuelRequired);
            Mileage += route.DistanceKm;
        }
    }

}
