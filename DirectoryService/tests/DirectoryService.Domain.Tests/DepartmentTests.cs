using Xunit;

namespace DirectoryService.Domain.Tests
{
    public class DepartmentTests
    {
        [Fact]
        public void Create_WithValidData_ReturnsDepartment()
        {
            // Arrange
            var identifier = Identifier.Create("SALES");

            // Act
            var department = Department.Create("Sales Department", identifier, null);

            // Assert
            Assert.NotNull(department);
            Assert.Equal("Sales Department", department.Name);
            Assert.Equal("SALES", department.Identifier.Value);
            Assert.Null(department.Parent);
            Assert.Equal(0, department.Depth);
            Assert.Equal("SALES", department.Path);
            Assert.True(department.IsActive);
            Assert.NotEqual(Guid.Empty, department.Id);
        }

        [Fact]
        public void Create_WithParent_SetsDepthAndPath()
        {
            // Arrange
            var parentId = Identifier.Create("COMPANY");
            var parentDept = Department.Create("Company", parentId, null);
            var childId = Identifier.Create("SALES");

            // Act
            var childDept = Department.Create("Sales", childId, parentDept);

            // Assert
            Assert.Equal(parentDept.Id, childDept.Parent?.Id);
            Assert.Equal(1, childDept.Depth);
            Assert.Equal("COMPANY/SALES", childDept.Path);
        }

        [Fact]
        public void Create_WithParent_AddsToParentChildren()
        {
            // Arrange
            var parentId = Identifier.Create("COMPANY");
            var parentDept = Department.Create("Company", parentId, null);
            var childId = Identifier.Create("SALES");

            // Act
            var childDept = Department.Create("Sales", childId, parentDept);

            // Assert
            Assert.Contains(childDept, parentDept.Childrens);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Create_WithInvalidName_ThrowsArgumentException(string? invalidName)
        {
            // Arrange
            var identifier = Identifier.Create("DEPT");

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => 
                Department.Create(invalidName ?? "", identifier, null));
            Assert.Contains("Department name is uncorrect", ex.Message);
        }

        [Fact]
        public void Create_WithNameTooLong_ThrowsArgumentException()
        {
            // Arrange
            var longName = new string('a', 121);
            var identifier = Identifier.Create("DEPT");

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => 
                Department.Create(longName, identifier, null));
            Assert.Contains("Department name is uncorrect", ex.Message);
        }

        [Fact]
        public void ChangeName_WithValidName_UpdatesName()
        {
            // Arrange
            var identifier = Identifier.Create("SALES");
            var dept = Department.Create("Sales", identifier, null);
            var oldUpdatedAt = dept.UpdatedAt;

            // Act
            System.Threading.Thread.Sleep(10);
            dept.ChangeName("Sales Department");

            // Assert
            Assert.Equal("Sales Department", dept.Name);
            Assert.True(dept.UpdatedAt > oldUpdatedAt);
        }

        [Fact]
        public void ChangeIsActive_ToInactive_UpdatesStatus()
        {
            // Arrange
            var identifier = Identifier.Create("SALES");
            var dept = Department.Create("Sales", identifier, null);

            // Act
            dept.ChangeIsActive(false);

            // Assert
            Assert.False(dept.IsActive);
        }

        [Fact]
        public void ChangeParent_WithNewParent_UpdatesPathAndDepth()
        {
            // Arrange
            var dept1Id = Identifier.Create("DEPT1");
            var dept1 = Department.Create("Department 1", dept1Id, null);

            var dept2Id = Identifier.Create("DEPT2");
            var dept2 = Department.Create("Department 2", dept2Id, null);

            var childId = Identifier.Create("CHILD");
            var child = Department.Create("Child", childId, dept1);

            // Act
            child.ChangeParent(dept2);

            // Assert
            Assert.Equal(dept2.Id, child.Parent?.Id);
            Assert.Equal("DEPT2/CHILD", child.Path);
            Assert.Equal(1, child.Depth);
            Assert.Contains(child, dept2.Childrens);
            Assert.DoesNotContain(child, dept1.Childrens);
        }

        [Fact]
        public void ChangeParent_WithSelfAsParent_ThrowsInvalidOperationException()
        {
            // Arrange
            var identifier = Identifier.Create("DEPT");
            var dept = Department.Create("Department", identifier, null);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => dept.ChangeParent(dept));
            Assert.Contains("не может быть родителем самого себя", ex.Message);
        }

        [Fact]
        public void ChangeParent_WithChildAsParent_ThrowsInvalidOperationException()
        {
            // Arrange
            var parentId = Identifier.Create("PARENT");
            var parent = Department.Create("Parent", parentId, null);

            var childId = Identifier.Create("CHILD");
            var child = Department.Create("Child", childId, parent);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => parent.ChangeParent(child));
            Assert.Contains("Нельзя назначить потомка", ex.Message);
        }

        [Fact]
        public void ChangeIdentifier_UpdatesPathForAllChildren()
        {
            // Arrange
            var parentId = Identifier.Create("PARENT");
            var parent = Department.Create("Parent", parentId, null);

            var childId = Identifier.Create("CHILD");
            var child = Department.Create("Child", childId, parent);

            // Act
            parent.ChangeIdentifier(Identifier.Create("NEWPAR"));

            // Assert
            Assert.Equal("NEWPAR", parent.Identifier.Value);
            Assert.Equal("NEWPAR/CHILD", child.Path);
        }

        [Fact]
        public void AddLocation_WithValidLocation_AddsToLocations()
        {
            // Arrange
            var identifier = Identifier.Create("SALES");
            var dept = Department.Create("Sales", identifier, null);
            var location = Location.Create("Moscow", "Address", "UTC");

            // Act
            dept.AddLocation(location);

            // Assert
            Assert.Single(dept.Locations);
            Assert.Equal(location.Id, dept.Locations[0].Location.Id);
        }

        [Fact]
        public void AddLocation_WithDuplicateLocation_DoesNotAddTwice()
        {
            // Arrange
            var identifier = Identifier.Create("SALES");
            var dept = Department.Create("Sales", identifier, null);
            var location = Location.Create("Moscow", "Address", "UTC");

            // Act
            dept.AddLocation(location);
            dept.AddLocation(location);

            // Assert
            Assert.Single(dept.Locations);
        }

        [Fact]
        public void RemoveLocation_WithValidLocation_RemovesFromLocations()
        {
            // Arrange
            var identifier = Identifier.Create("SALES");
            var dept = Department.Create("Sales", identifier, null);
            var location = Location.Create("Moscow", "Address", "UTC");
            dept.AddLocation(location);

            // Act
            dept.RemoveLocation(location);

            // Assert
            Assert.Empty(dept.Locations);
        }

        [Fact]
        public void AddPosition_WithValidPosition_AddsToPositions()
        {
            // Arrange
            var identifier = Identifier.Create("SALES");
            var dept = Department.Create("Sales", identifier, null);
            var position = Position.Create("Manager");

            // Act
            dept.AddPosition(position);

            // Assert
            Assert.Single(dept.Positions);
            Assert.Equal(position.Id, dept.Positions[0].Position.Id);
        }

        [Fact]
        public void Create_InitializesChildrenLocationsAndPositionsAsEmpty()
        {
            // Arrange & Act
            var identifier = Identifier.Create("DEPT");
            var dept = Department.Create("Department", identifier, null);

            // Assert
            Assert.NotNull(dept.Childrens);
            Assert.Empty(dept.Childrens);
            Assert.NotNull(dept.Locations);
            Assert.Empty(dept.Locations);
            Assert.NotNull(dept.Positions);
            Assert.Empty(dept.Positions);
        }
    }
}
