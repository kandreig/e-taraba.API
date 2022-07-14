using AutoMapper;

namespace e_taraba.API.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Entities.User, DTOs.UserForClaimsDto>();
            CreateMap<Entities.User, DTOs.UserInfoDto>();
        }
    }
}
