using FluentValidation;

namespace FlowDesk.Application.Features.AssetCategories.ListAssetCategories;

internal sealed class ListAssetCategoriesQueryValidation : AbstractValidator<ListAssetCategoriesQuery>
{
    public ListAssetCategoriesQueryValidation()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty()
            .WithMessage("Workspace ID is required.");
    }
}
