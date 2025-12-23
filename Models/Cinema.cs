namespace CinemaRentalCourseworkApp.Models;

public sealed class Cinema : Organization, IContactable
{
    // Composition
    public Address Address { get; set; } = new();

    public int SeatsCount { get; set; }
    public string DirectorFullName { get; set; } = string.Empty;
    public string OwnerFullName { get; set; } = string.Empty;

    /// <summary>
    /// В UML указан метод updateInfo(). В рамках UI он не обязателен,
    /// но оставлен как точка расширения (например, для пересчётов/валидации).
    /// </summary>
    public void UpdateInfo()
    {
        // Здесь можно выполнять дополнительную валидацию/нормализацию данных.
    }

    public string GetContactInfo()
        => $@"Кинотеатр: {Name}; адрес: {Address.ToDisplayString()}; тел.: {Phone}";

    public override string ToString() => $@"{Name} (мест: {SeatsCount}, ID: {Id})";
}
