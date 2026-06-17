namespace FlowDesk.Application.Features.Assets.GetAsset;

internal sealed class GetAssetByIdQueryValidation : AbstractValidator<GetAssetByIdQuery>
{
    public GetAssetByIdQueryValidation()
    {
        RuleFor(x => x.AssetId)
            .NotEmpty()
            .WithMessage("Asset ID is required.");

        RuleFor(x => x.WorkspaceId)
            .NotEmpty()
            .WithMessage("Workspace ID is required.");
    }
}
