namespace NextERP.Core.Entities
{
    public class RefreshTokenModel
    {
        public int RefreshTokeId { get; set; }
        public string Token { get; set; } = string.Empty;
        public int UserId { get; set; }
        public virtual UserModel? User { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedByIp { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string? RevokedByIp { get; set; }
        public string? ReplacedByToken { get; set; }
    }
}
