using AutoMapper;
using InvoiceManager.Common;
using InvoiceManager.Data;
using InvoiceManager.DTO.InvoiceDTOs;
using InvoiceManager.Mapping;
using InvoiceManager.Models;
using InvoiceManager.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InvoiceManager.Services
{
    public class InvoiceService : IInvoiceService
    {
        private AppDbContext _context;
        private IMapper _mapper;
        public InvoiceService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<InvoiceResponseDTO> CreateAsync(CreateInvoiceDTO dto)
        {
            var invoice = _mapper.Map<Invoice>(dto);
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            return _mapper.Map<InvoiceResponseDTO>(invoice);
        }

        public async Task<PagedResult<InvoiceResponseDTO>> GetPagedAsync(InvoicesQueryParams queryParams)
        {
            queryParams.Validate();
            
            var query = _context.Invoices
                .Include(x => x.InvoiceRows)
                .AsQueryable();

            if (queryParams.CustomerId.HasValue)
            {
                query = query.Where(x => x.CustomerId == queryParams.CustomerId);
            }

            if (!string.IsNullOrEmpty(queryParams.SortByStatus))
            {
                if (Enum.TryParse<InvoiceStatus>(queryParams.SortByStatus, out InvoiceStatus status))
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
            if (!string.IsNullOrWhiteSpace(queryParams.Sort))
                query = ApplySorting(query, queryParams.Sort, queryParams.SortDirection!);
            else
                query = query.OrderByDescending(t => t.CreatedAt);
            
            var totalCount = await query.CountAsync();
            var skip = (queryParams.PageNumber - 1) * queryParams.PageSize;
            var invoices = await query.Skip(skip).Take(queryParams.PageSize).ToListAsync();
            var invoicesDto = _mapper.Map<IEnumerable<InvoiceResponseDTO>>(invoices);
            return PagedResult<InvoiceResponseDTO>.Create(
                invoicesDto,
                totalCount,
                queryParams.PageNumber,
                queryParams.PageSize);
        }
        public async Task<IEnumerable<InvoiceResponseDTO>> GetAllAsync()
        {
            var invoices = await _context
                .Invoices
                .Include(x => x.InvoiceRows)
                .ToListAsync();
            return _mapper.Map<IEnumerable<InvoiceResponseDTO>>(invoices);
        }

        public async Task<InvoiceResponseDTO?> GetByIdAsync(int id)
        {
            var invoice = await _context.Invoices
                .Include(x => x.InvoiceRows)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (invoice == null)
                {
                return null;
            }
            return _mapper.Map<InvoiceResponseDTO>(invoice);
        }

        public async Task<InvoiceResponseDTO?> UpdateAsync(int id, InvoiceUpdateDTO dto)
        {
            var existingInvoice = await _context.Invoices
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);

            if (existingInvoice == null)
            {
                return null;
            }

            _mapper.Map(dto, existingInvoice);

            await _context.SaveChangesAsync();
            return _mapper.Map<InvoiceResponseDTO>(existingInvoice);
        }

        public async Task<bool> UpdateStatusAsync(int id, InvoiceStatus newStatus)
        {
            var existingInvoice = await _context.Invoices
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
            if (existingInvoice == null)
            {
                return false;
            }

            existingInvoice.Status = newStatus;
            existingInvoice.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var existingInvoice = await _context.Invoices
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
            if (existingInvoice == null)
            {
                return false;
            }

            existingInvoice.DeletedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> HardDeleteAsync(int id)
        {
            var existingInvoice = await _context.Invoices
                .FirstOrDefaultAsync(x => x.Id == id);
            if (existingInvoice == null)
            {
                return false;
            }
            if (existingInvoice.Status != InvoiceStatus.Sent)
            {
                _context.Invoices.Remove(existingInvoice);
                await _context.SaveChangesAsync();
                return true;
            }
            await _context.SaveChangesAsync();
            return false;
        }
        private IQueryable<Invoice> ApplySorting(
            IQueryable<Invoice> query,
            string sortField,
            string sortDirection)
        {
            var isDescending = sortDirection?.ToLower() == "desc";

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
