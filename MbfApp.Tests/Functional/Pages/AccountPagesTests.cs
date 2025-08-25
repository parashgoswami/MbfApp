using MbfApp.Data;
using MbfApp.Data.Entities;
using MbfApp.Data.Enums;
using MbfApp.Tests.Functional.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;

namespace MbfApp.Tests.Functional.Pages;

public class AccountPagesTests : BlazorPageTest
{
    [Fact]
    public async Task Account_Create_Works()
    {
        await Page.GotoBlazorServerPageAsync("accounts/create");

        var accountName = "Test Account";

        await Page.GetByLabel("Account Name").FillAsync(accountName);
        await Page.GetByLabel("Account Type").SelectOptionAsync(new[] { "Asset" });
        await Page.GetByRole(AriaRole.Button, new() { Name = "Create" }).ClickAsync();

        // Assert
        var newAccount = Page.GetByRole(AriaRole.Cell, new() { Name = accountName });
        await Assertions.Expect(newAccount).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Account_Edit_Works()
    {
        // Arrange
        var initialAccountName = "Account to Edit";
        var updatedAccountName = "Updated Account";

        // Seed an account to edit
        await using var scope = Host.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var account = new Account { AccountLabel = initialAccountName, AccountType = AccountType.Asset };
        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        // Act
        await Page.GotoBlazorServerPageAsync($"accounts/edit?Id={account.Id}");

        await Page.GetByLabel("Account Name").FillAsync(updatedAccountName);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

        // Assert
        await Assertions.Expect(Page.GetByRole(AriaRole.Cell, new() { Name = updatedAccountName })).ToBeVisibleAsync();
        await Assertions.Expect(Page.GetByText(initialAccountName)).Not.ToBeVisibleAsync();
    }

    [Fact]
    public async Task Accounts_Index_Displays_Accounts()
    {
        // Arrange
        var accountsToSeed = new List<string> { "Account A", "Account B", "Account C" };

        await using var scope = Host.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        foreach (var accountLabel in accountsToSeed)
        {
            context.Accounts.Add(new Account { AccountLabel = accountLabel, AccountType = AccountType.Asset });
        }
        await context.SaveChangesAsync();

        // Act
        await Page.GotoBlazorServerPageAsync("/accounts");

        // Assert
        foreach (var accountLabel in accountsToSeed)
        {
            await Assertions.Expect(Page.GetByRole(AriaRole.Cell, new() { Name = accountLabel })).ToBeVisibleAsync();
        }
    }
}
