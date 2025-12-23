namespace CinemaRentalCourseworkApp.Models;

public sealed class LegalInfo
{
    public string Inn { get; set; } = string.Empty;
    public string LegalAddress { get; set; } = string.Empty;

    public override string ToString() => $@"ИНН: {Inn}, юр. адрес: {LegalAddress}";
}
