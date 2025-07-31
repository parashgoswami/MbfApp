using MbfApp.Data.Enums;

namespace MbfApp.Dtos.AccountCodes;

public record AccountCodeResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public AccountType AccountType { get; init; }    
}
