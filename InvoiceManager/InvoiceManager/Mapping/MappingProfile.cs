using AutoMapper;
using InvoiceManager.DTO;
using InvoiceManager.Models;

namespace InvoiceManager.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateCustomerDTO, Customer>()
                .ForMember(
                dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest=> dest.CreatedAt, opt=> opt.MapFrom(_=> DateTimeOffset.UtcNow));
            CreateMap<Customer, CustomerResponseDTO>()
                .ForMember(
                dest => dest.InvoicesCount,
                opt => opt.MapFrom(src => src.Invoices.Count));
            CreateMap<CustomerUpdateDTO, Customer>()
                .ForMember(
                dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest=>dest.UpdatedAt, opt=> opt.MapFrom(_=> DateTimeOffset.UtcNow));

            //Invoices
            CreateMap<CreateInvoiceRowDTO, InvoiceRow>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.InvoiceId, opt => opt.Ignore())
                .ForMember(dest => dest.Invoice, opt => opt.Ignore())
                .ForMember(dest => dest.Amount, opt => opt.Ignore());
            CreateMap<CreateInvoiceDTO, Invoice>()
                .ForMember(
                dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTimeOffset.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => InvoiceStatus.Draft))
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore()) 
                .ForMember(dest => dest.Customer, opt => opt.Ignore());
            ;
            CreateMap<CustomerUpdateDTO, InvoiceRow>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.InvoiceId, opt => opt.Ignore())
                .ForMember(dest => dest.Invoice, opt => opt.Ignore());

            CreateMap<InvoiceUpdateDTO, Invoice>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())   
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())   
                .ForMember(dest => dest.Status, opt => opt.Ignore())    
                .ForMember(dest => dest.InvoiceRows, opt => opt.Ignore())       
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore()) 
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTimeOffset.UtcNow));      
            CreateMap<Invoice, InvoiceResponseDTO>();
            CreateMap<InvoiceRow, InvoiceRowResponseDTO>();


        }
    }
}
