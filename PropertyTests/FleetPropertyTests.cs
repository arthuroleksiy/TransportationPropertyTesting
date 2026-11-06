using FluentAssertions;
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportationPropertyTesting.Domain;
using TransportationPropertyTesting.Generators;

namespace TransportationPropertyTesting.PropertyTests
{
   
    public class FleetPropertyTests
    {

        [Property(Arbitrary = new[] { typeof(FuelTankGen), typeof(FuelAmountGen)})]
        public void Fuel_never_exceeds_capacity(FuelTank tank, double fuel)
        {
            fuel = Math.Abs(fuel);
            tank.Refuel(fuel);
            tank.Current.Should().BeLessThanOrEqualTo(tank.Capacity);
        }

        [Property(Arbitrary = new[] { typeof(VehicleGen), typeof(RouteGen) })]
        public void Mileage_only_increases(Vehicle vehicle, Route route)
        {
            var required = route.DistanceKm / 100 * vehicle.ConsumptionPer100Km;

            if (required > vehicle.Tank.Current)
            {
                return;
            }

            var before = vehicle.Mileage;

            vehicle.Drive(route);

            vehicle.Mileage.Should().BeGreaterThan(before);

        }
        [Property(Arbitrary = new[] { typeof(VehicleGen), typeof(RouteGen)})]
        public void Driving_consumes_fuel(Vehicle vehicle, Route route)
        {

            var required = route.DistanceKm / 100 * vehicle.ConsumptionPer100Km;

            if (required > vehicle.Tank.Current)
                return;

            var before = vehicle.Tank.Current;

            vehicle.Drive(route);
            vehicle.Tank.Current.Should().BeLessThan(before);
        }

        [Property(Arbitrary = new[] { typeof(VehicleGen), typeof(RouteGen)})]
        public void Trip_changes_vehicle_state(Vehicle vehicle, Route route)
        {
            var required = route.DistanceKm / 100 * vehicle.ConsumptionPer100Km;

            if (required > vehicle.Tank.Current)
                return;

            var mileage = vehicle.Mileage;
            var fuel = vehicle.Tank.Current;

            new Trip(vehicle, route).Execute();

            vehicle.Mileage.Should().BeGreaterThan(mileage);
            vehicle.Tank.Current.Should().BeLessThan(fuel);

        }

        [Property(Arbitrary = new[] { typeof(VehicleGen) })]
        public void Total_mileage_is_sum_vehicles(Vehicle[] vehicles)
        {
            var total = FleetService.TotalMileage(vehicles);

            total.Should().BeApproximately(vehicles.Sum(vehicles => vehicles.Mileage), 0.0001);

        }

        [Property(Arbitrary = new[] { typeof(VehicleGen), typeof(RouteGen)})]
        public void Fleet_state_is_deterministic(Vehicle vehicle, Route route)
        {
            var required = route.DistanceKm / 100 * vehicle.ConsumptionPer100Km;

            if (required > vehicle.Tank.Current)
                return;

            var v1 = Clone(vehicle);
            var v2 = Clone(vehicle);

            v1.Drive(route);
            v2.Drive(route);

            v1.Mileage.Should().Be(v2.Mileage);
            v1.Tank.Current.Should().Be(v2.Tank.Current);
        }

        [Property(Arbitrary = new[] { typeof(FuelTankGen)})]
        public void Refuel_negative_throws(FuelTank tank, NegativeInt liters)
        {
            Action act = () => tank.Refuel(liters.Get);
            act.Should().Throw<ArgumentException>();
        }

        [Property(Arbitrary = new[] { typeof(FuelTankGen) })]
        public void Consume_more_than_available_throws(FuelTank tank, PositiveInt liters) {
            Action act = () => tank.Consume(tank.Current + liters.Get);
            act.Should().Throw<InvalidOperationException>();
        }

        [Property(Arbitrary = new[] { typeof(FuelTankGen), typeof(FuelAmountGen), typeof(FuelAmountGen)})]
        public void Two_refuels_equal_one_combined(FuelTank tank, double a, double b)
        {
            var copy = new FuelTank(tank.Capacity, tank.Current);
            tank.Refuel(a);
            tank.Refuel(b);
            copy.Refuel(a + b);
            tank.Current.Should().BeApproximately(copy.Current, 1e-9);
        }

        [Property(Arbitrary = new[] { typeof(VehicleGen), typeof(RouteGen) })]
        public Property Driving_consumes_fuel_when(Vehicle vehicle, Route route)
        {
            var required = route.DistanceKm / 100 * vehicle.ConsumptionPer100Km;
            bool hasFuel = required <= vehicle.Tank.Current;
            bool ok = true;

            if (hasFuel)
            {
                var before = vehicle.Tank.Current;
                vehicle.Drive(route);
                ok = vehicle.Tank.Current < before;
            }

            return ok.When(hasFuel);

        }

        [Property(Arbitrary = new[] { typeof(VehicleGen)})]
        public void Adding_vehicle_increses_total_fuel(Vehicle[] fleet, Vehicle extra)
        {
            var before = FleetService.TotalFuel(fleet);
            var after = FleetService.TotalFuel(fleet.Append(extra));

            after.Should().BeApproximately(before + extra.Tank.Current, 0.001);

        }

        private static Vehicle Clone(Vehicle v)
        {
            return new Vehicle(v.Model, new FuelTank(v.Tank.Capacity, v.Tank.Current), v.ConsumptionPer100Km);
        }


    }

}
