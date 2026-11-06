using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportationPropertyTesting.Domain
{
    /// <summary>
    /// Value object.
    /// PBT: generated distances must obey domain invariants.
    /// </summary>
    public sealed class Route
    {
        public double DistanceKm { get; }

        public Route(double distanceKm)
        {
            if (distanceKm <= 0)
                throw new ArgumentException("Route distance must be positive.");

            DistanceKm = distanceKm;
        }
    }

}
