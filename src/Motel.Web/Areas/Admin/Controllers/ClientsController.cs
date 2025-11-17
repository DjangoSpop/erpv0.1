using Microsoft.AspNetCore.Mvc;
using Motel.Application.DTOs;
using Motel.Application.Interfaces;

namespace Motel.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class ClientsController : Controller
{
    private readonly IClientsService _clientsService;

    public ClientsController(IClientsService clientsService)
    {
        _clientsService = clientsService;
    }

    public async Task<IActionResult> Index(string? search)
    {
        IEnumerable<ClientDto> clients;

        if (!string.IsNullOrEmpty(search))
            clients = await _clientsService.SearchAsync(search);
        else
            clients = await _clientsService.GetAllAsync();

        ViewBag.Search = search;
        return View(clients);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateClientDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        try
        {
            await _clientsService.CreateAsync(dto);
            TempData["Success"] = "تم إضافة النزيل بنجاح";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(dto);
        }
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var client = await _clientsService.GetByIdAsync(id);
        if (client == null)
            return NotFound();

        var dto = new UpdateClientDto
        {
            FullName = client.FullName,
            NationalIdOrPassport = client.NationalIdOrPassport,
            Phone = client.Phone,
            Email = client.Email,
            Notes = client.Notes
        };

        ViewBag.ClientId = id;
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateClientDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ClientId = id;
            return View(dto);
        }

        try
        {
            await _clientsService.UpdateAsync(id, dto);
            TempData["Success"] = "تم تحديث بيانات النزيل بنجاح";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            ViewBag.ClientId = id;
            return View(dto);
        }
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var client = await _clientsService.GetByIdAsync(id);
        if (client == null)
            return NotFound();

        return View(client);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _clientsService.DeleteAsync(id);
            TempData["Success"] = "تم حذف النزيل بنجاح";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
