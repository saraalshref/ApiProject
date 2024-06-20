using Api_Project.DTOS;
using Api_Project.Models;
using AutoMapper;

namespace Api_Project.Mapping
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<UserDTO, ApplicationUser>().ReverseMap();
            CreateMap<UserRegisterDTO, ApplicationUser>().ReverseMap();


        }


    }
}
