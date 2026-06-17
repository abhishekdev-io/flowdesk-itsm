using FluentValidation;

namespace FlowDesk.Application.Features.AssetCategories.DeactivateAssetCategory;

internal sealed class DeactivateAssetCategoryCommandValidator : AbstractValidator<DeactivateAssetCategoryCommand>
{
    public DeactivateAssetCategoryCommandValidator()
    {
        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("Category ID must be valid.");

        RuleFor(x => x.WorkspaceId)
            .NotEmpty().WithMessage("Workspace ID is required.");
    }
}
