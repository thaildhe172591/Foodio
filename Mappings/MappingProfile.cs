using AutoMapper;
using FoodioAPI.Entities;
using FoodioAPI.DTOs.User;

namespace FoodioAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.Username,
                           opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Roles,
                           opt => opt.Ignore());
        }
    }
}
