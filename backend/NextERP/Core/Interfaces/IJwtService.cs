namespace NextERP.Core.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(int userId, string email, IEnumerable<string> roles);
        string GenerateRefreshToken();
        bool ValidateToken(string token, out int userId);
    }
}
