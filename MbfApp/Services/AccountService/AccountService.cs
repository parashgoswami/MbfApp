using MbfApp.Data;
using MbfApp.Data.Entities;
using MbfApp.Dtos.AccountCodes;
using Microsoft.EntityFrameworkCore;

namespace MbfApp.Services.AccountCodeService;

public interface IAccountService
{
    public Task<AccountResponse?> GetAccountAsync(int id);  
    public Task CreateNewAccountCode(AccountRequestDto request);
    public Task DeleteAccount(int id);
    public Task UpdateAccount(int id, AccountRequestDto request);
    public Task<List<AccountResponse>> ListAccounts();

}

public class AccountService : IAccountService
{
    private AppDbContext _context;

    public AccountService(AppDbContext context)
    {
        _context = context;
    }
    public async Task CreateNewAccountCode(AccountRequestDto request)
    {
        var name = request.Name.Trim();
       
        var exists = await _context.Set<Account>()
            .AnyAsync(a => a.AccountLabel.ToLower() == name.ToLower());

        if (exists)
            throw new InvalidOperationException("Account name must be unique.");

        var entity = new Account
        {
            AccountType = request.AccountType,
            AccountLabel = name
        };
        _context.Set<Account>().Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAccount(int id)
    {
        var accountCode = await _context.Accounts.FindAsync(id);
        if (accountCode == null)
        {
            throw new InvalidOperationException("Account code not found.");
        }

        _context.Accounts.Remove(accountCode);
        await _context.SaveChangesAsync();
    }

    public async Task<AccountResponse?> GetAccountAsync(int id)
    {
        var accountCode = await _context.Accounts
            .Where(a => a.Id == id)
            .Select(a => new AccountResponse
            {
                Id = a.Id,
                AccountType = a.AccountType,
                Name = a.AccountLabel
            }).FirstOrDefaultAsync();
        return accountCode;
    }

    public async Task<List<AccountResponse>> ListAccounts()
    {
        var accountCodes = await _context.Accounts
            .Select(a => new AccountResponse
            {
                Id = a.Id,
                AccountType = a.AccountType,
                Name = a.AccountLabel
            }).ToListAsync();

        return accountCodes;
    }

    public Task UpdateAccount(int id, AccountRequestDto request)
    {
        var accountCode = _context.Accounts.FirstOrDefault(a => a.Id == id);

        if (accountCode == null)
            throw new InvalidOperationException("Account not found.");

        var name = request.Name.Trim();
        if (accountCode.AccountLabel.ToLower() != name.ToLower())
        {
            var exists = _context.Set<Account>()
                .Any(a => a.AccountLabel.ToLower() == name.ToLower());
            if (exists)
                throw new InvalidOperationException("Account name must be unique.");
        }

        accountCode.AccountLabel = name;
        accountCode.AccountType = request.AccountType;
        _context.Update(accountCode);

        return _context.SaveChangesAsync();
    }
}
