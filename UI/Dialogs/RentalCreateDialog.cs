using CinemaRentalCourseworkApp.Models;

namespace CinemaRentalCourseworkApp.UI.Dialogs;

public sealed class RentalCreateDialog : Form
{
    private readonly ComboBox _cbCinema = new() { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _cbFilm = new() { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly DateTimePicker _dpStart = new() { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short };
    private readonly DateTimePicker _dpEnd = new() { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short };
    private readonly TextBox _tbPayment = new() { Dock = DockStyle.Fill };
    private readonly TextBox _tbPenaltyRate = new() { Dock = DockStyle.Fill };

    public int CinemaId { get; private set; }
    public int FilmId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public double PaymentAmount { get; private set; }
    public double PenaltyRatePerDay { get; private set; }

    public RentalCreateDialog(IReadOnlyList<Cinema> cinemas, IReadOnlyList<Film> films)
    {
        if (cinemas.Count == 0) throw new InvalidOperationException("Сначала добавьте хотя бы один кинотеатр.");
        if (films.Count == 0) throw new InvalidOperationException("Сначала добавьте хотя бы один фильм.");

        Text = "Оформить аренду";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(520, 320);
        MaximizeBox = false;

        _cbCinema.DataSource = cinemas.Select(c => new Item(c.Id, c.Name)).ToList();
        _cbCinema.DisplayMember = nameof(Item.Name);
        _cbCinema.ValueMember = nameof(Item.Id);

        _cbFilm.DataSource = films.Select(f => new Item(f.Id, f.Title)).ToList();
        _cbFilm.DisplayMember = nameof(Item.Name);
        _cbFilm.ValueMember = nameof(Item.Id);

        _dpStart.Value = DateTime.Today;
        _dpEnd.Value = DateTime.Today.AddDays(7);
        _tbPayment.Text = "0";
        _tbPenaltyRate.Text = "0";

        var table = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            Padding = new Padding(12),
            AutoSize = true
        };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        int r = 0;
        AddRow(table, r++, "Кинотеатр", _cbCinema);
        AddRow(table, r++, "Фильм", _cbFilm);
        AddRow(table, r++, "Начало", _dpStart);
        AddRow(table, r++, "Окончание", _dpEnd);
        AddRow(table, r++, "Оплата", _tbPayment);
        AddRow(table, r++, "Пени за день", _tbPenaltyRate);

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, AutoSize = true };
        var btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Width = 90 };
        var btnCancel = new Button { Text = "Отмена", DialogResult = DialogResult.Cancel, Width = 90 };
        buttons.Controls.Add(btnOk);
        buttons.Controls.Add(btnCancel);
        table.Controls.Add(buttons, 0, r);
        table.SetColumnSpan(buttons, 2);

        Controls.Add(table);

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
        CinemaId = (int)(_cbCinema.SelectedValue ?? 0);
        FilmId = (int)(_cbFilm.SelectedValue ?? 0);
        StartDate = _dpStart.Value.Date;
        EndDate = _dpEnd.Value.Date;

        if (StartDate > EndDate)
            throw new ArgumentException("Дата начала не может быть позже даты окончания.");

        PaymentAmount = UiHelpers.ParseDouble((_tbPayment.Text ?? "").Trim(), "Оплата");
        PenaltyRatePerDay = UiHelpers.ParseDouble((_tbPenaltyRate.Text ?? "").Trim(), "Пени за день");

        if (PaymentAmount < 0) throw new ArgumentException("Оплата не может быть отрицательной.");
        if (PenaltyRatePerDay < 0) throw new ArgumentException("Пени за день не могут быть отрицательными.");
    }

    private sealed record Item(int Id, string Name);
}
