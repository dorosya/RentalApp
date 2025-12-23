namespace CinemaRentalCourseworkApp.UI.ViewModels;

public sealed class CinemaRow
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public int SeatsCount { get; init; }
    public string DirectorFullName { get; init; } = string.Empty;
    public string OwnerFullName { get; init; } = string.Empty;
    public string Inn { get; init; } = string.Empty;
    public string Bank { get; init; } = string.Empty;
}
