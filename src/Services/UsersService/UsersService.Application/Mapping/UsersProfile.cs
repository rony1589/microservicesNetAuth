using AutoMapper;
using UsersService.Application.DTOs;
using UsersService.Domain.Entities;

namespace UsersService.Application.Mapping
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()));
        }
    }
}
