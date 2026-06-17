namespace FlowDesk.Application.Common.Exceptions;

public sealed class PermissionMethodNotFoundException : Exception
{
    public PermissionMethodNotFoundException(string methodName)
        : base($"Authorization method '{methodName}' was not found.")
    {
    }
}
