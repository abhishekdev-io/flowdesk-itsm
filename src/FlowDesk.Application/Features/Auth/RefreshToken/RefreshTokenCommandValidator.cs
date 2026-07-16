namespace FlowDesk.Application.Features.Auth.RefreshToken;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required.")
            
            .MaximumLength(500)
            .WithMessage("Refresh token is too long.")
            .Must(BeValidBase64)
            .WithMessage("Invalid refresh token format.");
    }

    private bool BeValidBase64(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return false;
        
        // Quick length check for Base64 structure
        if (token.Length % 4 != 0) return false;

        // Verify it only contains valid Base64 characters
        return Convert.TryFromBase64String(token, new Span<byte>(new byte[token.Length]), out _);
    }
}
