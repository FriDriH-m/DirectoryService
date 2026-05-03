namespace DirectoryService.Domain
{
    public class Timezone
    {
        private const short MAX_TIMEZONE_LENGTH = 64;

        public string Value { get; }

        private Timezone(string value) => Value = value;

        public static Timezone Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Timezone cannot be null or empty.", nameof(value));

            if (value.Length > MAX_TIMEZONE_LENGTH)
                throw new ArgumentException($"Timezone cannot exceed {MAX_TIMEZONE_LENGTH} characters.", nameof(value));

            var trimmed = value.Trim();

            try
            {
                TimeZoneInfo.FindSystemTimeZoneById(trimmed);
            }
            catch (Exception ex) when (ex is TimeZoneNotFoundException or InvalidTimeZoneException)
            {
                throw new ArgumentException("Invalid timezone format", nameof(value));
            }

            return new Timezone(trimmed);
        }
    }
}
