using AutoMapper;
using Entity.Model;
using Entity.Dtos.ProviderDTO;

namespace Utilities.Mappers.Profiles
{
    public class ProviderProfile : Profile
    {
        public ProviderProfile()
        {
            CreateMap<Provider, ProviderDto>().ReverseMap();
            CreateMap<UpdateProviderDto, Provider>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<DeleteLogicalProviderDto, Provider>();
        }
    }
}