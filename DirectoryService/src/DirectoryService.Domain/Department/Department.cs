namespace DirectoryService.Domain
{
    public class Department
    {
        private const short MAX_NAME_LENGTH = 120;

        private List<Department> _children = new();
        private List<DepartmentLocation> _locations = new();
        private List<DepartmentPosition> _positions = new();

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public Identifier Identifier { get; private set; }

        public Department? Parent { get; private set; }

        public IReadOnlyList<Department> Childrens => _children;

        public IReadOnlyList<DepartmentLocation> Locations => _locations;

        public IReadOnlyList<DepartmentPosition> Positions => _positions;

        public Path Path { get; private set; }

        public bool IsActive { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        private Department(string name, Identifier identifier, Department? parentDepartment)
        {
            Id = Guid.NewGuid();
            Name = name;
            Identifier = identifier;
            Parent = parentDepartment;
            Path = new Path(this);
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public static Department Create(string name, Identifier identifier, Department? parent)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > MAX_NAME_LENGTH)
            {
                throw new ArgumentException("Department name is uncorrect.", nameof(name));
            }

            Department thisDepartment = new(name, identifier, parent);

            if (parent != null)
            {
                parent.AddChildren(thisDepartment);
            }

            return thisDepartment;
        }

        public void UpdatePath()
        {
            bool parentIsNotNull = Parent is not null;

            Path = new Path(this);

            UpdateChildrenPath();

            UpdatedAt = DateTime.UtcNow;
        }

        public void ChangeParent(Department newParent)
        {
            if (newParent.Id == this.Id)
            {
                throw new InvalidOperationException("Отдел не может быть родителем самого себя.");
            }

            if (IsAncestorOrSelf(newParent))
            {
                throw new InvalidOperationException("Нельзя назначить потомка или себя в качестве родителя.");
            }

            Parent?.RemoveChildren(this);
            Parent = newParent;
            Parent?.AddChildren(this);
            UpdatePath();

            UpdatedAt = DateTime.UtcNow;
        }

        public void ChangeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > MAX_NAME_LENGTH)
            {
                throw new ArgumentException("Department name is uncorrect.", nameof(name));
            }

            Name = name;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ChangeIsActive(bool isActive)
        {
            IsActive = isActive;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ChangeIdentifier(Identifier identifier)
        {
            Identifier = identifier;
            Path = new Path(this);

            UpdateChildrenPath();

            UpdatedAt = DateTime.UtcNow;
        }

        public void AddLocation(Location location)
        {
            if (location is null)
                throw new ArgumentNullException(nameof(location));

            if (_locations.Any(l => l.Location.Id == location.Id))
                return;

            _locations.Add(new DepartmentLocation(this, location));
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveLocation(Location location)
        {
            if (location is null)
                throw new ArgumentNullException(nameof(location));

            var departmentLocation = _locations.FirstOrDefault(l => l.Location.Id == location.Id);
            if (departmentLocation is not null)
            {
                _locations.Remove(departmentLocation);
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void AddPosition(Position position)
        {
            if (position is null)
                throw new ArgumentNullException(nameof(position));

            if (_positions.Any(p => p.Position.Id == position.Id))
                return;

            _positions.Add(new DepartmentPosition(this, position));
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemovePosition(Position position)
        {
            if (position is null)
                throw new ArgumentNullException(nameof(position));

            var departmentPosition = _positions.FirstOrDefault(p => p.Position.Id == position.Id);
            if (departmentPosition is not null)
            {
                _positions.Remove(departmentPosition);
                UpdatedAt = DateTime.UtcNow;
            }
        }

        private void AddChildren(Department children)
        {
            _children.Add(children);
            UpdatedAt = DateTime.UtcNow;
        }

        private void RemoveChildren(Department children)
        {
            _children.Remove(children);
            UpdatedAt = DateTime.UtcNow;
        }

        private bool IsAncestorOrSelf(Department candidate)
        {
            var stack = new Stack<Department>();
            stack.Push(this);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                if (current.Id == candidate.Id)
                    return true;

                for (int i = 0; i < current._children.Count; i++)
                {
                    stack.Push(current._children[i]);
                }
            }

            return false;
        }

        private void UpdateChildrenPath()
        {
            var stack = new Stack<Department>();
            stack.Push(this);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                current.Path = new Path(current);

                current.UpdatedAt = DateTime.UtcNow;

                for (int i = current._children.Count - 1; i >= 0; i--)
                {
                    stack.Push(current._children[i]);
                }
            }
        }
    }
}
