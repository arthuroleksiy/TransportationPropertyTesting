using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportationPropertyTesting.Domain
{

    /// <summary>
    /// Simple model of a fuel tank.
    /// Key invariant: Fuel is ALWAYS between 0 and Capacity.
    /// Perfect PBT target: bounds, monotonic behavior, conservation.
    /// </summary>
    public sealed class FuelTank
    {
        public double Capacity { get; }
        public double Current { get; private set; }

        public FuelTank(double capacity, double initial)
        {
            if (capacity <= 0)
                throw new ArgumentException("Capacity must be positive.");

            if (initial < 0 || initial > capacity)
                throw new ArgumentException("Initial fuel must be within capacity.");

            Capacity = capacity;
            Current = initial;
        }

        /// <summary>
        /// Property: Refuel never exceeds capacity.
        /// </summary>
        public void Refuel(double liters)
        {
            if (liters < 0)
                throw new ArgumentException("Refuel amount must be positive.");

            Current = Math.Min(Capacity, Current + liters);
        }

        /// <summary>
        /// Property: Consume never makes fuel negative.
        /// </summary>
        public void Consume(double liters)
        {
            if (liters < 0)
                throw new ArgumentException("Consumed fuel must be positive.");

            if (liters > Current)
                throw new InvalidOperationException("Not enough fuel.");

            Current -= liters;
        }
    }

}
