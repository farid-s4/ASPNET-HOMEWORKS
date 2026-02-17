using AutoMapper;
using InvoiceManager.Data;
using InvoiceManager.DTO.InvoiceDTOs;
using InvoiceManager.Mapping;
using InvoiceManager.Models;
using InvoiceManager.Services.Interfaces;
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
    }
}
