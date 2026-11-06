using FsCheck;
using FsCheck.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportationPropertyTesting.Generators
{
    // Generator for arbitrary fuel amounts (used in refuel tests)
    public static class FuelAmountGen
    {
        public static Arbitrary<double> ArbFuelAmount()
        {
            var gen = Gen.Choose(0, 200).Select(x => (double)x);

            return Arb.From(gen);

        }
    }

}
