namespace NextERP.Common.DTOs
{
    public class ErrorResponse
    {
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int Status { get; set; }
        public string Detail { get; set; } = string.Empty;
        public string Instance { get; set; } = string.Empty;
        public Dictionary<string, List<string>>? Errors { get; set; }

        public static ErrorResponse Create(int statusCode, string message, string path)
        {
            return new ErrorResponse
            {
                Status = statusCode,
                Title = GetTitle(statusCode),
                Detail = message,
                Instance = path,
                Type = "https://httpstatuses.com/" + statusCode
            };
        }

        private static string GetTitle(int statusCode)
        {
            return statusCode switch
            {
                400 => "Bad Request",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "Not Found",
                409 => "Conflict",
                422 => "Unprocessable Entity",
                500 => "Internal Server Error",
                _ => "Error"
            };
        }
    }
}
