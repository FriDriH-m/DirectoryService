using Xunit;

namespace DirectoryService.Domain.Tests
{
    public class LocationTests
    {
        [Fact]
        public void Create_WithValidData_ReturnsLocation()
        {
            // Arrange & Act
            var location = Location.Create("Moscow Office", "Arbat 12", "Europe/Moscow");

            // Assert
            Assert.NotNull(location);
            Assert.Equal("Moscow Office", location.Name);
            Assert.Equal("Arbat 12", location.Address.Value);
            Assert.Equal("Europe/Moscow", location.Timezone.Value);
            Assert.True(location.IsActive);
            Assert.NotEqual(Guid.Empty, location.Id);
        }

        [Fact]
        public void Create_WithUtcTimezone_Succeeds()
        {
            // Arrange & Act
            var location = Location.Create("UTC Office", "Address", "UTC");

            // Assert
            Assert.Equal("UTC", location.Timezone.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Create_WithInvalidName_ThrowsArgumentException(string? invalidName)
        {
            // Arrange & Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => 
                Location.Create(invalidName ?? "", "Address", "UTC"));
            Assert.Contains("Name is required", ex.Message);
        }

        [Fact]
        public void Create_WithNameTooShort_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => 
                Location.Create("Mo", "Address", "UTC"));
            Assert.Contains("Name must be between 3 and 120 characters", ex.Message);
        }

        [Fact]
        public void Create_WithInvalidAddress_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => 
                Location.Create("Moscow Office", "", "UTC"));
            Assert.Contains("Address is required", ex.Message);
        }

        [Fact]
        public void Create_WithInvalidTimezone_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => 
                Location.Create("Moscow Office", "Address", "InvalidTZ"));
            Assert.Contains("Invalid timezone format", ex.Message);
        }

        [Fact]
        public void Create_WithWhitespaceValues_TrimsAll()
        {
            // Arrange & Act
            var location = Location.Create("  Moscow Office  ", "  Arbat 12  ", "  UTC  ");

            // Assert
            Assert.Equal("Moscow Office", location.Name);
            Assert.Equal("Arbat 12", location.Address.Value);
            Assert.Equal("UTC", location.Timezone.Value);
        }

        [Fact]
        public void UpdateName_WithValidName_UpdatesName()
        {
            // Arrange
            var location = Location.Create("Moscow Office", "Address", "UTC");
            var oldUpdatedAt = location.UpdatedAt;

            // Act
            System.Threading.Thread.Sleep(10);
            location.UpdateName("New Moscow Office");

            // Assert
            Assert.Equal("New Moscow Office", location.Name);
            Assert.True(location.UpdatedAt > oldUpdatedAt);
        }

        [Fact]
        public void UpdateName_WithSameName_DoesNotChangeUpdatedAt()
        {
            // Arrange
            var location = Location.Create("Moscow Office", "Address", "UTC");
            var oldUpdatedAt = location.UpdatedAt;

            // Act
            location.UpdateName("Moscow Office");

            // Assert
            Assert.Equal(oldUpdatedAt, location.UpdatedAt);
        }

        [Fact]
        public void UpdateAddress_WithValidAddress_UpdatesAddress()
        {
            // Arrange
            var location = Location.Create("Moscow Office", "Old Address", "UTC");
            var oldUpdatedAt = location.UpdatedAt;

            // Act
            System.Threading.Thread.Sleep(10);
            location.UpdateAddress("New Address");

            // Assert
            Assert.Equal("New Address", location.Address.Value);
            Assert.True(location.UpdatedAt > oldUpdatedAt);
        }

        [Fact]
        public void UpdateTimezone_WithValidTimezone_UpdatesTimezone()
        {
            // Arrange
            var location = Location.Create("Moscow Office", "Address", "UTC");
            var oldUpdatedAt = location.UpdatedAt;

            // Act
            System.Threading.Thread.Sleep(10);
            location.UpdateTimezone("Europe/London");

            // Assert
            Assert.Equal("Europe/London", location.Timezone.Value);
            Assert.True(location.UpdatedAt > oldUpdatedAt);
        }

        [Fact]
        public void UpdateTimezone_WithSameTimezone_DoesNotChangeUpdatedAt()
        {
            // Arrange
            var location = Location.Create("Moscow Office", "Address", "UTC");
            var oldUpdatedAt = location.UpdatedAt;

            // Act
            location.UpdateTimezone("UTC");

            // Assert
            Assert.Equal(oldUpdatedAt, location.UpdatedAt);
        }

        [Fact]
        public void ChangeIsActive_ToInactive_UpdatesStatus()
        {
            // Arrange
            var location = Location.Create("Moscow Office", "Address", "UTC");

            // Act
            location.ChangeIsActive(false);

            // Assert
            Assert.False(location.IsActive);
        }

        [Fact]
        public void ChangeIsActive_ToActive_UpdatesStatus()
        {
            // Arrange
            var location = Location.Create("Moscow Office", "Address", "UTC");
            location.ChangeIsActive(false);

            // Act
            location.ChangeIsActive(true);

            // Assert
            Assert.True(location.IsActive);
        }

        [Fact]
        public void Create_InitializesDepartmentsAndPositionsAsEmpty()
        {
            // Arrange & Act
            var location = Location.Create("Moscow Office", "Address", "UTC");

            // Assert
            Assert.NotNull(location.Departments);
            Assert.Empty(location.Departments);
            Assert.NotNull(location.Positions);
            Assert.Empty(location.Positions);
        }
    }
}
