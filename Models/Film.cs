namespace CinemaRentalCourseworkApp.Models;

public sealed class Film
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string ScriptAuthor { get; set; } = string.Empty;
    public string Director { get; set; } = string.Empty;
    public string ProducerCompany { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public double PurchaseCost { get; set; }

    /// <summary>
    /// Для связи с поставщиком (в UML связь 1 -> 0..*).
    /// </summary>
    public int SupplierId { get; set; }

    public void ChangePrice(double newCost)
    {
        if (newCost < 0) throw new ArgumentOutOfRangeException(nameof(newCost), "Стоимость не может быть отрицательной.");
        PurchaseCost = newCost;
    }

    public override string ToString() => $@"{Title} ({ReleaseYear})";
}
