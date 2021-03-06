using AutoMapper;
using CompanyEmployees.Dtos;
using CompanyEmployees.Models;

namespace CompanyEmployees.Mapping
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<Company, CompanyDto>()
				.ForMember(c => c.FullAddress,
					opt => opt.MapFrom(x => string.Join(' ', x.Address, x.Country)));

			CreateMap<Employee, EmployeeDto>();

			CreateMap<CompanyForCreationDto, Company>();
			CreateMap<CompanyForUpdateDto, Company>().ReverseMap();

			CreateMap<EmployeeForUpdateDto, Employee>().ReverseMap();
			CreateMap<EmployeeForCreationDto, Employee>();

			CreateMap<UserForRegistrationDto, User>();
		}
	}
}