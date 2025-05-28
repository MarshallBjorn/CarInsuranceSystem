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
        CreateMap<Firm, FirmInsuranceDto>();
        CreateMap<InsuranceType, InsuranceTypeDto>()
            .ForMember(dest => dest.FirmInsuranceDto, opt => opt.MapFrom(src => src.Firm));
        CreateMap<CreateUpdateInsuranceTypeDto, InsuranceType>();
        CreateMap<Firm, FirmDto>();
    }
}