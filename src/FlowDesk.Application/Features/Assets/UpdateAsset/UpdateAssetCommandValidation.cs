namespace FlowDesk.Application.Features.Assets.UpdateAsset;

internal sealed class UpdateAssetCommandValidation : AbstractValidator<UpdateAssetCommand>
{
    public UpdateAssetCommandValidation()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Asset name is required.")
            .MaximumLength(100).WithMessage("Asset name cannot exceed 100 characters.");

        RuleFor(x => x.AssetId)
            .NotEmpty()
            .WithMessage("Asset ID is required.");    

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("Category ID must be valid.");

        RuleFor(x => x.WorkspaceId)
            .NotEmpty()
            .WithMessage("Workspace ID is required.");

        RuleFor(x => x.Manufacturer)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Modelnumber)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Serialnumber)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Serialnumber));     

    }
}

