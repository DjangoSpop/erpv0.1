using Motel.Application.DTOs;

namespace Motel.Application.Interfaces;

public interface IClientsService
{
    Task<IEnumerable<ClientDto>> GetAllAsync();
    Task<ClientDto?> GetByIdAsync(Guid id);
    Task<ClientDto> CreateAsync(CreateClientDto dto);
    Task<ClientDto> UpdateAsync(Guid id, UpdateClientDto dto);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<ClientDto>> SearchAsync(string query);
}
