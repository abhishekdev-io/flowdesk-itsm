namespace FlowDesk.Application.Common.Permission;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class RequirePermissionAttribute(Permissions permission) : Attribute
{
    public Permissions Permission { get; } = permission;
}
