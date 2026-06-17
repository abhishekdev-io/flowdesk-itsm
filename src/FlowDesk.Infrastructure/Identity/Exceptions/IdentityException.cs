namespace FlowDesk.Infrastructure.Identity.Exceptions;

public sealed class IdentityException : Exception
{
    public IEnumerable<string> Errors { get; }
    public IdentityException(IEnumerable<string> errors)
        : base("Identity operation failed.")
    {
        Errors = errors;
    }
}
