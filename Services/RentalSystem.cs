using CinemaRentalCourseworkApp.Models;

namespace CinemaRentalCourseworkApp.Services;

public sealed class RentalSystem
{
    private readonly List<Cinema> _cinemas = new();
    private readonly List<Supplier> _suppliers = new();
    private readonly List<Film> _films = new();
    private readonly List<Rental> _rentals = new();

    public IReadOnlyList<Cinema> Cinemas => _cinemas;
    public IReadOnlyList<Supplier> Suppliers => _suppliers;
    public IReadOnlyList<Film> Films => _films;
    public IReadOnlyList<Rental> Rentals => _rentals;

    public void AddCinema(Cinema cinema)
    {
        if (cinema is null) throw new ArgumentNullException(nameof(cinema));
        if (cinema.Id == 0) cinema.Id = IdGenerator.NextId(_cinemas, c => c.Id);
        _cinemas.Add(cinema);
    }

    public void AddSupplier(Supplier supplier)
    {
        if (supplier is null) throw new ArgumentNullException(nameof(supplier));
        if (supplier.Id == 0) supplier.Id = IdGenerator.NextId(_suppliers, s => s.Id);
        _suppliers.Add(supplier);
    }

    public void AddFilm(Film film, int supplierId)
    {
        if (film is null) throw new ArgumentNullException(nameof(film));
        var supplier = _suppliers.FirstOrDefault(s => s.Id == supplierId)
            ?? throw new InvalidOperationException("Поставщик не найден.");

        film.SupplierId = supplier.Id;

        if (film.Id == 0) film.Id = IdGenerator.NextId(_films, f => f.Id);
        _films.Add(film);
    }

    public Rental CreateRental(int cinemaId, int filmId, DateTime start, DateTime end, double payment)
    {
        var cinema = _cinemas.FirstOrDefault(c => c.Id == cinemaId)
            ?? throw new InvalidOperationException("Кинотеатр не найден.");

        var film = _films.FirstOrDefault(f => f.Id == filmId)
            ?? throw new InvalidOperationException("Фильм не найден.");

        if (start.Date > end.Date)
            throw new ArgumentException("Дата начала не может быть позже даты окончания.");

        if (payment < 0)
            throw new ArgumentOutOfRangeException(nameof(payment), "Оплата не может быть отрицательной.");

        var rental = new Rental
        {
            Id = IdGenerator.NextId(_rentals, r => r.Id),
            StartDate = start.Date,
            EndDate = end.Date,
            PaymentAmount = payment,
            PenaltyRatePerDay = 0,
            CinemaId = cinema.Id,
            FilmId = film.Id,
            Cinema = cinema,
            Film = film
        };

        _rentals.Add(rental);
        return rental;
    }

    public void CloseRental(int rentalId, DateTime returnDate)
    {
        var rental = _rentals.FirstOrDefault(r => r.Id == rentalId)
            ?? throw new InvalidOperationException("Аренда не найдена.");

        rental.CloseRental(returnDate);
    }

    public List<Rental> GetActiveRentals() => _rentals.Where(r => r.IsActive).ToList();

    // Доп. методы (не противоречат UML, но удобны для UI)
    public void RemoveCinema(int cinemaId)
    {
        if (_rentals.Any(r => r.CinemaId == cinemaId && r.IsActive))
            throw new InvalidOperationException("Нельзя удалить кинотеатр с активными арендами.");

        _cinemas.RemoveAll(c => c.Id == cinemaId);
        // можно оставить историю аренд; но удаление справочника сделает аренды «осиротевшими»
        // Поэтому при удалении кинотеатра удаляем его аренды:
        _rentals.RemoveAll(r => r.CinemaId == cinemaId);
    }

    public void RemoveSupplier(int supplierId)
    {
        if (_films.Any(f => f.SupplierId == supplierId))
            throw new InvalidOperationException("Нельзя удалить поставщика, у которого есть фильмы. Сначала удалите/переназначьте фильмы.");

        _suppliers.RemoveAll(s => s.Id == supplierId);
    }

    public void RemoveFilm(int filmId)
    {
        if (_rentals.Any(r => r.FilmId == filmId && r.IsActive))
            throw new InvalidOperationException("Нельзя удалить фильм с активными арендами.");

        _films.RemoveAll(f => f.Id == filmId);
        _rentals.RemoveAll(r => r.FilmId == filmId);
    }

    public void RemoveRental(int rentalId) => _rentals.RemoveAll(r => r.Id == rentalId);

    public Cinema? GetCinemaById(int id) => _cinemas.FirstOrDefault(c => c.Id == id);
    public Supplier? GetSupplierById(int id) => _suppliers.FirstOrDefault(s => s.Id == id);
    public Film? GetFilmById(int id) => _films.FirstOrDefault(f => f.Id == id);
    public Rental? GetRentalById(int id) => _rentals.FirstOrDefault(r => r.Id == id);

    public void RebindReferences()
    {
        foreach (var rental in _rentals)
        {
            rental.Cinema = GetCinemaById(rental.CinemaId);
            rental.Film = GetFilmById(rental.FilmId);
        }
    }

    public void ReplaceAll(
        IEnumerable<Cinema> cinemas,
        IEnumerable<Supplier> suppliers,
        IEnumerable<Film> films,
        IEnumerable<Rental> rentals)
    {
        _cinemas.Clear();
        _suppliers.Clear();
        _films.Clear();
        _rentals.Clear();

        _cinemas.AddRange(cinemas);
        _suppliers.AddRange(suppliers);
        _films.AddRange(films);
        _rentals.AddRange(rentals);

        RebindReferences();
    }
}
