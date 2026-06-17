using FluentValidation;

namespace FlowDesk.Application.Features.AssetCategories.UpdateAssetCategory;

internal sealed class UpdateAssetCategoryCommandValidator : AbstractValidator<UpdateAssetCategoryCommand>
{
    public UpdateAssetCategoryCommandValidator()
    {
        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("Category ID must be valid.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters.");

        RuleFor(x => x.WorkspaceId)
            .NotEmpty().WithMessage("Workspace ID is required.");

        RuleFor(x => x.Prefix)
            .NotEmpty()
            .MaximumLength(4)
            .Matches("^[A-Z]+$")
            .WithMessage("Prefix must contain only uppercase letters.");     
    }
}
