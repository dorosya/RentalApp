namespace CinemaRentalCourseworkApp.Models;

public sealed class BankDetails
{
    public string BankName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;

    public override string ToString() => $@"{BankName}, счёт: {AccountNumber}";
}
