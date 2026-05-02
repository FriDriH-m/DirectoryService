namespace DirectoryService.Domain
{
    public class Address
    {
        private const short MAX_ADDRESS_LENGTH = 500;

        public string Value { get; }

        private Address(string value)
        {
            Value = value;
        }

        public static Address Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Address cannot be null or empty.", nameof(value));
            }

            if (value.Length > MAX_ADDRESS_LENGTH)
            {
                throw new ArgumentException($"Address cannot exceed {MAX_ADDRESS_LENGTH} characters.", nameof(value));
            }

            return new Address(value.Trim());
        }
    }
}
