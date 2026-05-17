namespace NextERP.Common.Exceptions
{
    public class ApiException : Exception
    {
        public int StatusCode { get; }
        public List<string>? Errors { get; }

        public ApiException(string message, int statusCode = 400, List<string>? errors = null) 
            : base(message)
        {
            StatusCode = statusCode;
            Errors = errors;
        }
    }

    public class NotFoundException : ApiException
    {
        public NotFoundException(string message) : base(message, 404) { }
        public NotFoundException(string name, object key) 
            : base($"Entity \"{name}\" ({key}) was not found.", 404) { }
    }

    public class ValidationException : ApiException
    {
        public ValidationException(List<string> errors) 
            : base("One or more validation failures have occurred.", 400, errors) { }
    }

    public class ForbiddenException : ApiException
    {
        public ForbiddenException(string message = "You do not have permission to access this resource.") 
            : base(message, 403) { }
    }
}
