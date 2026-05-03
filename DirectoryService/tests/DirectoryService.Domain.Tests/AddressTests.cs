using Xunit;

namespace DirectoryService.Domain.Tests
{
    public class AddressTests
    {
        [Fact]
        public void Create_WithValidAddress_ReturnsAddress()
        {
            // Arrange & Act
            var address = Address.Create("123 Main Street, New York, NY 10001");

            // Assert
            Assert.NotNull(address);
            Assert.Equal("123 Main Street, New York, NY 10001", address.Value);
        }

        [Fact]
        public void Create_WithWhitespace_TrimsBothSides()
        {
            // Arrange & Act
            var address = Address.Create("  123 Main Street  ");

            // Assert
            Assert.Equal("123 Main Street", address.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Create_WithNullOrEmpty_ThrowsArgumentException(string? invalidAddress)
        {
            // Arrange & Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => Address.Create(invalidAddress ?? ""));
            Assert.Contains("Address cannot be null or empty", ex.Message);
        }

        [Fact]
        public void Create_WithAddressExceedingMaxLength_ThrowsArgumentException()
        {
            // Arrange
            var longAddress = new string('a', 501);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => Address.Create(longAddress));
            Assert.Contains("Address cannot exceed 500 characters", ex.Message);
        }

        [Fact]
        public void Create_WithMaxLengthAddress_Succeeds()
        {
            // Arrange
            var maxAddress = new string('a', 500);

            // Act
            var address = Address.Create(maxAddress);

            // Assert
            Assert.NotNull(address);
            Assert.Equal(500, address.Value.Length);
        }
    }
}
