using Xunit;

namespace DirectoryService.Domain.Tests
{
    public class IdentifierTests
    {
        [Fact]
        public void Create_WithValidIdentifier_ReturnsIdentifier()
        {
            // Arrange & Act
            var identifier = Identifier.Create("DEPT001");

            // Assert
            Assert.NotNull(identifier);
            Assert.Equal("DEPT001", identifier.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Create_WithNullOrEmpty_ThrowsArgumentException(string? invalidId)
        {
            // Arrange & Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => Identifier.Create(invalidId ?? ""));
            Assert.Contains("Identifier cannot be null or empty", ex.Message);
        }

        [Fact]
        public void Create_WithIdentifierExceedingMaxLength_ThrowsArgumentException()
        {
            // Arrange
            var longId = "TOOLONGIDENTIFIER";

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => Identifier.Create(longId));
            Assert.Contains("Identifier cannot exceed 10 characters", ex.Message);
        }

        [Fact]
        public void Create_WithLowercaseLetters_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => Identifier.Create("Dept001"));
            Assert.Contains("Identifier must be in uppercase", ex.Message);
        }

        [Theory]
        [InlineData("DEPT@001")]
        [InlineData("DEPT-001")]
        [InlineData("DEPT 001")]
        public void Create_WithSpecialCharacters_ThrowsArgumentException(string invalidId)
        {
            // Arrange & Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => Identifier.Create(invalidId));
            Assert.Contains("uncorrect format", ex.Message);
        }

        [Fact]
        public void Create_WithMaxLengthIdentifier_Succeeds()
        {
            // Arrange & Act
            var identifier = Identifier.Create("DEPT00001");

            // Assert
            Assert.NotNull(identifier);
            Assert.Equal(9, identifier.Value.Length);
        }

        [Theory]
        [InlineData("LETTERS")]
        [InlineData("123456")]
        [InlineData("ABC123")]
        public void Create_WithValidFormats_Succeeds(string validId)
        {
            // Arrange & Act
            var identifier = Identifier.Create(validId);

            // Assert
            Assert.NotNull(identifier);
            Assert.Equal(validId, identifier.Value);
        }
    }
}
