using NextERP.Common.Constants;

namespace NextERP.Features.Auth.DTOs
{
    /// <summary>
    /// Response DTO for registration operation
    /// Contains JWT tokens and user basic info
    /// </summary>
    public class RegisterResponseDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int AccessTokenExpiresIn { get; set; } // in seconds
        public IEnumerable<string> Roles { get; set; } = new List<string>();
        public IEnumerable<string> Permissions { get; set; } = new List<string>();
    }
}