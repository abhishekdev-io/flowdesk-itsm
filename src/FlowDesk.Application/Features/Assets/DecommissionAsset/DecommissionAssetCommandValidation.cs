namespace FlowDesk.Application.Features.Assets.DecommissionAsset;

public class DecommissionAssetCommandValidation : AbstractValidator<DecommissionAssetCommand>
{
    public DecommissionAssetCommandValidation()
    {
        RuleFor(x => x.AssetId)
            .NotEmpty().WithMessage("Asset ID is required.");

        RuleFor(x => x.WorkspaceId)
            .NotEmpty().WithMessage("Workspace ID is required.");
    }
}
