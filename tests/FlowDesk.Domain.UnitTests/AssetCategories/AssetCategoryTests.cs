using FlowDesk.Domain.AssetCategories;
using FlowDesk.Domain.Exceptions;
using FluentAssertions;

namespace FlowDesk.Domain.UnitTests.AssetCategories;

public class AssetCategoryTests
{
    private static readonly Guid ValidWorkspaceId = Guid.NewGuid();

    [Theory]
    [InlineData("Laptops", "00000000-0000-0000-0000-000000000000", "LAP", "Invalid workspace.")]
    [InlineData("Laptops", "a6b7c8d9-e0f1-2345-6789-abcdef012345", "", "Prefix is required.")]
    [InlineData("Laptops", "a6b7c8d9-e0f1-2345-6789-abcdef012345", "   ", "Prefix is required.")]
    [InlineData("Laptops", "a6b7c8d9-e0f1-2345-6789-abcdef012345", null, "Prefix is required.")]
    public void Create_WithInvalidParameters_ShouldThrowDomainException(
        string name, string workspaceIdString, string prefix, string expectedErrorMessage)
    {
        // Arrange
        var workspaceId = Guid.Parse(workspaceIdString);

        // Act
        Action act = () => AssetCategory.Create(name, workspaceId, prefix);

        // Assert
        act.Should().Throw<DomainException>().WithMessage(expectedErrorMessage);
    }

    [Fact]
    public void Create_WithValidParameters_ShouldPopulateAndNormalizeProperties()
    {
        // Act
        var category = AssetCategory.Create("  Monitors  ", ValidWorkspaceId, "  mon  ");

        // Assert
        category.Should().NotBeNull();
        category.Name.Value.Should().Be("Monitors"); 
        category.NormalizedName.Should().Be("MONITORS");
        category.Prefix.Should().Be("MON");
        category.WorkspaceId.Should().Be(ValidWorkspaceId);
        category.IsActive.Should().BeTrue();
        category.NextAssetNumber.Should().Be(1);
    }
    
    [Theory]
    [InlineData("Desktops", "DK", true, null)]
    [InlineData("Desktops", "", false, "Prefix is required.")]
    [InlineData("Desktops", "   ", false, "Prefix is required.")]
    public void Update_ShouldBehaveAsExpected(
        string newName, string newPrefix, bool shouldSucceed, string? expectedError)
    {
        // Arrange
        var category = AssetCategory.Create("Laptops", ValidWorkspaceId, "LP");

        // Act
        Action act = () => category.Update(newName, newPrefix);

        // Assert
        if (shouldSucceed)
        {
            act.Should().NotThrow();
            category.NormalizedName.Should().Be(newName.ToUpperInvariant());
            category.Prefix.Should().Be(newPrefix.ToUpperInvariant());
        }
        else
        {
            act.Should().Throw<DomainException>().WithMessage(expectedError);
        }
    }

    [Fact]
    public void GenerateAssetTag_ShouldIncrementSequenceContinuously()
    {
        // Arrange
        var category = AssetCategory.Create("Laptops", ValidWorkspaceId, "LP");

        // Act & Assert (Tests both the padding formula AND the state mutation in one go)
        category.GenerateAssetTag().Should().Be("LP-0001");
        category.GenerateAssetTag().Should().Be("LP-0002");
        category.NextAssetNumber.Should().Be(3);
    }

    [Fact]
    public void Lifecycle_StateTransitions_ShouldMutateTimestampsCorrectly()
    {
        // Step 1: Arrange/Create
        var category = AssetCategory.Create("Laptops", ValidWorkspaceId, "LP");

        // Step 2: Act & Assert Deactivation
        category.Deactivate();
        category.IsActive.Should().BeFalse();
        var deactTime = category.DeactivatedAt.Should().NotBeNull().And.Subject;

        // Idempotency check: Deactivating again changes nothing
        category.Deactivate();
        category.DeactivatedAt.Should().Be(deactTime);

        // Step 3: Act & Assert Restoration
        category.Restore();
        category.IsActive.Should().BeTrue();
        category.DeactivatedAt.Should().BeNull();
        category.RestoredAt.Should().NotBeNull();
    }
}


