namespace FlowDesk.Application.Features.Assets.RestoreAsset;

internal sealed class RestoreAssetCommandValidation : AbstractValidator<RestoreAssetCommand>
{
    public RestoreAssetCommandValidation()
    {
        RuleFor(x => x.AssetId)
            .NotEmpty().WithMessage("Asset ID is required.");

        RuleFor(x => x.WorkspaceId)
            .NotEmpty().WithMessage("Workspace ID is required.");
    }
}
