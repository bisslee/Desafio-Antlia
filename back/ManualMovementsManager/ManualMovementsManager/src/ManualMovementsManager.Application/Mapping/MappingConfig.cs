using AutoMapper;
using ManualMovementsManager.Application.Commands;
using ManualMovementsManager.Application.Commands.Customers.AddCustomer;
using ManualMovementsManager.Application.Commands.Customers.ChangeCustomer;
using ManualMovementsManager.Application.Commands.ManualMovements.AddManualMovement;
using ManualMovementsManager.Application.Commands.ManualMovements.ChangeManualMovement;
using ManualMovementsManager.Application.Commands.ProductCosifs.AddProductCosif;
using ManualMovementsManager.Application.Commands.ProductCosifs.ChangeProductCosif;
using ManualMovementsManager.Application.Commands.Products.AddProduct;
using ManualMovementsManager.Application.Commands.Products.ChangeProduct;
using ManualMovementsManager.Application.DTOs;
using ManualMovementsManager.Application.Queries.Customers.GetCustomer;
using ManualMovementsManager.Application.Queries.ManualMovements.GetManualMovement;
using ManualMovementsManager.Application.Queries.ProductCosifs.GetProductCosif;
using ManualMovementsManager.Application.Queries.Products.GetProduct;
using ManualMovementsManager.Domain.Entities;
using System;

namespace ManualMovementsManager.Application.Mapping
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {

            // Customer mappings
            CreateMap<Customer, GetCustomerResponse>().ReverseMap();
            CreateMap<Func<Customer, bool>, GetCustomerRequest >().ReverseMap();
            CreateMap<Customer, AddCustomerResponse>().ReverseMap();
            CreateMap<Customer, AddCustomerRequest>().ReverseMap();
            CreateMap<Customer, ChangeCustomerRequest>().ReverseMap();
            CreateMap<Customer, CustomerDto>();

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

            // ManualMovementsManager mappings
            CreateMap<ManualMovement, GetManualMovementResponse>().ReverseMap();
            CreateMap<Func<ManualMovement, bool>, GetManualMovementRequest>().ReverseMap();
            CreateMap<ManualMovement, AddManualMovementResponse>().ReverseMap();
            CreateMap<ManualMovement, AddManualMovementRequest>().ReverseMap();
            CreateMap<ManualMovement, ChangeManualMovementRequest>().ReverseMap();
            CreateMap<ManualMovement, ManualMovementDto>();

            // Product mappings
            CreateMap<Product, GetProductResponse>().ReverseMap();
            CreateMap<Func<Product, bool>, GetProductRequest>().ReverseMap();
            CreateMap<Product, AddProductResponse>().ReverseMap();
            CreateMap<Product, AddProductRequest>().ReverseMap();
            CreateMap<Product, ChangeProductRequest>().ReverseMap();
            CreateMap<Product, ProductDto>();

            // ProductCosifs
            CreateMap<ProductCosif, AddProductCosifRequest>().ReverseMap();
            CreateMap<ProductCosif, AddProductCosifResponse>().ReverseMap();
            CreateMap<ProductCosif, ChangeProductCosifRequest>().ReverseMap();
            CreateMap<ProductCosif, GetProductCosifResponse>().ReverseMap();
            CreateMap<ProductCosif, ProductCosifDto>();

        }
    }
    
}
