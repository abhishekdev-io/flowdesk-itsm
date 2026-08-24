namespace FlowDesk.Application.Features.Assets.AssetStats;

internal sealed class GetAssetStatsQueryValidator : AbstractValidator<GetAssetStatsQuery>
{
    public GetAssetStatsQueryValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty()
            .WithMessage("Workspace ID is required.");
    }
}
