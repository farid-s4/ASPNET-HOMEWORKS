using AutoMapper;
using InvoiceManager.Common;
using InvoiceManager.Data;
using InvoiceManager.DTO;
using InvoiceManager.Models;
using InvoiceManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InvoiceManager.Services
{
    public class InvoiceService(AppDbContext context, IMapper mapper) : IInvoiceService
    {
        public async Task<InvoiceResponseDTO> CreateAsync(CreateInvoiceDTO dto)
        {
            var customerExists = await context.Customers
                .AnyAsync(c => c.Id == dto.CustomerId && c.DeletedAt == null);

            if (!customerExists)
            {
                throw new KeyNotFoundException("Customer not found");
            }
            var invoice = mapper.Map<Invoice>(dto);
            
            foreach (var row in invoice.InvoiceRows)
            {
                row.Amount = row.Quantity * row.Rate;
            }
            
            invoice.TotalAmount = invoice.InvoiceRows.Sum(x => x.Amount);

            invoice.Status = InvoiceStatus.Created;
            invoice.CreatedAt = DateTimeOffset.UtcNow;
            invoice.UpdatedAt = DateTimeOffset.UtcNow;

            context.Invoices.Add(invoice);
            await context.SaveChangesAsync();
            
            var saved = await context.Invoices
                .Include(i => i.InvoiceRows)
                .FirstAsync(i => i.Id == invoice.Id);
            
            return mapper.Map<InvoiceResponseDTO>(saved);
        }

        public async Task<PagedResult<InvoiceResponseDTO>> GetPagedAsync(InvoicesQueryParams queryParams)
        {
            queryParams.Validate();
            
            var query = context.Invoices
                .Where(i => !i.DeletedAt.HasValue)
                .Include(i => i.InvoiceRows)
                .AsQueryable();

            if (queryParams.CustomerId.HasValue)
            {
                query = query.Where(x => x.CustomerId == queryParams.CustomerId);
            }

            if (!string.IsNullOrEmpty(queryParams.SortByStatus))
            {
                if (Enum.TryParse(queryParams.SortByStatus, out InvoiceStatus status))
                {
                    query = query.Where(x => x.Status == status);
                }
            }

            if (!string.IsNullOrWhiteSpace(queryParams.Search))
            {
                var search = queryParams.Search;
                query = query.Where(x => x.Comment.ToLower().Contains(search) ||
                                                 x.Comment.ToLower().Contains(search));
            }
            query = !string.IsNullOrWhiteSpace(queryParams.Sort)
                ? ApplySorting(query, queryParams.Sort, queryParams.SortDirection!)
                : query.OrderByDescending(t => t.CreatedAt);
            
            var totalCount = await query.CountAsync();
            var skip = (queryParams.PageNumber - 1) * queryParams.PageSize;
            var invoices = await query.Skip(skip).Take(queryParams.PageSize).ToListAsync();
            var invoicesDto = mapper.Map<IEnumerable<InvoiceResponseDTO>>(invoices);
            return PagedResult<InvoiceResponseDTO>.Create(
                invoicesDto,
                totalCount,
                queryParams.PageNumber,
                queryParams.PageSize);
        }
        public async Task<IEnumerable<InvoiceResponseDTO>> GetAllAsync()
        {
            var invoices = await context.Invoices
                .Where(i => !i.DeletedAt.HasValue)
                .Include(i => i.InvoiceRows)
                .ToListAsync();
            return mapper.Map<IEnumerable<InvoiceResponseDTO>>(invoices);
        }

        public async Task<InvoiceResponseDTO?> GetByIdAsync(int id)
        {
            var invoice = await context.Invoices
                .Where(i => !i.DeletedAt.HasValue)
                .Include(i => i.InvoiceRows)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (invoice == null)
            {
                return null;
            }
            return mapper.Map<InvoiceResponseDTO>(invoice);
        }

        public async Task<InvoiceResponseDTO?> UpdateAsync(int id, InvoiceUpdateDTO dto)
        {
            var existingInvoice = await context.Invoices
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);

            if (existingInvoice == null)
            {
                return null;
            }

            if (existingInvoice.Status != InvoiceStatus.Created)
            {
                return null;
            }

            mapper.Map(dto, existingInvoice);

            await context.SaveChangesAsync();
            return mapper.Map<InvoiceResponseDTO>(existingInvoice);
        }

        public async Task<bool> UpdateStatusAsync(int id, InvoiceStatus newStatus)
        {
            var existingInvoice = await context.Invoices
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
            if (existingInvoice == null)
            {
                return false;
            }

            existingInvoice.Status = newStatus;
            existingInvoice.UpdatedAt = DateTimeOffset.UtcNow;

            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var existingInvoice = await context.Invoices
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
            if (existingInvoice == null)
            {
                return false;
            }

            existingInvoice.DeletedAt = DateTimeOffset.UtcNow;

            await context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> HardDeleteAsync(int id)
        {
            var existingInvoice = await context.Invoices
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existingInvoice == null)
                return false;

            if (existingInvoice.Status != InvoiceStatus.Created)
                return false;

            context.Invoices.Remove(existingInvoice);
            await context.SaveChangesAsync();
            return true;
        }
        private IQueryable<Invoice> ApplySorting(
            IQueryable<Invoice> query,
            string sortField,
            string sortDirection)
        {
            var isDescending = sortDirection.ToLower() == "desc";

            return sortField.ToLower() switch
            {
                "title" => isDescending
                    ? query.OrderByDescending(t=> t.Comment)
                    : query.OrderBy(t => t.Comment),

                "createdat" => isDescending
                    ? query.OrderByDescending(t => t.CreatedAt)
                    : query.OrderBy(t => t.CreatedAt),

                "status" => isDescending
                    ? query.OrderByDescending(t => t.Status)
                    : query.OrderBy(t => t.Status),

                "priority" => isDescending
                    ? query.OrderByDescending(t => t.Status)
                    : query.OrderBy(t => t.Status),
                
                _ => query.OrderByDescending(t => t.CreatedAt)
            };
        }
    }
}
