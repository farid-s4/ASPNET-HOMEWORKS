using InvoiceManager.DTO.InvoiceDTOs;
using InvoiceManager.Models;
using InvoiceManager.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace InvoiceManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private IInvoiceService _invoiceService;

        public InvoicesController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InvoiceResponseDTO>>> GetAll()
        {
            var invoices = await _invoiceService.GetAllAsync();
            return Ok(invoices);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceResponseDTO>> GetById(int id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);
            if (invoice == null)
                return NotFound();
            return Ok(invoice);
        }
        [HttpPost]
        public async Task<ActionResult<InvoiceResponseDTO>> Create([FromBody] CreateInvoiceDTO dto)
        {
            var createdInvoice = await _invoiceService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdInvoice.Id }, createdInvoice);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<InvoiceResponseDTO>> Update(int id, [FromBody] InvoiceUpdateDTO dto)
        {
            var updatedInvoice = await _invoiceService.UpdateAsync(id, dto);
            if (updatedInvoice == null)
                return NotFound();
            return Ok(updatedInvoice);
        }
        [HttpPatch("{id}/status")]
        public async Task<ActionResult> UpdateStatus(int id, [FromBody] InvoiceStatus newStatus)
        {
            var result = await _invoiceService.UpdateStatusAsync(id, newStatus);
            if (!result)
                return NotFound();
            return NoContent();
        }


        [HttpDelete("soft/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _invoiceService.SoftDeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }

        [HttpDelete("hard/{id}")]
        public async Task<IActionResult> HardDelete(int id)
        {
            var result = await _invoiceService.HardDeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}