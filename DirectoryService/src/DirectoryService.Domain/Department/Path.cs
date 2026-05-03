namespace DirectoryService.Domain
{
    public class Path
    {
        private const string PATH_SEPARATOR = "/";

        private List<string> _pathIdentifiers = new();

        public IReadOnlyList<string> PathIdentifiers => _pathIdentifiers;

        public string Value { get; }

        public short Depth { get; }

        public Path(Department department)
        {
            Value = department.Parent is not null ?
                $"{department.Parent.Path.Value}{PATH_SEPARATOR}{department.Identifier.Value}" : department.Identifier.Value;

            Depth = department.Parent is not null ? (short)(department.Parent.Path.Depth + 1) : (short)0;

            BuildPathIdentifiers();
        }

        private void BuildPathIdentifiers()
        {
            string[] values = Value.Split(PATH_SEPARATOR);

            _pathIdentifiers.AddRange(values);
        }
    }
}
