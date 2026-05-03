using Xunit;

namespace DirectoryService.Domain.Tests
{
    public class TimezoneTests
    {
        [Fact]
        public void Create_WithValidUtc_ReturnsTimezone()
        {
            // Arrange & Act
            var timezone = Timezone.Create("UTC");

            // Assert
            Assert.NotNull(timezone);
            Assert.Equal("UTC", timezone.Value);
        }

        [Fact]
        public void Create_WithValidIanaFormat_ReturnsTimezone()
        {
            // Arrange & Act
            var timezone = Timezone.Create("Europe/Moscow");

            // Assert
            Assert.NotNull(timezone);
            Assert.Equal("Europe/Moscow", timezone.Value);
        }

        [Theory]
        [InlineData("America/New_York")]
        [InlineData("Asia/Tokyo")]
        [InlineData("Australia/Sydney")]
        public void Create_WithValidIanaFormats_Succeeds(string validTimezone)
        {
            // Arrange & Act
            var timezone = Timezone.Create(validTimezone);

            // Assert
            Assert.NotNull(timezone);
            Assert.Equal(validTimezone, timezone.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Create_WithNullOrEmpty_ThrowsArgumentException(string? invalidTz)
        {
            // Arrange & Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => Timezone.Create(invalidTz ?? ""));
            Assert.Contains("Timezone cannot be null or empty", ex.Message);
        }

        [Fact]
        public void Create_WithTimezoneExceedingMaxLength_ThrowsArgumentException()
        {
            // Arrange
            var longTimezone = new string('a', 65);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => Timezone.Create(longTimezone));
            Assert.Contains("Timezone cannot exceed 64 characters", ex.Message);
        }

        [Theory]
        [InlineData("InvalidFormat")]
        [InlineData("Europe")]
        [InlineData("Moscow")]
        public void Create_WithInvalidFormat_ThrowsArgumentException(string invalidFormat)
        {
            // Arrange & Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => Timezone.Create(invalidFormat));
            Assert.Contains("Invalid timezone format", ex.Message);
        }

        [Fact]
        public void Create_WithWhitespace_TrimsValue()
        {
            // Arrange & Act
            var timezone = Timezone.Create("  UTC  ");

            // Assert
            Assert.Equal("UTC", timezone.Value);
        }

        [Fact]
        public void Equals_WithDifferentValue_ReturnsFalse()
        {
            // Arrange
            var tz1 = Timezone.Create("UTC");
            var tz2 = Timezone.Create("Europe/Moscow");

            // Act & Assert
            Assert.NotEqual(tz1, tz2);
        }
    }
}
