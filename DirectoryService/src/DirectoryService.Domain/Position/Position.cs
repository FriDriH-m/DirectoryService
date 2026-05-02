namespace DirectoryService.Domain
{
    public class Position
    {
        private List<PositionLocation> _locations = new();
        private List<DepartmentPosition> _departments = new();

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string? Description { get; private set; }

        public bool IsActive { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        public IReadOnlyList<PositionLocation> Locations => _locations;

        public IReadOnlyList<DepartmentPosition> Departments => _departments;

        private Position(Guid id, string name, string? description, bool isActive, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
            Name = name;
            Description = description;
            IsActive = isActive;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public static Position Create(string name, string? description = null)
        {
            ValidateName(name);
            ValidateDescription(description);

            var now = DateTime.UtcNow;
            return new Position(Guid.NewGuid(), name.Trim(), description?.Trim(), true, now, now);
        }

        public void UpdateName(string name)
        {
            ValidateName(name);
            if (Name != name)
            {
                Name = name.Trim();
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void UpdateDescription(string? description)
        {
            ValidateDescription(description);
            var normalized = description?.Trim();
            if (Description != normalized)
            {
                Description = normalized;
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void ChangeIsActive(bool isActive)
        {
            IsActive = isActive;
            UpdatedAt = DateTime.UtcNow;
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name is required.", nameof(name));
            }

            var length = name.Trim().Length;
            if (length < 3 || length > 100)
                throw new ArgumentException("Name must be between 3 and 100 characters.", nameof(name));
        }

        private static void ValidateDescription(string? description)
        {
            if (description is null) return;
            if (description.Trim().Length > 1000)
                throw new ArgumentException("Description must be 1000 characters or fewer.", nameof(description));
        }

        public void AddLocation(Location location)
        {
            if (location is null)
                throw new ArgumentNullException(nameof(location));

            if (_locations.Any(l => l.Location.Id == location.Id))
                return;

            _locations.Add(new PositionLocation(this, location));
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveLocation(Location location)
        {
            if (location is null)
                throw new ArgumentNullException(nameof(location));

            var positionLocation = _locations.FirstOrDefault(l => l.Location.Id == location.Id);
            if (positionLocation is not null)
            {
                _locations.Remove(positionLocation);
                UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
