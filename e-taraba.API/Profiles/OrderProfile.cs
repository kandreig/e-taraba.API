using AutoMapper;

namespace e_taraba.API.Profiles
{
    public class OrderProfile:Profile
    {
        public OrderProfile()
        {
            CreateMap<Entities.Order, DTOs.OrderWithoutProductsDto>();
            CreateMap<Entities.Order, DTOs.OrderWithProductsDto>();
            CreateMap<DTOs.OrderForCreationDto, Entities.Order>();
        }
    }
}
