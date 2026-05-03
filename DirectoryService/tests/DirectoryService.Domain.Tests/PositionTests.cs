using Xunit;

namespace DirectoryService.Domain.Tests
{
    public class PositionTests
    {
        [Fact]
        public void Create_WithValidData_ReturnsPosition()
        {
            // Arrange & Act
            var position = Position.Create("Manager", "Team manager position");

            // Assert
            Assert.NotNull(position);
            Assert.Equal("Manager", position.Name);
            Assert.Equal("Team manager position", position.Description);
            Assert.True(position.IsActive);
            Assert.NotEqual(Guid.Empty, position.Id);
        }

        [Fact]
        public void Create_WithoutDescription_Succeeds()
        {
            // Arrange & Act
            var position = Position.Create("Developer");

            // Assert
            Assert.NotNull(position);
            Assert.Equal("Developer", position.Name);
            Assert.Null(position.Description);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Create_WithInvalidName_ThrowsArgumentException(string? invalidName)
        {
            // Arrange & Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => Position.Create(invalidName ?? ""));
            Assert.Contains("Name is required", ex.Message);
        }

        [Fact]
        public void Create_WithNameTooShort_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => Position.Create("Ma"));
            Assert.Contains("Name must be between 3 and 100 characters", ex.Message);
        }

        [Fact]
        public void Create_WithNameTooLong_ThrowsArgumentException()
        {
            // Arrange
            var longName = new string('a', 101);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => Position.Create(longName));
            Assert.Contains("Name must be between 3 and 100 characters", ex.Message);
        }

        [Fact]
        public void Create_WithDescriptionTooLong_ThrowsArgumentException()
        {
            // Arrange
            var longDescription = new string('a', 1001);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => Position.Create("Manager", longDescription));
            Assert.Contains("Description must be 1000 characters or fewer", ex.Message);
        }

        [Fact]
        public void Create_WithWhitespaceInName_TrimsValue()
        {
            // Arrange & Act
            var position = Position.Create("  Developer  ", "  Some desc  ");

            // Assert
            Assert.Equal("Developer", position.Name);
            Assert.Equal("Some desc", position.Description);
        }

        [Fact]
        public void UpdateName_WithValidName_UpdatesName()
        {
            // Arrange
            var position = Position.Create("Manager");
            var oldUpdatedAt = position.UpdatedAt;

            // Act
            System.Threading.Thread.Sleep(10);
            position.UpdateName("Senior Manager");

            // Assert
            Assert.Equal("Senior Manager", position.Name);
            Assert.True(position.UpdatedAt > oldUpdatedAt);
        }

        [Fact]
        public void UpdateName_WithSameName_DoesNotChangeUpdatedAt()
        {
            // Arrange
            var position = Position.Create("Manager");
            var oldUpdatedAt = position.UpdatedAt;

            // Act
            position.UpdateName("Manager");

            // Assert
            Assert.Equal(oldUpdatedAt, position.UpdatedAt);
        }

        [Fact]
        public void UpdateDescription_WithValidDescription_UpdatesDescription()
        {
            // Arrange
            var position = Position.Create("Manager", "Old description");
            var oldUpdatedAt = position.UpdatedAt;

            // Act
            System.Threading.Thread.Sleep(10);
            position.UpdateDescription("New description");

            // Assert
            Assert.Equal("New description", position.Description);
            Assert.True(position.UpdatedAt > oldUpdatedAt);
        }

        [Fact]
        public void UpdateDescription_WithNull_ClearsDescription()
        {
            // Arrange
            var position = Position.Create("Manager", "Some description");

            // Act
            position.UpdateDescription(null);

            // Assert
            Assert.Null(position.Description);
        }

        [Fact]
        public void ChangeIsActive_ToInactive_UpdatesStatus()
        {
            // Arrange
            var position = Position.Create("Manager");

            // Act
            position.ChangeIsActive(false);

            // Assert
            Assert.False(position.IsActive);
        }

        [Fact]
        public void ChangeIsActive_ToActive_UpdatesStatus()
        {
            // Arrange
            var position = Position.Create("Manager");
            position.ChangeIsActive(false);

            // Act
            position.ChangeIsActive(true);

            // Assert
            Assert.True(position.IsActive);
        }
    }
}
