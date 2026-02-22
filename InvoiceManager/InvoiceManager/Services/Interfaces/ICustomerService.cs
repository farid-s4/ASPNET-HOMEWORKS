using InvoiceManager.Common;
using InvoiceManager.DTO.CustomerDTOs;
using InvoiceManager.DTO.InvoiceDTOs;
using InvoiceManager.Models;

namespace InvoiceManager.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerResponseDTO>> GetAllAsync();
        Task<CustomerResponseDTO?> GetByIdAsync(int id);
        Task<CustomerResponseDTO> CreateAsync(CreateCustomerDTO dto);
        Task<CustomerResponseDTO?> UpdateAsync(int id, CustomerUpdateDTO dto);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> HardDeleteAsync(int id);
        Task<PagedResult<CustomerResponseDTO>> GetPagedAsync(CustomerQueryParams queryParams);
    }
}
