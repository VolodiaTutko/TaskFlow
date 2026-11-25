using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using TaskFlow.Infrastructure.Identity;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.IntegrationTests.Persistence;

public sealed class IdentityModelTests
{
    [Fact]
    public void ApplicationUser_ShouldUseGuidPrimaryKey()
    {
        var user = GetUserEntity();

        var key = user.FindPrimaryKey();

        Assert.NotNull(key);
        Assert.Equal(typeof(Guid), Assert.Single(key.Properties).ClrType);
    }

    [Fact]
    public void ApplicationUser_DisplayName_ShouldHaveMaxLength100()
    {
        var user = GetUserEntity();

        var property = user.FindProperty(nameof(ApplicationUser.DisplayName));

        Assert.NotNull(property);
        Assert.Equal(100, property.GetMaxLength());
    }

    [Fact]
    public void ApplicationUser_NormalizedEmail_ShouldHaveSingleUniqueIndex()
    {
        var user = GetUserEntity();

        var emailIndexes = user.GetIndexes()
            .Where(i => i.Properties.Count == 1
                && i.Properties[0].Name == nameof(ApplicationUser.NormalizedEmail))
            .ToList();

        var index = Assert.Single(emailIndexes);
        Assert.True(index.IsUnique);
    }

    private static IEntityType GetUserEntity()
    {
        var options = new DbContextOptionsBuilder<TaskFlowDbContext>()
            .UseNpgsql("Host=localhost;Database=model_test;Username=test")
            .Options;

        using var context = new TaskFlowDbContext(options);

        return context.Model.FindEntityType(typeof(ApplicationUser))!;
    }
}