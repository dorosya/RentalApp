using System.Text.Json.Serialization;

namespace CinemaRentalCourseworkApp.Models;

public sealed class Rental
{
    public int Id { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public double PaymentAmount { get; set; }
    public double PenaltyRatePerDay { get; set; }

    public DateTime? ActualReturnDate { get; set; }

    // Связи (в JSON сохраняем только id, а ссылки восстанавливаем при загрузке)
    public int FilmId { get; set; }
    public int CinemaId { get; set; }

    [JsonIgnore]
    public Film? Film { get; set; }

    [JsonIgnore]
    public Cinema? Cinema { get; set; }

    public void CloseRental(DateTime returnDate)
    {
        if (returnDate.Date < StartDate.Date)
            throw new ArgumentException("Дата возврата не может быть раньше даты начала аренды.");

        ActualReturnDate = returnDate.Date;
    }

    public bool IsOverdue(DateTime checkDate)
    {
        var effectiveDate = (ActualReturnDate ?? checkDate.Date);
        return effectiveDate.Date > EndDate.Date;
    }

    public double CalculatePenalty()
    {
        if (ActualReturnDate is null) return 0;

        var overdueDays = (ActualReturnDate.Value.Date - EndDate.Date).Days;
        if (overdueDays <= 0) return 0;

        return overdueDays * PenaltyRatePerDay;
    }

    public bool IsActive => ActualReturnDate is null;

    public override string ToString()
    {
        var film = Film?.Title ?? $"FilmId={FilmId}";
        var cinema = Cinema?.Name ?? $"CinemaId={CinemaId}";
        return $@"Аренда #{Id}: {cinema} — {film}";
    }
}
