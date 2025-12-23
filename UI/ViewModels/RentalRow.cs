namespace CinemaRentalCourseworkApp.UI.ViewModels;

public sealed class RentalRow
{
    public int Id { get; init; }
    public int CinemaId { get; init; }
    public string Cinema { get; init; } = string.Empty;
    public int FilmId { get; init; }
    public string Film { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public DateTime? ActualReturnDate { get; init; }
    public double PaymentAmount { get; init; }
    public double PenaltyRatePerDay { get; init; }
    public double PenaltyAmount { get; init; }
    public bool IsActive { get; init; }
    public bool IsOverdueToday { get; init; }
}
