namespace NextERP.Core.ValueObjects
{
        public class Address
        {
            public string Street { get; set; } = string.Empty;
            public string? Street2 { get; set; }
            public string City { get; set; } = string.Empty;
            public string? State { get; set; }
            public string? ZipCode { get; set; }
            public string Country { get; set; } = string.Empty;

            public override string ToString()
            {
                var parts = new[] { Street, Street2, City, State, ZipCode, Country };
                return string.Join(", ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
            }
        }
}
