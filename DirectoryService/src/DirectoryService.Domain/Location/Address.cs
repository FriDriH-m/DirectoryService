using System.ComponentModel.DataAnnotations;

namespace DirectoryService.Domain
{
    public class Address
    {
        private const int MAX_ADDRESS_LENGTH = 300; // Более разумная длина для отдельных полей

        public string City { get; }

        public string Street { get; }

        public string House { get; }

        public string? Appartment { get; }

        private Address(
            string street,
            string house,
            string city,
            string appartment)
        {
            Street = street;
            House = house;
            City = city;
        }

        public static Address Create(
            string street,
            string house,
            string city,
            string appartment)
        {
            ValidateStringArgument(street, nameof(street));
            ValidateStringArgument(house, nameof(house));
            ValidateStringArgument(city, nameof(city));

            var totalLength = (street?.Length ?? 0) + (house?.Length ?? 0) +
                              (city?.Length ?? 0) + (appartment?.Length ?? 0);
            if (totalLength > MAX_ADDRESS_LENGTH)
            {
                throw new ArgumentException($"Address cannot exceed {MAX_ADDRESS_LENGTH} characters.");
            }

            return new Address(street.Trim(), house.Trim(), city.Trim(), appartment?.Trim() ?? string.Empty);
        }

        public Address Update(
            string? street = null,
            string? house = null,
            string? city = null,
            string? appartment = null)
        {
            return Create(
                street ?? Street,
                house ?? House,
                city ?? City,
                appartment ?? string.Empty
            );
        }

        public override string ToString() => $"{Street}, {House}, {City}";

        private static void ValidateStringArgument(string? value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{paramName} cannot be null or empty.", paramName);
            }
        }
    }
}