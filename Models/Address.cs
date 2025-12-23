namespace CinemaRentalCourseworkApp.Models;

public sealed class Address
{
    public string Region { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string House { get; set; } = string.Empty;

    public string ToDisplayString()
    {
        var parts = new[] { Region, City, Street, House }
            .Where(p => !string.IsNullOrWhiteSpace(p));
        return string.Join(", ", parts);
    }

    public override string ToString() => ToDisplayString();
}
