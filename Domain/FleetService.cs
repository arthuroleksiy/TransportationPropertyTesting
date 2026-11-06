using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportationPropertyTesting.Domain
{
    /// <summary>
    /// Higher-level operations across vehicles.
    /// PBT: system-level invariants (fuel distribution).
    /// </summary>
    public static class FleetService
    {
        public static double TotalFuel(IEnumerable<Vehicle> vehicles)
            => vehicles.Sum(v => v.Tank.Current);

        public static double TotalMileage(IEnumerable<Vehicle> vehicles)
            => vehicles.Sum(v => v.Mileage);
    }

}
