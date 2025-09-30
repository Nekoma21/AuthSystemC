namespace AuthSystem.Domain.Entities;

public class TwoFactorCode : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime? UsedAt { get; set; }
    public TwoFactorCodeType Type { get; set; }
    
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsValid => !IsUsed && !IsExpired;
}

public enum TwoFactorCodeType
{
    Email,
    SMS,
    Authenticator
}
