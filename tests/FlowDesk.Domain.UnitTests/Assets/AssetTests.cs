using FlowDesk.Domain.Assets;
using FlowDesk.Domain.Exceptions;
using FluentAssertions;

namespace FlowDesk.Domain.UnitTests.Assets;
 
public class AssetTests
{ 
    private const string ValidName = "MacBook Pro M3";
    private static readonly Guid ValidWorkspaceId = Guid.NewGuid();
    private const int ValidCategoryId = 1;
    private const string ValidModelNumber = "A2941";
    private const string ValidAssetTag = "AST-2026-001";
    private const string ValidManufacturer = "Apple";
    private const string? ValidSerialNumber = "C02X8123G91";

    #region Create Method Tests

    [Fact]
    public void Create_Should_ReturnActiveAsset_When_InputsAreValid()
    {
        // Act
        var asset = Asset.Create(
            ValidName, 
            ValidWorkspaceId, 
            ValidCategoryId, 
            ValidModelNumber, 
            ValidAssetTag, 
            ValidSerialNumber, 
            ValidManufacturer);
        
        //Assert
        asset.Should().NotBeNull();
        asset.Id.Should().NotBeEmpty();
        asset.Status.Should().Be(AssetStatus.Active);
        asset.NormalizedName.Should().Be(ValidName.Trim().ToUpperInvariant());
        asset.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory] 
    // Test Tag
    [InlineData("", ValidManufacturer, ValidModelNumber, "Asset tag is required.")]
    [InlineData(" ", ValidManufacturer, ValidModelNumber, "Asset tag is required.")]
    [InlineData(null, ValidManufacturer, ValidModelNumber, "Asset tag is required.")]
    // Test Manufacturer
    [InlineData(ValidAssetTag, "", ValidModelNumber, "Manufacturer is required.")]
    [InlineData(ValidAssetTag, " ", ValidModelNumber, "Manufacturer is required.")]
    [InlineData(ValidAssetTag, null, ValidModelNumber, "Manufacturer is required.")]
    // Test Model Number
    [InlineData(ValidAssetTag, ValidManufacturer, "", "Modelnumber is required")]
    [InlineData(ValidAssetTag, ValidManufacturer, " ", "Modelnumber is required")]
    [InlineData(ValidAssetTag, ValidManufacturer, null, "Modelnumber is required")]
    public void Create_Should_ThrowDomainException_When_RequiredStringInputsAreInvalid(
        string? tag, 
        string? manufacturer, 
        string? modelNumber, 
        string expectedMessage)
    {
        // Act
        Action act = () => Asset.Create(
            ValidName, 
            ValidWorkspaceId, 
            ValidCategoryId, 
            modelNumber!, 
            tag!, 
            ValidSerialNumber, 
            manufacturer!);

        // Assert
        act.Should().Throw<DomainException>().WithMessage(expectedMessage);
    }
    
    [Fact]
    public void Create_Should_ThrowDomainException_When_WorkspaceIdIsEmpty()
    {
        // Act
        Action act = () => Asset.Create(ValidName, Guid.Empty, ValidCategoryId, ValidModelNumber, ValidAssetTag, ValidSerialNumber, ValidManufacturer);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Invalid workspace.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_Should_ThrowDomainException_When_CategoryIdIsInvalid(int invalidCategoryId)
    {
        // Act
        Action act = () => Asset.Create(ValidName, ValidWorkspaceId, invalidCategoryId, ValidModelNumber, ValidAssetTag, ValidSerialNumber, ValidManufacturer);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Invalid category.");
    }

    #endregion

    #region Update Method Tests

    [Fact]
    public void Update_Should_ModifyProperties_When_InputsAreValid()
    {
        // Arrange
        var asset = CreateValidAsset();

        // Act
        asset.Update("ThinkPad P1", 2, "20YQ", "Lenovo", "S-99999");

        // Assert
        asset.CategoryId.Should().Be(2);
        asset.ModelNumber.Should().Be("20YQ");
        asset.Manufacturer.Should().Be("Lenovo");
        asset.SerialNumber.Should().Be("S-99999");
        asset.NormalizedName.Should().Be("THINKPAD P1");
        asset.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Update_Should_ThrowDomainException_When_ModelNumberIsEmpty()
    {
        // Arrange
        var asset = CreateValidAsset();

        // Act
        Action act = () => asset.Update(ValidName, ValidCategoryId, "", ValidManufacturer, ValidSerialNumber);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Modelnumber is required");
    }

    #endregion

    #region Lifecycle State Machine Tests (Maintenance & Decommissioning)

    [Fact]
    public void SendForMaintenance_Should_TransitionToUnderMaintenance_When_AssetIsActive()
    {
        // Arrange
        var asset = CreateValidAsset(); // Starts as Active

        // Act
        asset.SendForMaintenance();

        // Assert
        asset.Status.Should().Be(AssetStatus.UnderMaintenance);
    }

    [Fact]
    public void SendForMaintenance_Should_ThrowDomainStateException_When_AssetIsNotActive()
    {
        // Arrange
        var asset = CreateValidAsset();
        asset.SendForMaintenance(); // Transitions state to UnderMaintenance

        // Act
        Action act = () => asset.SendForMaintenance(); // Attempting it again on non-active asset

        // Assert
        act.Should().Throw<DomainStateException>().WithMessage("Only active assets can be sent for maintenance");
    }

    [Fact]
    public void ReturnFromMaintenance_Should_TransitionToActive_When_AssetIsUnderMaintenance()
    {
        // Arrange
        var asset = CreateValidAsset();
        asset.SendForMaintenance();

        // Act
        asset.ReturnFromMaintenance();

        // Assert
        asset.Status.Should().Be(AssetStatus.Active);
    }

    [Fact]
    public void Decommission_Should_SetStatusToDecommissioned_When_AssetIsActive()
    {
        // Arrange
        var asset = CreateValidAsset();

        // Act
        asset.Decommission();

        // Assert
        asset.Status.Should().Be(AssetStatus.Decommissioned);
        asset.DecommissionedAt.Should().NotBeNull();
    }

    [Fact]
    public void Restore_Should_SetStatusToUnderMaintenance_When_AssetIsDecommissioned()
    {
        // Arrange
        var asset = CreateValidAsset();
        asset.Decommission();

        // Act
        asset.Restore();

        // Assert
        asset.Status.Should().Be(AssetStatus.UnderMaintenance);
        asset.DecommissionedAt.Should().BeNull();
        asset.RestoredAt.Should().NotBeNull();
    }

    [Fact]
    public void Restore_Should_ThrowDomainStateException_When_AssetIsNotDecommissioned()
    {
        // Arrange
        var asset = CreateValidAsset(); // Status is Active

        // Act
        Action act = () => asset.Restore();

        // Assert
        act.Should().Throw<DomainStateException>().WithMessage("Only decommissioned assets can be restored");
    }

    #endregion

    #region Test Helper Methods

    private static Asset CreateValidAsset()
    {
        return Asset.Create(
            ValidName,
            ValidWorkspaceId,
            ValidCategoryId,
            ValidModelNumber,
            ValidAssetTag,
            ValidSerialNumber,
            ValidManufacturer);
    }

    #endregion
}
