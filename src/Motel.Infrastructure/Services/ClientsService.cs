using Microsoft.EntityFrameworkCore;
using Motel.Application.DTOs;
using Motel.Application.Interfaces;
using Motel.Domain.Entities;
using Motel.Infrastructure.Data;

namespace Motel.Infrastructure.Services;

public class ClientsService : IClientsService
{
    private readonly AppDbContext _context;

    public ClientsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ClientDto>> GetAllAsync()
    {
        return await _context.Clients
            .Select(c => new ClientDto
            {
                Id = c.Id,
                FullName = c.FullName,
                NationalIdOrPassport = c.NationalIdOrPassport,
                Phone = c.Phone,
                Email = c.Email,
                Notes = c.Notes
            })
            .ToListAsync();
    }

    public async Task<ClientDto?> GetByIdAsync(Guid id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null) return null;

        return new ClientDto
        {
            Id = client.Id,
            FullName = client.FullName,
            NationalIdOrPassport = client.NationalIdOrPassport,
            Phone = client.Phone,
            Email = client.Email,
            Notes = client.Notes
        };
    }

    public async Task<ClientDto> CreateAsync(CreateClientDto dto)
    {
        var client = new Client
        {
            Id = Guid.NewGuid(),
            FullName = dto.FullName,
            NationalIdOrPassport = dto.NationalIdOrPassport,
            Phone = dto.Phone,
            Email = dto.Email,
            Notes = dto.Notes
        };

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        return new ClientDto
        {
            Id = client.Id,
            FullName = client.FullName,
            NationalIdOrPassport = client.NationalIdOrPassport,
            Phone = client.Phone,
            Email = client.Email,
            Notes = client.Notes
        };
    }

    public async Task<ClientDto> UpdateAsync(Guid id, UpdateClientDto dto)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null)
            throw new InvalidOperationException("النزيل غير موجود");

        client.FullName = dto.FullName;
        client.NationalIdOrPassport = dto.NationalIdOrPassport;
        client.Phone = dto.Phone;
        client.Email = dto.Email;
        client.Notes = dto.Notes;

        await _context.SaveChangesAsync();

        return new ClientDto
        {
            Id = client.Id,
            FullName = client.FullName,
            NationalIdOrPassport = client.NationalIdOrPassport,
            Phone = client.Phone,
            Email = client.Email,
            Notes = client.Notes
        };
    }

    public async Task DeleteAsync(Guid id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null)
            throw new InvalidOperationException("النزيل غير موجود");

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ClientDto>> SearchAsync(string query)
    {
        return await _context.Clients
            .Where(c => c.FullName.Contains(query) ||
                       c.NationalIdOrPassport.Contains(query) ||
                       c.Phone.Contains(query))
            .Select(c => new ClientDto
            {
                Id = c.Id,
                FullName = c.FullName,
                NationalIdOrPassport = c.NationalIdOrPassport,
                Phone = c.Phone,
                Email = c.Email,
                Notes = c.Notes
            })
            .ToListAsync();
    }
}
