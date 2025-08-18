using MbfApp.Data;
using MbfApp.Data.Entities;
using MbfApp.Dtos.Member;
using MbfApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace MbfApp.Tests.Services;

public class MemberServiceTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private Mock<UserManager<ApplicationUser>> GetMockUserManager()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        
        return new Mock<UserManager<ApplicationUser>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!
        );
    }

    [Fact]
    public async Task GetPaginatedListAync_ShouldReturnPaginatedList()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var mockUserManager = GetMockUserManager();

        var location = new Location { Id = 1, Name = "Test Location" };
        context.Locations.Add(location);
        context.Members.AddRange(
            new Member { Id = 1, EmployeeNo = "101", FirstName = "John", LastName = "Doe", Location = location },
            new Member { Id = 2, EmployeeNo = "102", FirstName = "Jane", LastName = "Doe", Location = location },
            new Member { Id = 3, EmployeeNo = "103", FirstName = "Peter", LastName = "Jones", Location = location }
        );
        await context.SaveChangesAsync();

        var service = new MemberService(mockUserManager.Object, context);
        var searchParams = new MemberSearchParameters { PageIndex = 1, PageSize = 2 };

        // Act
        var result = await service.GetPaginatedListAync(searchParams);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(2, result.TotalPages);
    }

    [Fact]
    public async Task GetPaginatedListAync_WithSearchTerm_ShouldReturnFilteredList()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var mockUserManager = GetMockUserManager();
        
        var location = new Location { Id = 1, Name = "Test Location" };
        context.Locations.Add(location);
        context.Members.AddRange(
            new Member { Id = 1, EmployeeNo = "101", FirstName = "John", LastName = "Doe", Location = location },
            new Member { Id = 2, EmployeeNo = "102", FirstName = "Jane", LastName = "Doe", Location = location },
            new Member { Id = 3, EmployeeNo = "103", FirstName = "Peter", LastName = "Jones", Location = location }
        );
        await context.SaveChangesAsync();

        var service = new MemberService(mockUserManager.Object, context);
        var searchParams = new MemberSearchParameters { PageIndex = 1, PageSize = 10, SearchTerm = "Doe" };

        // Act
        var result = await service.GetPaginatedListAync(searchParams);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task GetMemberByIdAsync_ShouldReturnMember_WhenMemberExists()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var mockUserManager = GetMockUserManager();

        var location = new Location { Id = 1, Name = "Test Location" };
        context.Locations.Add(location);
        context.Members.Add(new Member { Id = 1, EmployeeNo = "123", FirstName = "Test", LastName = "User", Location = location });
        await context.SaveChangesAsync();

        var service = new MemberService(mockUserManager.Object, context);

        // Act
        var result = await service.GetMemberByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test", result.FirstName);
        Assert.Equal("Test Location", result.LocationName);
    }

    [Fact]
    public async Task GetMemberByIdAsync_ShouldReturnNull_WhenMemberDoesNotExist()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var mockUserManager = GetMockUserManager();

        var service = new MemberService(mockUserManager.Object, context);

        // Act
        var result = await service.GetMemberByIdAsync(99);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateMember_Should_Add_Member_And_User()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var mockUserManager = GetMockUserManager();

        mockUserManager
            .Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        mockUserManager
            .Setup(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Member"))
            .ReturnsAsync(IdentityResult.Success);

        var service = new MemberService(mockUserManager.Object, context);

        var dto = GetMemberRequestDto();

        // Act
        await service.CreateMember(dto);

        // Assert
        var member = await context.Members.FirstOrDefaultAsync();
        Assert.NotNull(member);
        Assert.Equal("6001", member.EmployeeNo);

        mockUserManager.Verify(u => u.CreateAsync(It.IsAny<ApplicationUser>(), "Welcome@123"), Times.Once);
        mockUserManager.Verify(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Member"), Times.Once);
    }

    [Fact]
    public async Task CreateMember_Should_Throw_When_EmployeeNo_Not_Unique()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.Members.Add(new Member { EmployeeNo = "6001", FirstName = "Existing" });
        await context.SaveChangesAsync();

        var mockUserManager = GetMockUserManager();
        var service = new MemberService(mockUserManager.Object, context);

        var dto = GetMemberRequestDto();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateMember(dto));
        Assert.Equal("Employee number must be unique.", exception.Message);
    }

    [Fact]
    public async Task CreateMember_Should_Rollback_When_User_Creation_Fails()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var mockUserManager = GetMockUserManager();

        mockUserManager
            .Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());

        var service = new MemberService(mockUserManager.Object, context);

        var dto = GetMemberRequestDto();

        // Act
        await service.CreateMember(dto);

        // Assert
        var member = await context.Members.FirstOrDefaultAsync(m => m.EmployeeNo == "E999");
        Assert.Null(member); // Should be rolled back
    }

    [Fact]
    public async Task UpdateMember_ShouldUpdateMember_WhenDataIsValid()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var mockUserManager = GetMockUserManager();

        context.Members.Add(new Member { Id = 1, EmployeeNo = "123", FirstName = "First Old", LastName = "Last Old" });
        await context.SaveChangesAsync();

        var service = new MemberService(mockUserManager.Object, context);
        var request = new MemberRequestDto { EmployeeNo = "124", FirstName = "New", LastName = "New" };

        // Act
        await service.UpdateMember(1, request);

        // Assert
        var member = await context.Members.FindAsync(1);
        Assert.NotNull(member);
        Assert.Equal("New", member.FirstName);        
        Assert.Equal("New", member.LastName);
        Assert.Equal("124", member.EmployeeNo);
    }

    [Fact]
    public async Task UpdateMember_ShouldThrowInvalidOperationException_WhenMemberNotFound()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var mockUserManager = GetMockUserManager();

        var service = new MemberService(mockUserManager.Object, context);
        var request = new MemberRequestDto { EmployeeNo = "123" };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.UpdateMember(99, request));
        Assert.Equal("Member not found.", exception.Message);
    }

    [Fact]
    public async Task UpdateMember_ShouldThrowInvalidOperationException_WhenEmployeeNoExists()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var mockUserManager = GetMockUserManager();

        context.Members.Add(new Member { Id = 1, EmployeeNo = "123", FirstName = "Test", LastName = "User" });
        context.Members.Add(new Member { Id = 2, EmployeeNo = "456", FirstName = "Another", LastName = "User" });
        await context.SaveChangesAsync();

        var request = new MemberRequestDto { EmployeeNo = "456" };
        var service = new MemberService(mockUserManager.Object, context);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.UpdateMember(1, request));
        Assert.Equal("Employee number must be unique.", exception.Message);
    }

    [Fact]
    public async Task DeleteMember_ShouldRemoveMember_WhenMemberExists()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var mockUserManager = GetMockUserManager();

        var member = new Member { Id = 1, EmployeeNo = "123", FirstName = "Test", LastName = "User", Email = "test@test.com" };
        context.Members.Add(member);
        await context.SaveChangesAsync();

        mockUserManager
            .Setup(x => x.FindByEmailAsync(member.Email))
            .ReturnsAsync(new ApplicationUser());
        mockUserManager
            .Setup(x => x.DeleteAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);

        var service = new MemberService(mockUserManager.Object, context);

        // Act
        await service.DeleteMember(1);

        // Assert
        var exists = await context.Members.AnyAsync();
        Assert.False(exists);
    }

    private MemberRequestDto GetMemberRequestDto()
    {
        return new MemberRequestDto
        {
            EmployeeNo = "6001",
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Phone = "1234567890",
            Share = 100,
            Nominee = "Jane Doe",
            JoiningDate = DateTime.UtcNow,
            LocationId = 1
        };
    }
}
