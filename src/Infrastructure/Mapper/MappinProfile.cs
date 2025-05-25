using AutoMapper;
using Core.Entities;
using Core.DTOs;

namespace Infrastructure.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<Car, CarDto>();
        CreateMap<UserCar, UserCarDto>();
        CreateMap<Firm, FirmDto>();
        CreateMap<CarInsurance, CarInsuranceDto>();
        CreateMap<InsuranceType, InsuranceTypeDto>();
    }
}