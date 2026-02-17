using AutoMapper;
using InvoiceManager.Data;
using InvoiceManager.DTO.CustomerDTOs;
using InvoiceManager.DTO.InvoiceDTOs;
using InvoiceManager.Models;
using InvoiceManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InvoiceManager.Services
{
    public class CustomerService : ICustomerService
    {
        private AppDbContext _context;
        private IMapper _mapper;
        public CustomerService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CustomerResponseDTO> CreateAsync(CreateCustomerDTO dto)
        {
            var customer = _mapper.Map<Customer>(dto);
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return _mapper.Map<CustomerResponseDTO>(customer);
        }

        public async Task<IEnumerable<CustomerResponseDTO>> GetAllAsync()
        {
            var customers = await _context
                .Customers
                .Include(c => c.Invoices)
                .ToListAsync();
            return _mapper.Map<IEnumerable<CustomerResponseDTO>>(customers);
        }

        public async Task<CustomerResponseDTO?> GetByIdAsync(int id)
        {
            var customer = await _context
                .Customers
                .Include(c => c.Invoices)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (customer == null)
                {
                return null;
            }
            return _mapper.Map<CustomerResponseDTO>(customer);
        }

        public async Task<bool> HardDeleteAsync(int id)
        {
            var customer = _context
                .Customers
                .Include(c => c.Invoices)
                .FirstOrDefault(c => c.Id == id);
            if (customer == null)
            {
                return false;
            }
            if (!customer.Invoices.Any())
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var existingCustomer = await _context.Customers
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
            if (existingCustomer == null)
            {
                return false;
            }

            existingCustomer.DeletedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<CustomerResponseDTO?> UpdateAsync(int id, CustomerUpdateDTO dto)
        {
            var customer = _mapper.Map<Customer>(dto);
            
            var existingCustomer = await _context
       .Customers
       .FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null);

            if (existingCustomer == null)
            {
                return null;
            }

            _mapper.Map(dto, existingCustomer);

            await _context.SaveChangesAsync();

            return _mapper.Map<CustomerResponseDTO>(customer);
        }
    }
}
