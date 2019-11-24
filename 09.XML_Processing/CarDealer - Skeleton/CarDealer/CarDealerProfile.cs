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
        }
    }
}
