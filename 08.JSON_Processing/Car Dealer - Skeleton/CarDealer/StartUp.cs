using System;
using System.Collections.Generic;
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

                var inputJson = File.ReadAllText("./../../../Datasets/sales.json");

                var result = ImportSales(db, inputJson);

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
    }
}