using System.Text.RegularExpressions;

namespace NextERP.Core.ValueObjects
{
    public class PhoneNumber
    {
        public string Value { get; private set; }

        public PhoneNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidDataException("Phone number cannot be empty");

            if (!IsValidPhoneNumber(value))
                throw new InvalidDataException("Invalid phone number format");

            Value = value;
        }

        private static bool IsValidPhoneNumber(string phone)
        {
            var regex = new Regex(@"^\+?[1-9]\d{1,14}$");
            return regex.IsMatch(phone);
        }

        public override string ToString() => Value;

        public static implicit operator string(PhoneNumber phone) => phone.Value;
        public static implicit operator PhoneNumber(string value) => new PhoneNumber(value);
    }
}
