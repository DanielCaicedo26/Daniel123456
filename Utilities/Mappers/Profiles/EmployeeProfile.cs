using AutoMapper;
using Entity.Model;
using Entity.Dtos.EmployeeDTO;

namespace Utilities.Mappers.Profiles
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<Employee, EmployeeDto>().ReverseMap();
            CreateMap<UpdateEmployeeDto, Employee>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<DeleteLogicalEmployeeDto, Employee>();
        }
    }
}