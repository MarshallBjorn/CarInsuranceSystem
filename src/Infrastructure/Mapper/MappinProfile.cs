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
        CreateMap<CarInsurance, CarInsuranceDto>();
        CreateMap<InsuranceType, InsuranceTypeDto>()
            .ForMember(dest => dest.firmDto, opt => opt.MapFrom(src => src.Firm));
        CreateMap<Firm, FirmDto>();
    }
}