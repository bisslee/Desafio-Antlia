using AutoMapper;
using ManualMovements.Application.Commands;
using ManualMovements.Application.Commands.Customers.AddCustomer;
using ManualMovements.Application.Commands.Customers.ChangeCustomer;
using ManualMovements.Application.Queries.Customers.GetCustomer;
using ManualMovements.Domain.Entities;
using System;

namespace ManualMovements.Application.Mapping
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Customer, GetCustomerResponse>().ReverseMap();
            CreateMap<Func<Customer, bool>, GetCustomerRequest >().ReverseMap();
            CreateMap<Customer, AddCustomerResponse>().ReverseMap();
            CreateMap<Customer, AddCustomerRequest>().ReverseMap();
            CreateMap<Customer, ChangeCustomerRequest>().ReverseMap();

            CreateMap<AddressRequest, Address>()
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Street))
                .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number))
                .ForMember(dest => dest.Complement, opt => opt.MapFrom(src => src.Complement))
                .ForMember(dest => dest.Neighborhood, opt => opt.MapFrom(src => src.Neighborhood))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
                .ForMember(dest => dest.ZipCode, opt => opt.MapFrom(src => src.ZipCode))
                .ReverseMap(); 
        }
    }
    
}
