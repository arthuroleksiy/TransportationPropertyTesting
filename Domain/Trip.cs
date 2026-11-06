using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportationPropertyTesting.Domain
{
    /// <summary>
    /// Aggregate record of driving.
    /// PBT: replaying trips leads to determinism.
    /// </summary>
    public sealed class Trip
    {
        public Vehicle Vehicle { get; }
        public Route Route { get; }

        public Trip(Vehicle vehicle, Route route)
        {
            Vehicle = vehicle;
            Route = route;
        }

        /// <summary>
        /// Property: Executing trip changes vehicle state.
        /// </summary>
        public void Execute()
        {
            Vehicle.Drive(Route);
        }
    }
}
