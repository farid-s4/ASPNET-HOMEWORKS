using InvoiceManager.Common;
using InvoiceManager.DTO;
using InvoiceManager.Models;

namespace InvoiceManager.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<IEnumerable<InvoiceResponseDTO>> GetAllAsync();
        Task<InvoiceResponseDTO?> GetByIdAsync(int id);
        Task<PagedResult<InvoiceResponseDTO>> GetPagedAsync(InvoicesQueryParams queryParams);
        Task<bool> UpdateStatusAsync(int id, InvoiceStatus newStatus);
        Task<InvoiceResponseDTO> CreateAsync(CreateInvoiceDTO dto);
        Task<InvoiceResponseDTO?> UpdateAsync(int id, InvoiceUpdateDTO dto); // Можно редактировать только не отправленные инвойсы 
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> HardDeleteAsync(int id);
        Task<InvoiceFileDTO?> DownloadInvoice(int id);
    }
}
