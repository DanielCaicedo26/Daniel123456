
    using AutoMapper;
    using Entity.Model;
    using Entity.Dtos.CityDTO;

    namespace Utilities.Mappers.Profiles
    {
        public class CityProfile : Profile
        {
            public CityProfile()
            {
                CreateMap<City, CityDto>().ReverseMap();
                CreateMap<UpdateCityDto, City>()
                    .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
                CreateMap<DeleteLogicalCityDto, City>();
            }
        }
    }


