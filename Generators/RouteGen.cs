using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportationPropertyTesting.Domain;
using FsCheck;
using FsCheck.Fluent;

namespace TransportationPropertyTesting.Generators
{
    /// <summary>
    /// Route must never be zero/negative.
    /// </summary>
    // Generator for arbitrary Route instances
    public static class RouteGen
    {
        public static Arbitrary<Route> ArbRoute() {
            var gen = from km in Gen.Choose(1, 2000)
            select new Route(km);

            return Arb.From(gen);
        }
}


}
