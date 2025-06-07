
    using AutoMapper;
    using Entity.Model;
    using Entity.Dtos.CountryDTO;

    namespace Utilities.Mappers.Profiles
    {
        public class CountryProfile : Profile
        {
            public CountryProfile()
            {
                CreateMap<Country, CountryDto>().ReverseMap();
                CreateMap<UpdateCountryDto, Country>()
                    .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
                CreateMap<DeleteLogicalCountryDto, Country>();
            }
        }
    }

