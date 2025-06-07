using AutoMapper;
using Entity.Model;
using Entity.Dtos.FormDTO;

namespace Utilities.Mappers.Profiles
{
    public class FormProfile : Profile
    {
        public FormProfile()
        {
            CreateMap<Form, FormDto>().ReverseMap();
            CreateMap<UpdateFormDto, Form>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<DeleteLogicalFormDto, Form>();
        }
    }
}