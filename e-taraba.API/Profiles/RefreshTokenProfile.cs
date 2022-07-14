using AutoMapper;

namespace e_taraba.API.Profiles
{
    public class RefreshTokenProfile : Profile
    {
        public RefreshTokenProfile()
        {
            CreateMap<DTOs.RefreshTokenDto, Entities.RefreshToken>();
            CreateMap<DTOs.RefreshTokenUpdateDto, Entities.RefreshToken>();
        }
    }
}
