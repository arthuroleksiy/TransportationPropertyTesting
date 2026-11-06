using FsCheck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportationPropertyTesting.Domain;
using TransportationPropertyTesting.Domain;
using FsCheck;
using FsCheck.Fluent;

namespace TransportationPropertyTesting.Generators
{
    /// <summary>
    /// Vehicle generator depends on FuelTank generator.
    /// Shows composition of generators.
    /// </summary>
    // Generator for arbitrary Vehicle instances
    public static class VehicleGen
    {
        public static Arbitrary<Vehicle> ArbVehicle()
        {
            var consumptionGen = Gen.Choose(5, 25).Select(x => (double)x);
            var gen = from tank in FuelTankGen.ArbFuelTank().Generator
                      from consumption in consumptionGen
                      select new Vehicle("TestModel", tank, consumption);

            return Arb.From(gen);



        }
    }
}
