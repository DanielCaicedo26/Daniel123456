using AutoMapper;
using Entity.Model;
using Entity.Dtos.ModuleDTO;

namespace Utilities.Mappers.Profiles
{
    public class ModuleProfile : Profile
    {
        public ModuleProfile()
        {
            CreateMap<Module, ModuleDto>().ReverseMap();
            CreateMap<UpdateModuleDto, Module>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<DeleteLogicalModuleDto, Module>();
        }
    }
}