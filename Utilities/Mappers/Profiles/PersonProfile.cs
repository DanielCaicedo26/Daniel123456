using AutoMapper;
using Entity.Model;
using Entity.Dtos.PersonDTO;

namespace Utilities.Mappers.Profiles
{
    public class PersonProfile : Profile
    {
        public PersonProfile()
        {
            CreateMap<Person, PersonDto>().ReverseMap();
            CreateMap<UpdatePersonDto, Person>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<DeleteLogicalPersonDto, Person>();
        }
    }
}