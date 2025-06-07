using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Mappers.Profiles
{
    using AutoMapper;
    using Entity.Model;
    using Entity.Dtos.DepartmentDTO;

    namespace Utilities.Mappers.Profiles
    {
        public class DepartmentProfile : Profile
        {
            public DepartmentProfile()
            {
                CreateMap<Department, DepartmentDto>().ReverseMap();
                CreateMap<UpdateDepartmentDto, Department>()
                    .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
                CreateMap<DeleteLogicalDepartmentDto, Department>();
            }
        }
    }
}
