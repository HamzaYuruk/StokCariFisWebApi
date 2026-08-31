using AutoMapper;
using CariWebApi.Application.DTOs;
using CariWebApi.Domain.Entities;

namespace CariWebApi.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //
        CreateMap<Company, CompanyDto>();
        CreateMap<CreateCompanyDto, Company>();
        CreateMap<UpdateCompanyDto, Company>();
        //
        CreateMap<Stock,StockDto>();
        CreateMap<CreateStockDto, Stock>();
        CreateMap<UpdateStockDto, Stock>();
        
        //
        
    }
}