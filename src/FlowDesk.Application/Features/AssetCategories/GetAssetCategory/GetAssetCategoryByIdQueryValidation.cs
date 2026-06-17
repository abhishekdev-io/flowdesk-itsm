using FluentValidation;

namespace FlowDesk.Application.Features.AssetCategories.GetAssetCategory;

internal sealed class GetAssetCategoryByIdQueryValidation : AbstractValidator<GetAssetCategoryByIdQuery>
{
    public GetAssetCategoryByIdQueryValidation()
    {
        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("AssetCategory ID must be valid.");

        RuleFor(x => x.WorkspaceId)
            .NotEmpty()
            .WithMessage("Workspace ID is required.");
    }
}
