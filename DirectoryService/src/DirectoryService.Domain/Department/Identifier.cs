
namespace DirectoryService.Domain
{
    public record Identifier
    {
        private const short MAX_IDENTIFIER_LENGTH = 10;
        public string Value { get; }

        private Identifier(string value)
        {
            Value = value;
        }

        public static Identifier Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Identifier cannot be null or empty.", nameof(value));
            }

            if (value.Length > MAX_IDENTIFIER_LENGTH)
            {
                throw new ArgumentException($"Identifier cannot exceed {MAX_IDENTIFIER_LENGTH} characters.", nameof(value));
            }

            if (value.ToUpperInvariant() != value)
            {
                throw new ArgumentException("Identifier must be in uppercase.", nameof(value));
            }

            if (value.Any(c => !char.IsLetterOrDigit(c)))
            {
                throw new ArgumentException("Identifier have uncorrect format.", nameof(value));
            }

            return new Identifier(value);
        }
    }

}
