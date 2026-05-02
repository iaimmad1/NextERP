namespace NextERP.Core.ValueObjects
{
    public class Email
    {
        public string Value { get; private set; }

        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidDataException("Email cannot be empty");

            if (!IsValidEmail(value))
                throw new InvalidDataException("Invalid email format");

            Value = value.ToLowerInvariant();
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public override string ToString() => Value;

        public static implicit operator string(Email email) => email.Value;
        public static implicit operator Email(string value) => new Email(value);
    }
}
