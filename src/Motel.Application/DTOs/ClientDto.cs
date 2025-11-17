namespace Motel.Application.DTOs;

public class ClientDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = default!;
    public string NationalIdOrPassport { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string? Email { get; set; }
    public string? Notes { get; set; }
}

public class CreateClientDto
{
    public string FullName { get; set; } = default!;
    public string NationalIdOrPassport { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string? Email { get; set; }
    public string? Notes { get; set; }
}

public class UpdateClientDto
{
    public string FullName { get; set; } = default!;
    public string NationalIdOrPassport { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string? Email { get; set; }
    public string? Notes { get; set; }
}
