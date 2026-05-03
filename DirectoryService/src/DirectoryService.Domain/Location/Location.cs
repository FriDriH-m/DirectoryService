namespace DirectoryService.Domain
{
    public class Location
    {
        private const int MIN_NAME_LENGTH = 3;
        private const int MAX_NAME_LENGTH = 120;
        private const int MAX_ADDRESS_LENGTH = 500;
        private const int MAX_TIMEZONE_LENGTH = 64;

        private List<DepartmentLocation> _departments = new();
        private List<PositionLocation> _positions = new();

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public Address Address { get; private set; }

        public Timezone Timezone { get; private set; }

        public bool IsActive { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        public IReadOnlyList<DepartmentLocation> Departments => _departments;

        public IReadOnlyList<PositionLocation> Positions => _positions;

        private Location(
            Guid id,
            string name,
            Address address,
            Timezone timezone,
            bool isActive,
            DateTime createdAt,
            DateTime updatedAt)
        {
            Id = id;
            Name = name;
            Address = address;
            Timezone = timezone;
            IsActive = isActive;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public static Location Create(string name, Address address, string timezone)
        {
            ValidateName(name);

            var now = DateTime.UtcNow;
            return new Location(Guid.NewGuid(), name.Trim(), address, Timezone.Create(timezone), true, now, now);
        }

        public void UpdateName(string name)
        {
            ValidateName(name);
            var normalized = name.Trim();
            if (Name != normalized)
            {
                Name = normalized;
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void UpdateAddress(Address address)
        {
            Address = address;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateTimezone(Timezone timezone)
        {
            Timezone = timezone;
            UpdatedAt = DateTime.UtcNow;
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
            if (length < MIN_NAME_LENGTH || length > MAX_NAME_LENGTH)
            {
                throw new ArgumentException($"Name must be between {MIN_NAME_LENGTH} and {MAX_NAME_LENGTH} characters.", nameof(name));
            }
        }

        public void AddDepartment(Department department)
        {
            if (department is null)
                throw new ArgumentNullException(nameof(department));

            if (_departments.Any(d => d.Department.Id == department.Id))
                return;

            _departments.Add(new DepartmentLocation(department, this));
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveDepartment(Department department)
        {
            if (department is null)
                throw new ArgumentNullException(nameof(department));

            var departmentLocation = _departments.FirstOrDefault(d => d.Department.Id == department.Id);
            if (departmentLocation is not null)
            {
                _departments.Remove(departmentLocation);
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void AddPosition(Position position)
        {
            if (position is null)
                throw new ArgumentNullException(nameof(position));

            if (_positions.Any(p => p.Position.Id == position.Id))
                return;

            _positions.Add(new PositionLocation(position, this));
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemovePosition(Position position)
        {
            if (position is null)
                throw new ArgumentNullException(nameof(position));

            var positionLocation = _positions.FirstOrDefault(p => p.Position.Id == position.Id);
            if (positionLocation is not null)
            {
                _positions.Remove(positionLocation);
                UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}