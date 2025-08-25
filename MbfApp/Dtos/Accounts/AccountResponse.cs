using MbfApp.Data.Enums;

namespace MbfApp.Dtos.AccountCodes;

public record AccountResponse
{
    public int Id { get; init; }
    public string AccountName { get; init; } = string.Empty;
    public AccountType AccountType { get; init; }    
}
