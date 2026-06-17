using FluentValidation;

namespace FlowDesk.Application.Features.AssetCategories.CreateAssetCategory;

internal sealed class CreateAssetCategoryCommandValidator : AbstractValidator<CreateAssetCategoryCommand>
{
    public CreateAssetCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters.");

        RuleFor(x => x.WorkspaceId)
            .NotEmpty().WithMessage("Workspace ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("Invalid Workspace ID.");  

        RuleFor(x => x.Prefix)
            .NotEmpty()
            .MaximumLength(4)
            .Matches("^[A-Z]+$")
            .WithMessage("Prefix must contain only uppercase letters.");      
    }

    
}
