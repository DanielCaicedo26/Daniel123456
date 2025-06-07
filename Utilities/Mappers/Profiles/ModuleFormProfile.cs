using AutoMapper;
using Entity.Model;
using Entity.Dtos.ModuleFormDTO;

namespace Utilities.Mappers.Profiles
{
    public class ModuleFormProfile : Profile
    {
        public ModuleFormProfile()
        {
            CreateMap<ModuleForm, ModuleFormDto>()
                .ForMember(dest => dest.FormName, opt => opt.MapFrom(src => src.Form != null ? src.Form.Name : string.Empty))
                .ForMember(dest => dest.ModuleName, opt => opt.MapFrom(src => src.Module != null ? src.Module.Name : string.Empty));

            CreateMap<ModuleFormDto, ModuleForm>();

            CreateMap<UpdateModuleFormDto, ModuleForm>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<DeleteLogicalModuleFormDto, ModuleForm>();
        }
    }
}