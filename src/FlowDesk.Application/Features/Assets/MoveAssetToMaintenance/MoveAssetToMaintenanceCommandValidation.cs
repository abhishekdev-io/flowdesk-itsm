namespace FlowDesk.Application.Features.Assets.MoveAssetToMaintenance;

internal sealed class MoveAssetToMaintenanceCommandValidation : AbstractValidator<MoveAssetToMaintenanceCommand>
{
    public MoveAssetToMaintenanceCommandValidation()
    {
        RuleFor(x => x.AssetId)
            .NotEmpty().WithMessage("Asset ID is required.");

        RuleFor(x => x.WorkspaceId)
            .NotEmpty().WithMessage("Workspace ID is required.");
    }
}
