namespace DirectoryService.Domain
{
    public record PositionLocation
    {
        public Position Position { get; }

        public Location Location { get; }

        internal PositionLocation(Position position, Location location)
        {
            if (position == null)
                throw new ArgumentNullException(nameof(position));
            if (location == null)
                throw new ArgumentNullException(nameof(location));

            Position = position;
            Location = location;
        }
    }
}
