using System.Linq;

using CarDealer.Models;
using CarDealer.Dtos.Export;
using CarDealer.Dtos.Import;


using AutoMapper;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            this.CreateMap<ImportSuppliersDto, Supplier>();

            this.CreateMap<ImportPartsDto, Part>();

            this.CreateMap<ImportCustomerDto, Customer>();

            this.CreateMap<ImportSalesDto, Sale>();

            this.CreateMap<Supplier, ExportLocalSuppliersDto>();

            this.CreateMap<Part, ExportCarPartDto>();

            this.CreateMap<Car, ExportCarDto>()
                .ForMember(x => x.Parts,
                    y => y.MapFrom(x => x.PartCars.Select(pc => pc.Part)));
        }
    }
}
