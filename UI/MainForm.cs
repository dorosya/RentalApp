using CinemaRentalCourseworkApp.Models;
using CinemaRentalCourseworkApp.Persistence;
using CinemaRentalCourseworkApp.Services;
using CinemaRentalCourseworkApp.UI.Dialogs;
using CinemaRentalCourseworkApp.UI.ViewModels;

namespace CinemaRentalCourseworkApp.UI;

public sealed class MainForm : Form
{
    private readonly RentalSystem _system = new();
    private readonly JsonDataStore _store;

    private readonly BindingSource _cinemasSource = new();
    private readonly BindingSource _suppliersSource = new();
    private readonly BindingSource _filmsSource = new();
    private readonly BindingSource _rentalsSource = new();

    private readonly DataGridView _gridCinemas = new()
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        AutoGenerateColumns = true,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        MultiSelect = false
    };

    private readonly DataGridView _gridSuppliers = new()
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        AutoGenerateColumns = true,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        MultiSelect = false
    };

    private readonly DataGridView _gridFilms = new()
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        AutoGenerateColumns = true,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        MultiSelect = false
    };

    private readonly DataGridView _gridRentals = new()
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        AutoGenerateColumns = true,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        MultiSelect = false
    };

    private readonly ToolStripStatusLabel _statusPath = new();

    private readonly CheckBox _cbActiveOnly = new() { Text = "Только активные", AutoSize = true };
    private readonly CheckBox _cbOverdueOnly = new() { Text = "Только просроченные (на сегодня)", AutoSize = true };

    public MainForm()
    {
        Text = "Система учета проката кинофильмов (вариант 3)";
        Width = 1200;
        Height = 700;
        StartPosition = FormStartPosition.CenterScreen;

        _store = new JsonDataStore(JsonDataStore.DefaultPath());
        _store.LoadInto(_system);

        var menu = BuildMenu();
        var tabs = BuildTabs();
        var status = BuildStatusBar();

        Controls.Add(tabs);
        Controls.Add(menu);
        Controls.Add(status);

        MainMenuStrip = menu;

        _gridCinemas.DataSource = _cinemasSource;
        _gridSuppliers.DataSource = _suppliersSource;
        _gridFilms.DataSource = _filmsSource;
        _gridRentals.DataSource = _rentalsSource;

        _cbActiveOnly.CheckedChanged += (_, __) => RefreshRentals();
        _cbOverdueOnly.CheckedChanged += (_, __) => RefreshRentals();

        RefreshAll();
    }

    private MenuStrip BuildMenu()
    {
        var menu = new MenuStrip();

        var file = new ToolStripMenuItem("Файл");
        file.DropDownItems.Add(new ToolStripMenuItem("Сохранить", null, (_, __) => Save(), Keys.Control | Keys.S));
        file.DropDownItems.Add(new ToolStripMenuItem("Перезагрузить из файла", null, (_, __) => Reload()));
        file.DropDownItems.Add(new ToolStripSeparator());
        file.DropDownItems.Add(new ToolStripMenuItem("Выход", null, (_, __) => Close()));

        var help = new ToolStripMenuItem("Справка");
        help.DropDownItems.Add(new ToolStripMenuItem("О программе", null, (_, __) =>
        {
            UiHelpers.ShowInfo(this,
                "Курсовая работа по программированию (C#, WinForms).\n" +
                "Хранение данных: JSON.\n" +
                "Предметная область: прокат кинофильмов кинотеатрам.\n\n" +
                $"Файл данных: {_store.FilePath}",
                "О программе");
        }));

        menu.Items.Add(file);
        menu.Items.Add(help);

        return menu;
    }

    private Control BuildTabs()
    {
        var tab = new TabControl { Dock = DockStyle.Fill };

        tab.TabPages.Add(BuildCinemasTab());
        tab.TabPages.Add(BuildSuppliersTab());
        tab.TabPages.Add(BuildFilmsTab());
        tab.TabPages.Add(BuildRentalsTab());

        return tab;
    }

    private TabPage BuildCinemasTab()
    {
        var page = new TabPage("Кинотеатры");

        var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(8) };
        var btnAdd = new Button { Text = "Добавить", Width = 110 };
        var btnEdit = new Button { Text = "Изменить", Width = 110 };
        var btnDel = new Button { Text = "Удалить", Width = 110 };

        btnAdd.Click += (_, __) => AddCinema();
        btnEdit.Click += (_, __) => EditCinema();
        btnDel.Click += (_, __) => DeleteCinema();

        top.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDel });

        page.Controls.Add(_gridCinemas);
        page.Controls.Add(top);
        return page;
    }

    private TabPage BuildSuppliersTab()
    {
        var page = new TabPage("Поставщики");

        var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(8) };
        var btnAdd = new Button { Text = "Добавить", Width = 110 };
        var btnEdit = new Button { Text = "Изменить", Width = 110 };
        var btnDel = new Button { Text = "Удалить", Width = 110 };

        btnAdd.Click += (_, __) => AddSupplier();
        btnEdit.Click += (_, __) => EditSupplier();
        btnDel.Click += (_, __) => DeleteSupplier();

        top.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDel });

        page.Controls.Add(_gridSuppliers);
        page.Controls.Add(top);
        return page;
    }

    private TabPage BuildFilmsTab()
    {
        var page = new TabPage("Фильмы");

        var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(8) };
        var btnAdd = new Button { Text = "Добавить", Width = 110 };
        var btnEdit = new Button { Text = "Изменить", Width = 110 };
        var btnDel = new Button { Text = "Удалить", Width = 110 };

        btnAdd.Click += (_, __) => AddFilm();
        btnEdit.Click += (_, __) => EditFilm();
        btnDel.Click += (_, __) => DeleteFilm();

        top.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDel });

        page.Controls.Add(_gridFilms);
        page.Controls.Add(top);
        return page;
    }

    private TabPage BuildRentalsTab()
    {
        var page = new TabPage("Аренды");

        var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(8) };
        var btnCreate = new Button { Text = "Оформить", Width = 110 };
        var btnClose = new Button { Text = "Закрыть", Width = 110 };
        var btnDel = new Button { Text = "Удалить", Width = 110 };

        btnCreate.Click += (_, __) => CreateRental();
        btnClose.Click += (_, __) => CloseRental();
        btnDel.Click += (_, __) => DeleteRental();

        top.Controls.AddRange(new Control[]
        {
            btnCreate, btnClose, btnDel,
            new Label { Width = 16 },
            _cbActiveOnly, _cbOverdueOnly
        });

        page.Controls.Add(_gridRentals);
        page.Controls.Add(top);
        return page;
    }

    private StatusStrip BuildStatusBar()
    {
        var status = new StatusStrip();
        _statusPath.Text = $"JSON: {_store.FilePath}";
        status.Items.Add(_statusPath);
        return status;
    }

    private void RefreshAll()
    {
        RefreshCinemas();
        RefreshSuppliers();
        RefreshFilms();
        RefreshRentals();
    }

    private void RefreshCinemas()
    {
        var rows = _system.Cinemas
            .Select(c => new CinemaRow
            {
                Id = c.Id,
                Name = c.Name,
                Phone = c.Phone,
                Address = c.Address.ToDisplayString(),
                SeatsCount = c.SeatsCount,
                DirectorFullName = c.DirectorFullName,
                OwnerFullName = c.OwnerFullName,
                Inn = c.LegalInfo.Inn,
                Bank = c.BankDetails.BankName
            })
            .OrderBy(r => r.Id)
            .ToList();

        _cinemasSource.DataSource = rows;
    }

    private void RefreshSuppliers()
    {
        var rows = _system.Suppliers
            .Select(s => new SupplierRow
            {
                Id = s.Id,
                Name = s.Name,
                Phone = s.Phone,
                Inn = s.LegalInfo.Inn,
                LegalAddress = s.LegalInfo.LegalAddress,
                Bank = s.BankDetails.BankName,
                AccountNumber = s.BankDetails.AccountNumber
            })
            .OrderBy(r => r.Id)
            .ToList();

        _suppliersSource.DataSource = rows;
    }

    private void RefreshFilms()
    {
        var bySupplier = _system.Suppliers.ToDictionary(s => s.Id, s => s.Name);

        var rows = _system.Films
            .Select(f => new FilmRow
            {
                Id = f.Id,
                Title = f.Title,
                Category = f.Category,
                ReleaseYear = f.ReleaseYear,
                Director = f.Director,
                ScriptAuthor = f.ScriptAuthor,
                ProducerCompany = f.ProducerCompany,
                PurchaseCost = f.PurchaseCost,
                SupplierId = f.SupplierId,
                SupplierName = bySupplier.TryGetValue(f.SupplierId, out var name) ? name : "(не найден)"
            })
            .OrderBy(r => r.Id)
            .ToList();

        _filmsSource.DataSource = rows;
    }

    private void RefreshRentals()
    {
        var today = DateTime.Today;

        IEnumerable<Rental> rentals = _system.Rentals;

        if (_cbActiveOnly.Checked)
            rentals = rentals.Where(r => r.IsActive);

        if (_cbOverdueOnly.Checked)
            rentals = rentals.Where(r => r.IsOverdue(today));

        var rows = rentals
            .Select(r => new RentalRow
            {
                Id = r.Id,
                CinemaId = r.CinemaId,
                Cinema = r.Cinema?.Name ?? _system.GetCinemaById(r.CinemaId)?.Name ?? "(не найден)",
                FilmId = r.FilmId,
                Film = r.Film?.Title ?? _system.GetFilmById(r.FilmId)?.Title ?? "(не найден)",
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                ActualReturnDate = r.ActualReturnDate,
                PaymentAmount = r.PaymentAmount,
                PenaltyRatePerDay = r.PenaltyRatePerDay,
                PenaltyAmount = r.CalculatePenalty(),
                IsActive = r.IsActive,
                IsOverdueToday = r.IsOverdue(today)
            })
            .OrderByDescending(r => r.IsActive)
            .ThenByDescending(r => r.IsOverdueToday)
            .ThenBy(r => r.Id)
            .ToList();

        _rentalsSource.DataSource = rows;
    }

    private int? GetSelectedId(BindingSource source)
    {
        if (source.Current is null) return null;
        var prop = source.Current.GetType().GetProperty("Id");
        if (prop?.GetValue(source.Current) is int id) return id;
        return null;
    }

    private void AddCinema()
    {
        try
        {
            var cinema = new Cinema();
            using var dlg = new CinemaEditDialog(cinema, isNew: true);
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            _system.AddCinema(cinema);
            Save();
            RefreshAll();
        }
        catch (Exception ex) { UiHelpers.ShowError(this, ex); }
    }

    private void EditCinema()
    {
        try
        {
            var id = GetSelectedId(_cinemasSource);
            if (id is null) return;

            var cinema = _system.GetCinemaById(id.Value);
            if (cinema is null) return;

            using var dlg = new CinemaEditDialog(cinema, isNew: false);
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            Save();
            RefreshAll();
        }
        catch (Exception ex) { UiHelpers.ShowError(this, ex); }
    }

    private void DeleteCinema()
    {
        try
        {
            var id = GetSelectedId(_cinemasSource);
            if (id is null) return;

            if (MessageBox.Show(this, "Удалить выбранный кинотеатр?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            _system.RemoveCinema(id.Value);
            Save();
            RefreshAll();
        }
        catch (Exception ex) { UiHelpers.ShowError(this, ex); }
    }

    private void AddSupplier()
    {
        try
        {
            var supplier = new Supplier();
            using var dlg = new SupplierEditDialog(supplier, isNew: true);
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            _system.AddSupplier(supplier);
            Save();
            RefreshAll();
        }
        catch (Exception ex) { UiHelpers.ShowError(this, ex); }
    }

    private void EditSupplier()
    {
        try
        {
            var id = GetSelectedId(_suppliersSource);
            if (id is null) return;

            var supplier = _system.GetSupplierById(id.Value);
            if (supplier is null) return;

            using var dlg = new SupplierEditDialog(supplier, isNew: false);
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            Save();
            RefreshAll();
        }
        catch (Exception ex) { UiHelpers.ShowError(this, ex); }
    }

    private void DeleteSupplier()
    {
        try
        {
            var id = GetSelectedId(_suppliersSource);
            if (id is null) return;

            if (MessageBox.Show(this, "Удалить выбранного поставщика?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            _system.RemoveSupplier(id.Value);
            Save();
            RefreshAll();
        }
        catch (Exception ex) { UiHelpers.ShowError(this, ex); }
    }

    private void AddFilm()
    {
        try
        {
            var film = new Film();
            using var dlg = new FilmEditDialog(film, _system.Suppliers, isNew: true);
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            _system.AddFilm(film, dlg.SelectedSupplierId);
            Save();
            RefreshAll();
        }
        catch (Exception ex) { UiHelpers.ShowError(this, ex); }
    }

    private void EditFilm()
    {
        try
        {
            var id = GetSelectedId(_filmsSource);
            if (id is null) return;

            var film = _system.GetFilmById(id.Value);
            if (film is null) return;

            using var dlg = new FilmEditDialog(film, _system.Suppliers, isNew: false);
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            film.SupplierId = dlg.SelectedSupplierId;
            Save();
            RefreshAll();
        }
        catch (Exception ex) { UiHelpers.ShowError(this, ex); }
    }

    private void DeleteFilm()
    {
        try
        {
            var id = GetSelectedId(_filmsSource);
            if (id is null) return;

            if (MessageBox.Show(this, "Удалить выбранный фильм?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            _system.RemoveFilm(id.Value);
            Save();
            RefreshAll();
        }
        catch (Exception ex) { UiHelpers.ShowError(this, ex); }
    }

    private void CreateRental()
    {
        try
        {
            using var dlg = new RentalCreateDialog(_system.Cinemas, _system.Films);
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            var rental = _system.CreateRental(dlg.CinemaId, dlg.FilmId, dlg.StartDate, dlg.EndDate, dlg.PaymentAmount);
            rental.PenaltyRatePerDay = dlg.PenaltyRatePerDay;

            Save();
            RefreshAll();
        }
        catch (Exception ex) { UiHelpers.ShowError(this, ex); }
    }

    private void CloseRental()
    {
        try
        {
            var id = GetSelectedId(_rentalsSource);
            if (id is null) return;

            var rental = _system.GetRentalById(id.Value);
            if (rental is null) return;

            if (!rental.IsActive)
            {
                UiHelpers.ShowInfo(this, "Аренда уже закрыта.");
                return;
            }

            using var dlg = new RentalCloseDialog(rental);
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            _system.CloseRental(rental.Id, dlg.ReturnDate);
            Save();
            RefreshAll();
        }
        catch (Exception ex) { UiHelpers.ShowError(this, ex); }
    }

    private void DeleteRental()
    {
        try
        {
            var id = GetSelectedId(_rentalsSource);
            if (id is null) return;

            if (MessageBox.Show(this, "Удалить выбранную аренду?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            _system.RemoveRental(id.Value);
            Save();
            RefreshAll();
        }
        catch (Exception ex) { UiHelpers.ShowError(this, ex); }
    }

    private void Save()
    {
        _store.SaveFrom(_system);
        _statusPath.Text = $"JSON: {_store.FilePath} (сохранено {DateTime.Now:HH:mm:ss})";
    }

    private void Reload()
    {
        _store.LoadInto(_system);
        RefreshAll();
        _statusPath.Text = $"JSON: {_store.FilePath} (перезагружено {DateTime.Now:HH:mm:ss})";
    }
}
