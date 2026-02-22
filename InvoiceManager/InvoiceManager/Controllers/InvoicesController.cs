using InvoiceManager.DTO.InvoiceDTOs;
using InvoiceManager.Models;
using InvoiceManager.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using InvoiceManager.Common;

namespace InvoiceManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoicesController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<InvoiceResponseDTO>>>> GetAll()
        {
            var invoices = await _invoiceService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<InvoiceResponseDTO>>.SuccessResult(invoices, "All invoices found"));
        }

        [HttpGet("paged")]
        public async Task<ActionResult<ApiResponse<PagedResult<InvoiceResponseDTO>>>> GetPagedAsync([FromQuery] InvoicesQueryParams queryParams)
        {
            var resp = await _invoiceService.GetPagedAsync(queryParams);
            return Ok(ApiResponse<PagedResult<InvoiceResponseDTO>>.SuccessResult(resp, "Paged result found"));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<InvoiceResponseDTO>>> GetById(int id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);
            if (invoice == null)
                return NotFound(ApiResponse<InvoiceResponseDTO>.FailureResult("Invoice not found"));
            
            return Ok(ApiResponse<InvoiceResponseDTO>.SuccessResult(invoice, "Invoice found"));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<InvoiceResponseDTO>>> Create([FromBody] CreateInvoiceDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.FailureResult("Validation failed"));

            var createdInvoice = await _invoiceService.CreateAsync(dto);
            var response = ApiResponse<InvoiceResponseDTO>.SuccessResult(createdInvoice, "Invoice created successfully");
            
            return CreatedAtAction(nameof(GetById), new { id = createdInvoice.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<InvoiceResponseDTO>>> Update(int id, [FromBody] InvoiceUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.FailureResult("Validation failed"));

            var updatedInvoice = await _invoiceService.UpdateAsync(id, dto);
            if (updatedInvoice == null)
                return NotFound(ApiResponse<InvoiceResponseDTO>.FailureResult("Invoice not found"));

            return Ok(ApiResponse<InvoiceResponseDTO>.SuccessResult(updatedInvoice, "Invoice updated successfully"));
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateStatus(int id, [FromBody] InvoiceStatus newStatus)
        {
            var result = await _invoiceService.UpdateStatusAsync(id, newStatus);
            if (!result)
                return NotFound(ApiResponse<object>.FailureResult("Invoice not found"));

            return Ok(ApiResponse<object>.SuccessResult(null, "Status updated successfully"));
        }

        [HttpDelete("soft/{id}")]
        public async Task<ActionResult<ApiResponse<object>>> SoftDelete(int id)
        {
            var result = await _invoiceService.SoftDeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<object>.FailureResult("Invoice not found"));

            return Ok(ApiResponse<object>.SuccessResult(null, "Invoice soft-deleted successfully"));
        }

        [HttpDelete("hard/{id}")]
        public async Task<ActionResult<ApiResponse<object>>> HardDelete(int id)
        {
            var result = await _invoiceService.HardDeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<object>.FailureResult("Invoice not found"));

            return Ok(ApiResponse<object>.SuccessResult(null, "Invoice permanently deleted"));
        }
    }
}