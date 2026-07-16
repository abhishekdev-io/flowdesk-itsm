namespace FlowDesk.Domain.Authentication;

public class RefreshToken : Entity<Guid>
{
    public string TokenHash { get; private set; } = default!;
    public Guid UserId { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public Guid? ReplacedByTokenId { get; private set; }
    public string? RevokedReason { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt is not null;
    public bool IsActive => !IsExpired && !IsRevoked;
    

    private RefreshToken() : base(Guid.Empty) { }

    private RefreshToken(Guid id, string tokenHash, Guid userId, DateTime expiresAt) : base(id)
    {
        Id = id;
        TokenHash = tokenHash;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
    }

    public static RefreshToken Create(string tokenHash, Guid userId, DateTime expiresAt)
    {
        return new RefreshToken(Guid.NewGuid(), tokenHash, userId, expiresAt);
    }

    public void Revoke(string reason, Guid? replacedByTokenId = null)
    {
        if (IsRevoked) return; 
        
        RevokedAt = DateTime.UtcNow;
        RevokedReason = reason;
        ReplacedByTokenId = replacedByTokenId;
    }
}
 