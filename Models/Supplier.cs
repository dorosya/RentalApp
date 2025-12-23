namespace CinemaRentalCourseworkApp.Models;

public sealed class Supplier : Organization, IContactable
{
    public string GetContactInfo()
        => $@"Поставщик: {Name}; тел.: {Phone}; {LegalInfo}; {BankDetails}";
}
