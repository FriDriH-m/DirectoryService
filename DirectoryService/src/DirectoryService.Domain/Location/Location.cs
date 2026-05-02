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
            string address,
            Timezone timezone,
            bool isActive,
            DateTime createdAt,
            DateTime updatedAt)
        {
            Id = id;
            Name = name;
            Address = Address.Create(address);
            Timezone = timezone;
            IsActive = isActive;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public static Location Create(string name, string address, string timezone)
        {
            ValidateName(name);
            ValidateAddress(address);
            ValidateTimezone(timezone);

            var now = DateTime.UtcNow;
            return new Location(Guid.NewGuid(), name.Trim(), address.Trim(), Timezone.Create(timezone), true, now, now);
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

        public void UpdateAddress(string address)
        {
            ValidateAddress(address);
            var normalized = address.Trim();
            if (Address.Value != normalized)
            {
                Address = Address.Create(normalized);
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void UpdateTimezone(string timezone)
        {
            ValidateTimezone(timezone);
            var normalized = timezone.Trim();
            if (Timezone.Value != normalized)
            {
                Timezone = Timezone.Create(normalized);
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
            if (length < MIN_NAME_LENGTH || length > MAX_NAME_LENGTH)
            {
                throw new ArgumentException($"Name must be between {MIN_NAME_LENGTH} and {MAX_NAME_LENGTH} characters.", nameof(name));
            }
        }

        private static void ValidateAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("Address is required.", nameof(address));
            }

            if (address.Trim().Length > MAX_ADDRESS_LENGTH)
            {
                throw new ArgumentException($"Address must be {MAX_ADDRESS_LENGTH} characters or fewer.", nameof(address));
            }
        }

        private static void ValidateTimezone(string timezone)
        {
            if (string.IsNullOrWhiteSpace(timezone))
            {
                throw new ArgumentException("Timezone is required.", nameof(timezone));
            }

            var normalized = timezone.Trim();
            // IANA-формат: Region/Location (например, Europe/Moscow) или UTC
            if (!normalized.Contains('/') && !normalized.Equals("UTC", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Invalid timezone format", nameof(timezone));
            }

            if (normalized.Length > MAX_TIMEZONE_LENGTH)
            {
                throw new ArgumentException($"Timezone must be {MAX_TIMEZONE_LENGTH} characters or fewer.", nameof(timezone));
            }
        }

        internal void AddDepartment(Department department)
        {
            if (department is null)
                throw new ArgumentNullException(nameof(department));

            if (_departments.Any(d => d.Department.Id == department.Id))
                return;

            _departments.Add(new DepartmentLocation(department, this));
            UpdatedAt = DateTime.UtcNow;
        }

        internal void RemoveDepartment(Department department)
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

        internal void AddPosition(Position position)
        {
            if (position is null)
                throw new ArgumentNullException(nameof(position));

            if (_positions.Any(p => p.Position.Id == position.Id))
                return;

            _positions.Add(new PositionLocation(position, this));
            UpdatedAt = DateTime.UtcNow;
        }

        internal void RemovePosition(Position position)
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