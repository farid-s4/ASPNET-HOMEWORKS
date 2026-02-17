using InvoiceManager.DTO.CustomerDTOs;
using InvoiceManager.Models;
using InvoiceManager.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurstomersController : ControllerBase
    {
        private ICustomerService _customerService;

        public CurstomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerResponseDTO>>> GetAll()
        {
            var customers = await _customerService.GetAllAsync();
            return Ok(customers);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponseDTO>> GetById(int id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
                return NotFound();
            return Ok(customer);
        }
        [HttpPost]
        public async Task<ActionResult<CustomerResponseDTO>> Create([FromBody] CreateCustomerDTO customer)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var createdCustomer = await _customerService.CreateAsync(customer);
            return CreatedAtAction(nameof(GetById), new { id = createdCustomer.Id }, createdCustomer);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<CustomerResponseDTO>> Update(int id, [FromBody]CustomerUpdateDTO customer)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var updatedCustomer = await _customerService.UpdateAsync(id, customer);
            if (updatedCustomer == null)
                return NotFound();
            return Ok(updatedCustomer);
        }
        [HttpDelete("soft/{id}")]
        public async Task<ActionResult> SoftDelete(int id)
        {
            var result = await _customerService.SoftDeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
        [HttpDelete("hard/{id}")]
        public async Task<ActionResult> HardDelete(int id)
        {
            var result = await _customerService.HardDeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}
