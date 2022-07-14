using AutoMapper;

namespace e_taraba.API.Profiles
{
    public class ProductOrderProfile : Profile
    {
        public ProductOrderProfile()
        {
            CreateMap<DTOs.ProductOrderForCreation, Entities.ProductOrder>();
            CreateMap<Entities.ProductOrder, DTOs.ProductOrderDto>();
        }
    }
}
