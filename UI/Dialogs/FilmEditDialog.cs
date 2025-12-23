using CinemaRentalCourseworkApp.Models;

namespace CinemaRentalCourseworkApp.UI.Dialogs;

public sealed class FilmEditDialog : Form
{
    private readonly TextBox _tbTitle = new() { Dock = DockStyle.Fill };
    private readonly TextBox _tbCategory = new() { Dock = DockStyle.Fill };
    private readonly TextBox _tbScriptAuthor = new() { Dock = DockStyle.Fill };
    private readonly TextBox _tbDirector = new() { Dock = DockStyle.Fill };
    private readonly TextBox _tbProducer = new() { Dock = DockStyle.Fill };
    private readonly TextBox _tbYear = new() { Dock = DockStyle.Fill };
    private readonly TextBox _tbCost = new() { Dock = DockStyle.Fill };
    private readonly ComboBox _cbSupplier = new() { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };

    public Film Film { get; }
    public int SelectedSupplierId { get; private set; }

    public FilmEditDialog(Film film, IReadOnlyList<Supplier> suppliers, bool isNew)
    {
        if (suppliers.Count == 0)
            throw new InvalidOperationException("Сначала добавьте хотя бы одного поставщика.");

        Film = film;

        Text = isNew ? "Добавить фильм" : "Редактировать фильм";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(640, 420);
        MaximizeBox = false;

        _cbSupplier.DataSource = suppliers.Select(s => new SupplierItem(s.Id, s.Name)).ToList();
        _cbSupplier.DisplayMember = nameof(SupplierItem.Name);
        _cbSupplier.ValueMember = nameof(SupplierItem.Id);

        var table = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            Padding = new Padding(12),
            AutoScroll = true
        };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        int r = 0;
        AddRow(table, r++, "Название", _tbTitle);
        AddRow(table, r++, "Категория", _tbCategory);
        AddRow(table, r++, "Автор сценария", _tbScriptAuthor);
        AddRow(table, r++, "Режиссёр", _tbDirector);
        AddRow(table, r++, "Компания-производитель", _tbProducer);
        AddRow(table, r++, "Год выхода", _tbYear);
        AddRow(table, r++, "Стоимость покупки", _tbCost);
        AddRow(table, r++, "Поставщик", _cbSupplier);

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(12) };
        var btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Width = 90 };
        var btnCancel = new Button { Text = "Отмена", DialogResult = DialogResult.Cancel, Width = 90 };
        buttons.Controls.Add(btnOk);
        buttons.Controls.Add(btnCancel);

        Controls.Add(table);
        Controls.Add(buttons);

        _tbTitle.Text = Film.Title;
        _tbCategory.Text = Film.Category;
        _tbScriptAuthor.Text = Film.ScriptAuthor;
        _tbDirector.Text = Film.Director;
        _tbProducer.Text = Film.ProducerCompany;
        _tbYear.Text = Film.ReleaseYear == 0 ? "" : Film.ReleaseYear.ToString();
        _tbCost.Text = Film.PurchaseCost == 0 ? "" : Film.PurchaseCost.ToString(System.Globalization.CultureInfo.InvariantCulture);

        // Select supplier
        var selected = suppliers.FirstOrDefault(s => s.Id == Film.SupplierId) ?? suppliers[0];
        _cbSupplier.SelectedValue = selected.Id;

        AcceptButton = btnOk;
        CancelButton = btnCancel;

        FormClosing += (_, e) =>
        {
            if (DialogResult != DialogResult.OK) return;

            try
            {
                Apply();
            }
            catch (Exception ex)
            {
                UiHelpers.ShowError(this, ex);
                e.Cancel = true;
            }
        };
    }

    private static void AddRow(TableLayoutPanel table, int row, string label, Control control)
    {
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        var lbl = new Label { Text = label, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
        table.Controls.Add(lbl, 0, row);
        table.Controls.Add(control, 1, row);
    }

    private void Apply()
    {
        Film.Title = (_tbTitle.Text ?? "").Trim();
        Film.Category = (_tbCategory.Text ?? "").Trim();
        Film.ScriptAuthor = (_tbScriptAuthor.Text ?? "").Trim();
        Film.Director = (_tbDirector.Text ?? "").Trim();
        Film.ProducerCompany = (_tbProducer.Text ?? "").Trim();
        Film.ReleaseYear = UiHelpers.ParseInt((_tbYear.Text ?? "").Trim(), "Год выхода");
        Film.ChangePrice(UiHelpers.ParseDouble((_tbCost.Text ?? "").Trim(), "Стоимость покупки"));

        SelectedSupplierId = (int)(_cbSupplier.SelectedValue ?? 0);

        if (string.IsNullOrWhiteSpace(Film.Title))
            throw new ArgumentException("Название фильма обязательно.");
        var currentYear = DateTime.Now.Year + 1;
        if (Film.ReleaseYear < 1888 || Film.ReleaseYear > currentYear)
            throw new ArgumentException($"Год выхода должен быть в диапазоне 1888..{currentYear}.");
    }

    private sealed record SupplierItem(int Id, string Name);
}
