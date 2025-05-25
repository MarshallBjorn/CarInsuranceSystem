using AutoMapper;
using Infrastructure.DTOs;
using Core.Entities;

namespace Infrastructure.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<Car, CarDto>();
        CreateMap<UserCar, UserCarDto>();
        CreateMap<Firm, FirmDto>();
    }
}