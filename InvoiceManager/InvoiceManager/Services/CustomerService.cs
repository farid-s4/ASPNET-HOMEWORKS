using AutoMapper;
using InvoiceManager.Common;
using InvoiceManager.Data;
using InvoiceManager.DTO.CustomerDTOs;
using InvoiceManager.Models;
using InvoiceManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InvoiceManager.Services
{
    public class CustomerService(AppDbContext context, IMapper mapper) : ICustomerService
    {
        public async Task<CustomerResponseDTO> CreateAsync(CreateCustomerDTO dto)
        {
            var customer = mapper.Map<Customer>(dto);
            context.Customers.Add(customer);
            await context.SaveChangesAsync();
            return mapper.Map<CustomerResponseDTO>(customer);
        }

        public async Task<IEnumerable<CustomerResponseDTO>> GetAllAsync()
        {
            var customers = await context
                .Customers
                .Include(c => c.Invoices)
                .ToListAsync();
            return mapper.Map<IEnumerable<CustomerResponseDTO>>(customers);
        }

        public async Task<CustomerResponseDTO?> GetByIdAsync(int id)
        {
            var customer = await context
                .Customers
                .Include(c => c.Invoices)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (customer == null)
            {
                return null;
            }
            return mapper.Map<CustomerResponseDTO>(customer);
        }

        public async Task<bool> HardDeleteAsync(int id)
        {
            var customer = context
                .Customers
                .Include(c => c.Invoices)
                .FirstOrDefault(c => c.Id == id);
            if (customer == null)
            {
                return false;
            }
            if (!customer.Invoices.Any())
            {
                context.Customers.Remove(customer);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<PagedResult<CustomerResponseDTO>> GetPagedAsync(CustomerQueryParams queryParams)
        {
            queryParams.Validate();
            
            var query = context.Customers
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
            var customersDto = mapper.Map<IEnumerable<CustomerResponseDTO>>(customers);
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
            var existingCustomer = await context.Customers
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
            if (existingCustomer == null)
            {
                return false;
            }

            existingCustomer.DeletedAt = DateTimeOffset.UtcNow;

            await context.SaveChangesAsync();

            return true;
        }

        public async Task<CustomerResponseDTO?> UpdateAsync(int id, CustomerUpdateDTO dto)
        {
            var existingCustomer = await context
       .Customers
       .FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null);

            if (existingCustomer == null)
            {
                return null;
            }

            mapper.Map(dto, existingCustomer);

            await context.SaveChangesAsync();

            return mapper.Map<CustomerResponseDTO>(existingCustomer);
        }
    }
}
