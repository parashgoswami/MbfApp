using MbfApp.Data;
using MbfApp.Data.Entities;
using MbfApp.Data.Enums;
using MbfApp.Dtos.AccountCodes;
using Microsoft.EntityFrameworkCore;

namespace MbfApp.Services.AccountCodeService;

public interface IAccountCodeService
{
    public Task<AccountCodeResponse?> GetAccountCodeAsync(int id);  
    public Task CreateNewAccountCode(AccountCodeRequestDto request);
    public Task DeleteAccountCode(int id);
    public Task UpdateAccountCode(int id, AccountCodeRequestDto request);
    public Task<List<AccountCodeResponse>> ListAccountCodes();

}

public class AccountCodeService : IAccountCodeService
{
    private AppDbContext _context;

    public AccountCodeService(AppDbContext context)
    {
        _context = context;
    }
    public async Task CreateNewAccountCode(AccountCodeRequestDto request)
    {
        var name = request.Name.Trim();
       
        var exists = await _context.Set<AccountCode>()
            .AnyAsync(a => a.Name.ToLower() == name.ToLower());

        if (exists)
            throw new InvalidOperationException("Account name must be unique.");

        var entity = new AccountCode
        {
            AccountType = request.AccountType,
            Name = name
        };
        _context.Set<AccountCode>().Add(entity);
        await _context.SaveChangesAsync();
    }

    public Task DeleteAccountCode(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<AccountCodeResponse?> GetAccountCodeAsync(int id)
    {
        var accountCode = await _context.AccountCodes
            .Where(a => a.Id == id)
            .Select(a => new AccountCodeResponse
            {
                Id = a.Id,
                AccountType = a.AccountType,
                Name = a.Name
            }).FirstOrDefaultAsync();
        return accountCode;
    }

    public async Task<List<AccountCodeResponse>> ListAccountCodes()
    {
          var accountCodes = await _context.AccountCodes
            .Select(a => new AccountCodeResponse
            {
                Id = a.Id,
                AccountType = a.AccountType,
                Name = a.Name
            }).ToListAsync();
            return accountCodes;
    }

    public Task UpdateAccountCode(int id, AccountCodeRequestDto request)
    {
       var accountCode = _context.AccountCodes.FirstOrDefault(a => a.Id == id);
        if (accountCode == null)
            throw new InvalidOperationException("Account code not found.");
        var name = request.Name.Trim();
        if (accountCode.Name.ToLower() != name.ToLower())
        {
            var exists = _context.Set<AccountCode>()
                .Any(a => a.Name.ToLower() == name.ToLower());
            if (exists)
                throw new InvalidOperationException("Account name must be unique.");
        }
        accountCode.Name = name;
        accountCode.AccountType = request.AccountType;
        _context.Update(accountCode);
        return _context.SaveChangesAsync();
    }
}
