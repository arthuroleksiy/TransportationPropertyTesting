using FsCheck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportationPropertyTesting.Domain;
using FsCheck.Fluent;

namespace TransportationPropertyTesting.Generators
{
    /// <summary>
    /// Custom generator to ensure:
    /// - capacity > 0
    /// - fuel is always within bounds
    /// Without generators, FsCheck would generate invalid domain state.
    /// </summary>
    // Generator for arbitrary FuelTank instances

    public static class FuelTankGen
    {
        public static Arbitrary<FuelTank> ArbFuelTank()
        {
            var capacityGen = Gen.Choose(10, 500).Select(x => (double)x);
            
            var gen = from capacity in capacityGen
                      from fuel in Gen.Choose(0, (int)capacity).Select(x => (double)x)

                      select new FuelTank(capacity, fuel);

            return Arb.From(gen);
        }
    }

}

