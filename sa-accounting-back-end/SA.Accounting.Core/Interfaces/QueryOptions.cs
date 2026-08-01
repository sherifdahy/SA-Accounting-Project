namespace SA.Accounting.Core.Interfaces;

public sealed record QueryOptions
{
    public static readonly QueryOptions Default = new();

    public bool IncludeDeleted { get; init; }
    public bool AsNoTracking { get; init; }
}
