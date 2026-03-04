using AutoMapper;
using InvoiceManager.Common;
using InvoiceManager.Data;
using InvoiceManager.DTO;
using InvoiceManager.Models;
using InvoiceManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InvoiceManager.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public CustomerService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<CustomerResponseDTO> CreateAsync(CreateCustomerDTO dto)
        {
            var emailExists = await _context.Customers
                .AnyAsync(c => c.Email == dto.Email && c.DeletedAt == null);

            if (emailExists)
                throw new ArgumentException($"Client with email: {dto.Email} exists.");
            
            var customer = _mapper.Map<Customer>(dto);
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return _mapper.Map<CustomerResponseDTO>(customer);
        }

        public async Task<IEnumerable<CustomerResponseDTO>> GetAllAsync()
        {
            var customers = await _context.Customers
                .Where(c => !c.DeletedAt.HasValue)
                .Include(c => c.Invoices)
                .ToListAsync();
            return _mapper.Map<IEnumerable<CustomerResponseDTO>>(customers);
        }

        public async Task<CustomerResponseDTO?> GetByIdAsync(int id)
        {
            var customer = await _context.Customers
                .Where(c => !c.DeletedAt.HasValue)
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

        public async Task<PagedResult<CustomerResponseDTO>> GetPagedAsync(CustomerQueryParams queryParams)
        {
            queryParams.Validate();
            
            var query = _context.Customers
                .Where(c => !c.DeletedAt.HasValue)
                .Include(c => c.Invoices)
                .AsQueryable();
            if (queryParams.InvoiceId.HasValue)
            {
                query = query.Where(c => c.Id == queryParams.InvoiceId);
            }
            if (!string.IsNullOrWhiteSpace(queryParams.Search))
            {
                var searchTerm = queryParams.Search.ToLower();

                query = query.Where(t => t.Phone != null && (t.Address != null && (t.Address.ToLower().Contains(searchTerm) ||
                                                                 t.Email.ToLower().Contains(searchTerm)) 
                                                             || t.Name.ToLower().Contains(searchTerm) || t.Phone.ToLower().Contains(searchTerm)));
            }

            if (!string.IsNullOrWhiteSpace(queryParams.Sort))
                query = ApplySorting(query, queryParams.Sort, queryParams.SortDirection!);
            else
                query = query.OrderByDescending(t => t.CreatedAt);
            
            var totalCount = await query.CountAsync();
            var skip = (queryParams.PageNumber - 1) * queryParams.PageSize;
            var customers = await query.Skip(skip).Take(queryParams.PageSize).ToListAsync();
            var customersDto = _mapper.Map<IEnumerable<CustomerResponseDTO>>(customers);
            return PagedResult<CustomerResponseDTO>.Create(
                customersDto,
                totalCount,
                queryParams.PageNumber,
                queryParams.PageSize);
        }

        private IQueryable<Customer> ApplySorting(IQueryable<Customer> query, string queryParamsSort, string queryParamsSortDirection)
        {
            var isDescending = queryParamsSortDirection.ToLower() == "desc";

            return queryParamsSort.ToLower() switch
            {
                "name" => isDescending
                    ? query.OrderByDescending(t=> t.Name)
                    : query.OrderBy(t => t.Name),

                "createdat" => isDescending
                    ? query.OrderByDescending(t => t.CreatedAt)
                    : query.OrderBy(t => t.CreatedAt),

                "email" => isDescending
                    ? query.OrderByDescending(t => t.Email)
                    : query.OrderBy(t => t.Email),

                "address" => isDescending
                    ? query.OrderByDescending(t => t.Address)
                    : query.OrderBy(t => t.Address),

                _ => query.OrderByDescending(t => t.CreatedAt)
            };
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
            var existingCustomer = await _context
       .Customers
       .FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null);

            if (existingCustomer == null)
            {
                return null;
            }

            _mapper.Map(dto, existingCustomer);

            await _context.SaveChangesAsync();

            return _mapper.Map<CustomerResponseDTO>(existingCustomer);
        }
    }
}
