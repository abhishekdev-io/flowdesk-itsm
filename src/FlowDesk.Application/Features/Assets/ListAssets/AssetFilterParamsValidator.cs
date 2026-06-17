namespace FlowDesk.Application.Features.Assets.ListAssets;

internal sealed class AssetFilterParamsValidator : AbstractValidator<AssetFilterParams>
{
    private static readonly HashSet<string> AllowedSortFields = new(
    [ "name", "status", "createdat", "category", "manufacturer" ],
    StringComparer.OrdinalIgnoreCase);
    public AssetFilterParamsValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);

        When(x => x.SortBy is not null, () =>
        {
            RuleFor(x => x.SortBy!)
                .Must(s => AllowedSortFields.Contains(s))
                .WithMessage($"SortBy must be one of: {string.Join(", ", AllowedSortFields)}.");
        });

        When(x => x.Search is not null, () =>
        {
            RuleFor(x => x.Search!)
                .NotEmpty()
                .WithMessage("Search must not be empty or whitespace.")
                .MaximumLength(200);
        });

        When(x => x.CategoryId is not null, () =>
        {
            RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("Category ID must be valid.");
        });

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue);
    }
}
