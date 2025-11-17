using Microsoft.AspNetCore.Mvc;
using Motel.Application.DTOs;
using Motel.Application.Interfaces;

namespace Motel.Web.Controllers.Api.V1;

[ApiController]
[Route("api/v1/clients")]
public class ClientsApiController : ControllerBase
{
    private readonly IClientsService _clientsService;

    public ClientsApiController(IClientsService clientsService)
    {
        _clientsService = clientsService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClientDto>>> GetAll([FromQuery] string? query = null)
    {
        if (!string.IsNullOrEmpty(query))
        {
            var results = await _clientsService.SearchAsync(query);
            return Ok(results);
        }

        var clients = await _clientsService.GetAllAsync();
        return Ok(clients);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClientDto>> GetById(Guid id)
    {
        var client = await _clientsService.GetByIdAsync(id);
        if (client == null)
            return NotFound(new { message = "النزيل غير موجود" });

        return Ok(client);
    }

    [HttpPost]
    public async Task<ActionResult<ClientDto>> Create([FromBody] CreateClientDto dto)
    {
        try
        {
            var client = await _clientsService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ClientDto>> Update(Guid id, [FromBody] UpdateClientDto dto)
    {
        try
        {
            var client = await _clientsService.UpdateAsync(id, dto);
            return Ok(client);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _clientsService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
