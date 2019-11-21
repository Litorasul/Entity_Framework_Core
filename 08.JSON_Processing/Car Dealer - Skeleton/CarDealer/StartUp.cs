using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
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

                var inputJson = File.ReadAllText("./../../../Datasets/parts.json");

                var result = ImportParts(db, inputJson);

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
    }
}