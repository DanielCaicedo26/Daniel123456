using AutoMapper;
using Entity.Model;
using Entity.Dtos.PermissionDTO;

namespace Utilities.Mappers.Profiles
{
    public class PermissionProfile : Profile
    {
        public PermissionProfile()
        {
            CreateMap<Permission, PermissionDto>().ReverseMap();
            CreateMap<UpdatePermissionDto, Permission>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<DeleteLogicalPermissionDto, Permission>();
        }
    }
}