using InvoiceManager.Common;
using InvoiceManager.DTO;
using InvoiceManager.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurstomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CurstomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<CustomerResponseDTO>>>> GetAll()
        {
            var customers = await _customerService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<CustomerResponseDTO>>.SuccessResult(customers, "All customers found"));
        }
        [HttpGet("paged")]
        public async Task<ActionResult<ApiResponse<PagedResult<CustomerResponseDTO>>>> GetPagedAsync([FromQuery] CustomerQueryParams queryParams)
        {
            var resp = await _customerService.GetPagedAsync(queryParams);
            return Ok(ApiResponse<PagedResult<CustomerResponseDTO>>.SuccessResult(resp, "Paged result found"));
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CustomerResponseDTO>>> GetById(int id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
                return NotFound(ApiResponse<CustomerResponseDTO>.FailureResult("Customer not found"));

            return Ok(ApiResponse<CustomerResponseDTO>.SuccessResult(customer, "Customer found"));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CustomerResponseDTO>>> Create([FromBody] CreateCustomerDTO customer)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<CreateCustomerDTO>.FailureResult("Invalid data"));

            var createdCustomer = await _customerService.CreateAsync(customer);
            
            var response =
                ApiResponse<CustomerResponseDTO>.SuccessResult(createdCustomer, "Customer created successfully");

            return CreatedAtAction(nameof(GetById), new { id = createdCustomer.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<CustomerResponseDTO>>> Update(int id,
            [FromBody] CustomerUpdateDTO customer)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<CustomerUpdateDTO>.FailureResult("Invalid data"));

            var updatedCustomer = await _customerService.UpdateAsync(id, customer);
            if (updatedCustomer == null)
                return NotFound(ApiResponse<CustomerResponseDTO>.FailureResult("Customer not found"));

            return Ok(ApiResponse<CustomerResponseDTO>.SuccessResult(updatedCustomer, "Customer updated successfully"));
        }

        [HttpDelete("soft/{id}")]
        public async Task<ActionResult<ApiResponse<object>>> SoftDelete(int id)
        {
            var result = await _customerService.SoftDeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<object>.FailureResult("Customer not found"));

            return Ok(ApiResponse<object>.SuccessResult(null, "Customer soft-deleted successfully"));
        }

        [HttpDelete("hard/{id}")]
        public async Task<ActionResult<ApiResponse<object>>> HardDelete(int id)
        {
            var result = await _customerService.HardDeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<object>.FailureResult("Customer not found"));

            return Ok(ApiResponse<object>.SuccessResult(null, "Customer hard-deleted successfully"));
        }
    }
}
