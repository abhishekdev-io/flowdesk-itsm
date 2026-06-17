using FluentValidation;

namespace FlowDesk.Application.Features.AssetCategories.RestoreAssetCategory;

internal sealed class RestoreAssetCategoryCommandValidator : AbstractValidator<RestoreAssetCategoryCommand>
{
    public RestoreAssetCategoryCommandValidator()
    {
        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("Category ID must be valid.");

        RuleFor(x => x.WorkspaceId)
            .NotEmpty().WithMessage("Workspace ID is required.");
    }
}
