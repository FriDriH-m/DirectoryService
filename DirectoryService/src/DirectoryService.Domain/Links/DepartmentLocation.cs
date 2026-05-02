namespace DirectoryService.Domain
{
    public record DepartmentLocation
    {
        public Department Department { get; }

        public Location Location { get; }

        internal DepartmentLocation(Department department, Location location)
        {
            if (department == null)
                throw new ArgumentNullException(nameof(department));
            if (location == null)
                throw new ArgumentNullException(nameof(location));

            Department = department;
            Location = location;
        }
    }
}
