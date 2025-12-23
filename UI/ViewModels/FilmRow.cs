namespace CinemaRentalCourseworkApp.UI.ViewModels;

public sealed class FilmRow
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public int ReleaseYear { get; init; }
    public string Director { get; init; } = string.Empty;
    public string ScriptAuthor { get; init; } = string.Empty;
    public string ProducerCompany { get; init; } = string.Empty;
    public double PurchaseCost { get; init; }
    public int SupplierId { get; init; }
    public string SupplierName { get; init; } = string.Empty;
}
