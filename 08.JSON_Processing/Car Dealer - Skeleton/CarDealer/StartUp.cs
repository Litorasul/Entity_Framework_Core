using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (var db = new CarDealerContext())
            {
                //db.Database.EnsureDeleted();
                //db.Database.EnsureCreated();

                //var inputJson = File.ReadAllText("./../../../Datasets/sales.json");

                var result = GetCarsWithTheirListOfParts(db);

                Console.WriteLine(result);

            }

        }

        //Problem 09 - 100%
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliers = JsonConvert.DeserializeObject<List<Supplier>>(inputJson);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}.";
        }

        //Problem 10 - 100%
        public static string ImportParts(CarDealerContext context, string inputJson)
        {

            var parts = JsonConvert.DeserializeObject<List<Part>>(inputJson).Where(p => p.SupplierId <= 31).ToList();

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}.";
        }

        //Problem 11 - 100%
        public static string ImportCars(CarDealerContext context, string inputJson)
        {

            var carsToImport = JsonConvert.DeserializeObject<List<CarsImportDTO>>(inputJson);

            var cars = new List<Car>();
            var carParts = new List<PartCar>();

            foreach (var carToImport in carsToImport)
            {
                var car = new Car()
                {
                    Make = carToImport.Make,
                    Model = carToImport.Model,
                    TravelledDistance = carToImport.TravelledDistance
                };

                foreach (var part in carToImport.PartsId.Distinct())
                {
                    var carPart = new PartCar()
                    {
                        PartId = part,
                        Car = car
                    };

                    carParts.Add(carPart);
                }

                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.PartCars.AddRange(carParts);
            context.SaveChanges();

            return $"Successfully imported {carsToImport.Count}.";

        }

        //Problem 12 - 100%
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {

            var customers = JsonConvert.DeserializeObject<List<Customer>>(inputJson);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}.";
        }

        //Problem 13 - 100%
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<List<Sale>>(inputJson);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}.";
        }

        //Problem 14 - 100%
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context
                .Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate.ToString("d", new CultureInfo("fr-FR")),
                    IsYoungDriver = c.IsYoungDriver
                })
                .ToList();

            var json = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return json;
        }

        //Problem 15 - 100%
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var toyotas = context
                .Cars
                .Where(t => t.Make == "Toyota")
                .OrderBy(t => t.Model)
                .ThenByDescending(t => t.TravelledDistance)
                .Select(t => new
                {
                    t.Id,
                    t.Make,
                    t.Model,
                    t.TravelledDistance
                })
                .ToList();

            var json = JsonConvert.SerializeObject(toyotas, Formatting.Indented);

            return json;
        }

        //Problem 16 - 100%
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context
                .Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToList();

            var json = JsonConvert.SerializeObject(suppliers, Formatting.Indented);

            return json;
        }

        //Problem 17 - 100%
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context
                .Cars
                .Select(c => new
                {
                    car = new
                    {
                        c.Make,
                        c.Model,
                        c.TravelledDistance
                    },
                    parts = c.PartCars.Select(p => new
                    {
                        p.Part.Name,
                        Price = $"{p.Part.Price:F2}"
                    })

                })
                .ToList();

            var json = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return json;
        }
    }
}