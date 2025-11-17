using Microsoft.AspNetCore.Mvc;
using Motel.Application.DTOs;
using Motel.Application.Interfaces;

namespace Motel.Web.Controllers.Api.V1;

[ApiController]
[Route("api/v1/invoices")]
public class InvoicesApiController : ControllerBase
{
    private readonly IInvoicesService _invoicesService;

    public InvoicesApiController(IInvoicesService invoicesService)
    {
        _invoicesService = invoicesService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<InvoiceDto>> GetById(Guid id)
    {
        var invoice = await _invoicesService.GetByIdAsync(id);
        if (invoice == null)
            return NotFound(new { message = "الفاتورة غير موجودة" });

        return Ok(invoice);
    }

    [HttpPost("{id}/issue")]
    public async Task<ActionResult<InvoiceDto>> Issue(Guid id)
    {
        try
        {
            var invoice = await _invoicesService.IssueAsync(id);
            return Ok(invoice);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/mark-paid")]
    public async Task<ActionResult<InvoiceDto>> MarkPaid(Guid id, [FromBody] MarkPaidDto dto)
    {
        try
        {
            var invoice = await _invoicesService.MarkAsPaidAsync(id, dto.PaymentMethod);
            return Ok(invoice);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
