using AutoMapper;

namespace e_taraba.API.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Entities.Product, DTOs.ProductWithoutOrdersDto>();
            CreateMap<Entities.Product, DTOs.ProductWithOrdersDto>();
            CreateMap<DTOs.ProductForCreationDto, Entities.Product>();
            CreateMap<DTOs.ProductForUpdateDto, Entities.Product>();
        }
    }
}
