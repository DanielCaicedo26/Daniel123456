using AutoMapper;
using Entity.Model;
using Entity.Dtos.RolFormPermissionDTO;

namespace Utilities.Mappers.Profiles
{
    public class RolFormPermissionProfile : Profile
    {
        public RolFormPermissionProfile()
        {
            CreateMap<RolFormPermission, RolFormPermissionDto>()
                .ForMember(dest => dest.RolName, opt => opt.MapFrom(src => src.Rol != null ? src.Rol.Name : string.Empty))
                .ForMember(dest => dest.PermissionName, opt => opt.MapFrom(src => src.Permission != null ? src.Permission.Name : string.Empty));

            CreateMap<RolFormPermissionDto, RolFormPermission>();

            CreateMap<UpdateRolFormPermissionDto, RolFormPermission>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<DeleteLogicalRolFormPermissionDto, RolFormPermission>();
        }
    }
}