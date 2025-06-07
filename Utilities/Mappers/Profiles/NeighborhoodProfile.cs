using AutoMapper;
using Entity.Model.Base;
using Entity.Dtos.NeighborhoodDTO;

namespace Utilities.Mappers.Profiles
{
    public class NeighborhoodProfile : Profile
    {
        public NeighborhoodProfile()
        {
            CreateMap<Neighborhood, NeighborhoodDto>().ReverseMap();
            CreateMap<UpdateNeighborhoodDto, Neighborhood>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<DeleteLogicalNeighborhoodDto, Neighborhood>();
        }
    }
}