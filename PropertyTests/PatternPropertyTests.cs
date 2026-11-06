
using FluentAssertions;
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using TransportationPropertyTesting.Domain;
using TransportationPropertyTesting.Generators;

namespace TransportationPropertyTesting.PropertyTests
{
    public class PatternPropertyTests
    {

        // P1 · ТУДИ Й НАЗАД
        [Property(Arbitrary = new[] { typeof(FuelTankGen), typeof(FuelAmountGen) })]
        public Property P1_RoundTrip_refuel_then_consume(FuelTank tank, double liters)
        {
            bool fits = tank.Current + liters <= tank.Capacity;
            bool ok = true;

            if (fits)
            {
                var start = tank.Current;

                tank.Refuel(liters);
                tank.Consume(liters);

                ok = Math.Abs(tank.Current - start) < 1e-9;
            }

            return ok.When(fits);
        }

        // P2 · НЕЗМІННЕ (ІНВАРІАНТ)
        [Property(Arbitrary = new[] { typeof(FuelTankGen), typeof(FuelAmountGen) })]
        public Property P2_Invariant_consume_keeps_fuel_within_bounds(FuelTank tank, double liters)
        {
            bool enough = liters <= tank.Current;
            bool ok = true;

            if (enough)
            {
                tank.Consume(liters);

                ok = tank.Current >= 0 && tank.Current <= tank.Capacity;
            }

            return ok.When(enough);
        }

        // P3 · ПОВТОР НІЧОГО НЕ ЗМІНЮЄ (ІДЕМПОТЕНТНІСТЬ)
        [Property(Arbitrary = new[] { typeof(FuelTankGen) })]
        public void P3_Idempotent_fill_up_twice_equals_once(FuelTank tank)
        {
            tank.Refuel(tank.Capacity);
            var afterFirst = tank.Current;

            tank.Refuel(tank.Capacity);

            tank.Current.Should().Be(afterFirst);
        }

        // P4 · РІЗНІ ШЛЯХИ — ОДИН РЕЗУЛЬТАТ
        [Property(Arbitrary = new[] { typeof(VehicleGen), typeof(RouteGen) })]
        public Property P4_DifferentPaths_two_routes_equal_one_long(Vehicle vehicle, Route r1, Route r2)
        {
            var required = (r1.DistanceKm + r2.DistanceKm) / 100 * vehicle.ConsumptionPer100Km;
            bool hasFuel = required <= vehicle.Tank.Current;
            bool ok = true;

            if (hasFuel)
            {
                var twoTrips = Copy(vehicle);
                var oneTrip = Copy(vehicle);

                twoTrips.Drive(r1);
                twoTrips.Drive(r2);

                oneTrip.Drive(new Route(r1.DistanceKm + r2.DistanceKm));

                ok = Math.Abs(twoTrips.Mileage - oneTrip.Mileage) < 1e-9
                  && Math.Abs(twoTrips.Tank.Current - oneTrip.Tank.Current) < 1e-9;
            }

            return ok.When(hasFuel);
        }

        // P5 · ПОРІВНЯННЯ З ЕТАЛОНОМ (ТЕСТОВИЙ ОРАКУЛ)
        [Property(Arbitrary = new[] { typeof(VehicleGen), typeof(RouteGen) })]
        public Property P5_Oracle_trip_execute_equals_drive(Vehicle vehicle, Route route)
        {
            var required = route.DistanceKm / 100 * vehicle.ConsumptionPer100Km;
            bool hasFuel = required <= vehicle.Tank.Current;
            bool ok = true;

            if (hasFuel)
            {

                var viaTrip = Copy(vehicle);
                var direct = Copy(vehicle);

                new Trip(viaTrip, route).Execute();
                direct.Drive(route);

                ok = viaTrip.Mileage == direct.Mileage
                  && viaTrip.Tank.Current == direct.Tank.Current;
            }

            return ok.When(hasFuel);
        }

        // P6 · ПЕРЕДБАЧУВАНА ЗМІНА (МЕТАМОРФНА ВЛАСТИВІСТЬ)
        [Property(Arbitrary = new[] { typeof(VehicleGen), typeof(RouteGen) })]
        public Property P6_Metamorphic_double_route_uses_double_fuel(Vehicle vehicle, Route route)
        {
            var requiredForDouble = 2 * route.DistanceKm / 100 * vehicle.ConsumptionPer100Km;
            bool hasFuel = requiredForDouble <= vehicle.Tank.Current;
            bool ok = true;

            if (hasFuel)
            {
                var single = Copy(vehicle);
                var twice = Copy(vehicle);

                single.Drive(route);
                twice.Drive(new Route(route.DistanceKm * 2));

                var usedSingle = vehicle.Tank.Current - single.Tank.Current;
                var usedTwice = vehicle.Tank.Current - twice.Tank.Current;

                ok = Math.Abs(usedTwice - 2 * usedSingle) < 1e-9;
            }

            return ok.When(hasFuel);
        }
        
        // P7 · ЛЕГКО ПЕРЕВІРИТИ
        [Property(Arbitrary = new[] { typeof(VehicleGen), typeof(RouteGen) })]
        public Property P7_EasyToVerify_consumption_recovered_from_result(Vehicle vehicle, Route route)
        {
            var required = route.DistanceKm / 100 * vehicle.ConsumptionPer100Km;
            bool hasFuel = required <= vehicle.Tank.Current;
            bool ok = true;

            if (hasFuel)
            {
                var fuelBefore = vehicle.Tank.Current;
                var mileageBefore = vehicle.Mileage;

                vehicle.Drive(route);

                var fuelUsed = fuelBefore - vehicle.Tank.Current;
                var kmDriven = vehicle.Mileage - mileageBefore;

                var recovered = fuelUsed / kmDriven * 100;

                ok = Math.Abs(recovered - vehicle.ConsumptionPer100Km) < 1e-6;
            }

            return ok.When(hasFuel);
        }

        // P8 · ПРОСТО НЕ ПАДАЄ
        [Property(Arbitrary = new[] { typeof(FuelTankGen) })]
        public void P8_NoException_refuel_nonnegative_never_throws(FuelTank tank, NonNegativeInt liters)
        {
            Action act = () => tank.Refuel(liters.Get);
            act.Should().NotThrow();
        }

        private static Vehicle Copy(Vehicle v)
        {
            return new Vehicle(v.Model, new FuelTank(v.Tank.Capacity, v.Tank.Current), v.ConsumptionPer100Km);
        }
    }
}
