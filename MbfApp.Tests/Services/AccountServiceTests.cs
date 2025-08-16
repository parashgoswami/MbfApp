using MbfApp.Data;
using MbfApp.Data.Entities;
using MbfApp.Data.Enums;
using MbfApp.Dtos.AccountCodes;
using MbfApp.Services;
using Microsoft.EntityFrameworkCore;

namespace MbfApp.Tests.Services;

public class AccountServiceTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique DB for each test
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task ListAccounts_ShouldReturnAllAccounts()
    {
        // Arrange
        var context = GetInMemoryDbContext();

        context.Accounts.Add(new Account { AccountLabel = "Account 1", AccountType = AccountType.Asset });
        context.Accounts.Add(new Account { AccountLabel = "Account 2", AccountType = AccountType.Liabilities });
        await context.SaveChangesAsync();

        var service = new AccountService(context);

        // Act
        var result = await service.ListAccounts();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAccountAsync_ShouldReturnAccount_WhenAccountExists()
    {
        // Arrange
        var context = GetInMemoryDbContext();

        context.Accounts.Add(new Account { Id = 1, AccountLabel = "Test Account", AccountType = AccountType.Asset });
        await context.SaveChangesAsync();

        var service = new AccountService(context);

        // Act
        var result = await service.GetAccountAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Account", result.Name);
    }

    [Fact]
    public async Task GetAccountAsync_ShouldReturnNull_WhenAccountDoesNotExist()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new AccountService(context);

        // Act
        var result = await service.GetAccountAsync(99);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateNewAccountCode_ShouldAddAccount_WhenNameIsUnique()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var request = new AccountRequestDto { Name = "Test Account", AccountType = AccountType.Asset };

        // Act
        var service = new AccountService(context);
        await service.CreateNewAccountCode(request);

        // Assert
        var account = await context.Accounts.SingleOrDefaultAsync(a => a.AccountLabel == request.Name);
        Assert.NotNull(account);
        Assert.Equal("Test Account", account.AccountLabel);
    }

    [Fact]
    public async Task CreateNewAccountCode_ShouldThrowInvalidOperationException_WhenNameExists()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.Accounts.Add(new Account { AccountLabel = "Existing Account", AccountType = AccountType.Asset });
        await context.SaveChangesAsync();

        var service = new AccountService(context);
        var request = new AccountRequestDto { Name = "Existing Account", AccountType = AccountType.Asset };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateNewAccountCode(request));
        Assert.Equal("Account name must be unique.", exception.Message);
    }   

    [Fact]
    public async Task UpdateAccount_ShouldUpdateAccount_WhenDataIsValid()
    {
        // Arrange
        var context = GetInMemoryDbContext();

        context.Accounts.Add(new Account { Id = 1, AccountLabel = "Old Name", AccountType = AccountType.Asset });
        await context.SaveChangesAsync();

        var request = new AccountRequestDto { Name = "New Name", AccountType = AccountType.Liabilities };
        var service = new AccountService(context);

        // Act
        await service.UpdateAccount(1, request);

        // Assert
        var account = await context.Accounts.FindAsync(1);
        Assert.NotNull(account);
        Assert.Equal("New Name", account.AccountLabel);
        Assert.Equal(AccountType.Liabilities, account.AccountType);
    }

    [Fact]
    public async Task UpdateAccount_ShouldThrowInvalidOperationException_WhenAccountNotFound()
    {
        // Arrange
        var context = GetInMemoryDbContext();

        var service = new AccountService(context);
        var request = new AccountRequestDto { Name = "New Name", AccountType = AccountType.Asset };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.UpdateAccount(99, request));
        Assert.Equal("Account not found.", exception.Message);
    }

    [Fact]
    public async Task UpdateAccount_ShouldThrowInvalidOperationException_WhenNameExists()
    {
        // Arrange
        var context = GetInMemoryDbContext();

        context.Accounts.Add(new Account { Id = 1, AccountLabel = "OLD NAME", AccountType = AccountType.Asset });
        context.Accounts.Add(new Account { Id = 2, AccountLabel = "EXISTING NAME", AccountType = AccountType.Asset });
        await context.SaveChangesAsync();

        var service = new AccountService(context);
        var request = new AccountRequestDto { Name = "Existing Name", AccountType = AccountType.Asset };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.UpdateAccount(1, request));
        Assert.Equal("Account name must be unique.", exception.Message);
    }
    
    [Fact]
    public async Task DeleteAccount_ShouldRemoveAccount_WhenAccountExists()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.Accounts.Add(new Account { Id = 1, AccountLabel = "Test Account", AccountType = AccountType.Asset });
        await context.SaveChangesAsync();

        // Act
        var service = new AccountService(context);
        await service.DeleteAccount(1);

        // Assert
        var exists = await context.Accounts.AnyAsync();
        Assert.False(exists);
    }
}
