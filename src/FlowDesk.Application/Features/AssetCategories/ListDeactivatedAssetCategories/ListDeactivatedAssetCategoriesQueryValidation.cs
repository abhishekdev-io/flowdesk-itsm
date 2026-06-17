using FluentValidation;

namespace FlowDesk.Application.Features.AssetCategories.ListDeactivatedAssetCategories;

internal sealed class ListDeactivatedAssetCategoriesQueryValidation : AbstractValidator<ListDeactivatedAssetCategoriesQuery>
{
    public ListDeactivatedAssetCategoriesQueryValidation()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty()
            .WithMessage("Workspace ID is required.");
    }
}
