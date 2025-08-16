using MbfApp.Dtos.AccountCodes;

namespace MbfApp.Services;

public interface IAccountService
{
    public Task<AccountResponse?> GetAccountAsync(int id);  
    public Task CreateNewAccountCode(AccountRequestDto request);
    public Task DeleteAccount(int id);
    public Task UpdateAccount(int id, AccountRequestDto request);
    public Task<List<AccountResponse>> ListAccounts();
}
