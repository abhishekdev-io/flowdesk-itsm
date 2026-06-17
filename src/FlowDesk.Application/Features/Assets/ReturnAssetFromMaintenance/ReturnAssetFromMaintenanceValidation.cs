namespace FlowDesk.Application.Features.Assets.ReturnAssetFromMaintenance;

public class ReturnAssetFromMaintenanceValidation : AbstractValidator<ReturnAssetFromMaintenanceCommand>
{
    public ReturnAssetFromMaintenanceValidation()
    {
        RuleFor(x => x.AssetId)
            .NotEmpty().WithMessage("Asset ID is required.");

        RuleFor(x => x.WorkspaceId)
            .NotEmpty().WithMessage("Workspace ID is required.");
    }
}
