namespace CinemaRentalCourseworkApp.Models;

public abstract class Organization
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    // Composition
    public BankDetails BankDetails { get; set; } = new();
    public LegalInfo LegalInfo { get; set; } = new();

    public string GetName() => Name;
    public string GetPhone() => Phone;

    public override string ToString() => $@"{Name} (ID: {Id})";
}
