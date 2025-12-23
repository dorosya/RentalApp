using CinemaRentalCourseworkApp.Models;

namespace CinemaRentalCourseworkApp.Persistence;

public sealed class RentalSystemState
{
    public List<Cinema> Cinemas { get; set; } = new();
    public List<Supplier> Suppliers { get; set; } = new();
    public List<Film> Films { get; set; } = new();
    public List<Rental> Rentals { get; set; } = new();
}
