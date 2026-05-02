namespace DirectoryService.Domain
{
    public record DepartmentPosition
    {
        public Department Department { get; }

        public Position Position { get; }

        internal DepartmentPosition(Department department, Position position)
        {
            if (department == null)
                throw new ArgumentNullException(nameof(department));
            if (position == null)
                throw new ArgumentNullException(nameof(position));

            Department = department;
            Position = position;
        }
    }
}
