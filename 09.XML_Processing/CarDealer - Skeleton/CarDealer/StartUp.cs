using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using CarDealer.Data;
using CarDealer.Models;
using CarDealer.Dtos.Import;
using CarDealer.Dtos.Export;

using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Xml;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            Mapper.Initialize(cfg => cfg.AddProfile<CarDealerProfile>());

            using (var db = new CarDealerContext())
            {
                //db.Database.EnsureDeleted();
                //db.Database.EnsureCreated();

                //var inputXml = File.ReadAllText("./../../../Datasets/sales.xml");

                var result = GetCarsFromMakeBmw(db);

                Console.WriteLine(result);
            }
        }

        //Problem 09 - 100%
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportSuppliersDto[]),
                new XmlRootAttribute("Suppliers"));

            ImportSuppliersDto[] suppliersDtos;

            using (var reader = new StringReader(inputXml))
            {
                suppliersDtos = (ImportSuppliersDto[]) xmlSerializer.Deserialize(reader);
            }

            var suppliers = Mapper.Map<Supplier[]>(suppliersDtos);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Length}";
        }

        //Problem 10 - 100%
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportPartsDto[]),
                new XmlRootAttribute("Parts"));

            ImportPartsDto[] partsDtos;

            using (var reader = new StringReader(inputXml))
            {
                partsDtos = ((ImportPartsDto[]) xmlSerializer
                        .Deserialize(reader))
                    .Where(p => context.Suppliers.Any(s => s.Id == p.SupplierId))
                    .ToArray();
            }

            var parts = Mapper.Map<Part[]>(partsDtos);

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Length}";
        }

        //Problem 11 - 100%
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportCarDto[]),
                new XmlRootAttribute("Cars"));

            ImportCarDto[] carDtos;

            using (var reader = new StringReader(inputXml))
            {
                carDtos = (ImportCarDto[]) xmlSerializer.Deserialize(reader);
            }

            List<Car> cars = new List<Car>();
            List<PartCar> partsCars = new List<PartCar>();

            foreach (var carDto in carDtos)
            {
                var car = new Car()
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TravelledDistance = carDto.TravelledDistance
                };

                var parts = carDto
                    .Parts
                    .Where(pc => context.Parts.Any(p => p.Id == pc.Id))
                    .Select(p => p.Id)
                    .Distinct();

                foreach (var partId in parts)
                {
                    var partCar = new PartCar()
                    {
                        PartId = partId,
                        Car = car
                    };

                    partsCars.Add(partCar);
                }

                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.PartCars.AddRange(partsCars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        //Problem 12 - 100%
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportCustomerDto[]),
                new XmlRootAttribute("Customers"));

            ImportCustomerDto[] customerDtos;

            using (var reader = new StringReader(inputXml))
            {
                customerDtos = (ImportCustomerDto[]) xmlSerializer.Deserialize(reader);
            }

            var customers = Mapper.Map<Customer[]>(customerDtos);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Length}";
        }

        //Problem 13 - 100%
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportSalesDto[]),
                new XmlRootAttribute("Sales"));

            ImportSalesDto[] salesDtos;

            using (var reader = new StringReader(inputXml))
            {
                salesDtos = ((ImportSalesDto[]) xmlSerializer.Deserialize(reader))
                    .Where(s => context.Cars.Any(c => c.Id == s.CarId))
                    .ToArray();
            }

            var sales = Mapper.Map<Sale[]>(salesDtos);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Length}";
        }

        //Problem 14 - 100%
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.TravelledDistance > 2000000)
                .Select(c => new ExportCarsWithDistanceDto
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportCarsWithDistanceDto[]), new XmlRootAttribute("cars"));

            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            xmlSerializer.Serialize(new StringWriter(sb), cars, namespaces);

            return sb.ToString().TrimEnd();

        }

        //Problem 15 - 100%
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            StringBuilder sb = new StringBuilder();

            var bmws = context
                .Cars
                .Where(c => c.Make == "BMW")
                .Select(c => new ExportBMWDto
                {
                    Id = c.Id,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportBMWDto[]),
                new XmlRootAttribute("cars"));

            var nameSpaces = new XmlSerializerNamespaces();
            nameSpaces.Add("", "");

            using (var writer = new StringWriter(sb))
            {
                xmlSerializer.Serialize(writer, bmws, nameSpaces);
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 16 - 100%
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            StringBuilder sb = new StringBuilder();

            var suppliers = context
                .Suppliers
                .Where(s => !s.IsImporter)
                .ProjectTo<ExportLocalSuppliersDto>()
                .ToArray();


            var xmlSerializer = new XmlSerializer(typeof(ExportLocalSuppliersDto[]),
                new XmlRootAttribute("suppliers"));

            var nameSpaces = new XmlSerializerNamespaces();
            nameSpaces.Add("", "");

            using (var writer = new StringWriter(sb))
            {
                xmlSerializer.Serialize(writer, suppliers, nameSpaces);
            }

            return sb.ToString().TrimEnd();
        }


        //Problem 17 - 100%
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            StringBuilder sb = new StringBuilder();

            var cars = context
                .Cars
                .OrderByDescending(c => c.TravelledDistance)
                .ThenBy(c => c.Model)
                .Take(5)
                .ProjectTo<ExportCarDto>()
                .ToArray();

            foreach (var car in cars)
            {
                car.Parts = car.Parts
                    .OrderByDescending(p => p.Price)
                    .ToArray();
            }

            var xmlSerializer = new XmlSerializer(typeof(ExportCarDto[]),
                new XmlRootAttribute("cars"));

            var nameSpaces = new XmlSerializerNamespaces();
            nameSpaces.Add("", "");

            using (var writer = new StringWriter(sb))
            {
                xmlSerializer.Serialize(writer, cars, nameSpaces);
            }

            return sb.ToString().TrimEnd();
        }
    }
}