namespace FlowDesk.Application.Features.Assets.ListAssets;

    internal sealed class ListAssetsQueryValidator : AbstractValidator<ListAssetsQuery>
    {
        public ListAssetsQueryValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty()
            .WithMessage("Workspace ID is required.");

        When(x => x.Filters is not null, () =>
        {
            RuleFor(x => x.Filters!)
                .SetValidator(new AssetFilterParamsValidator());
        });
    }
    }
