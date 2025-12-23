using CinemaRentalCourseworkApp.Models;

namespace CinemaRentalCourseworkApp.UI.Dialogs;

public sealed class RentalCloseDialog : Form
{
    private readonly DateTimePicker _dpReturn = new() { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short };
    private readonly Label _lblInfo = new() { Dock = DockStyle.Fill, AutoSize = true };
    private readonly Label _lblPenalty = new() { Dock = DockStyle.Fill, AutoSize = true };

    public DateTime ReturnDate { get; private set; }

    public RentalCloseDialog(Rental rental)
    {
        if (!rental.IsActive)
            throw new InvalidOperationException("Аренда уже закрыта.");

        Text = "Закрыть аренду";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(520, 260);
        MaximizeBox = false;

        _dpReturn.Value = DateTime.Today;

        var film = rental.Film?.Title ?? $"FilmId={rental.FilmId}";
        var cinema = rental.Cinema?.Name ?? $"CinemaId={rental.CinemaId}";
        _lblInfo.Text = $@"{cinema} — {film}
Срок: {rental.StartDate:dd.MM.yyyy} — {rental.EndDate:dd.MM.yyyy}
Оплата: {rental.PaymentAmount:0.##}
Пени/день: {rental.PenaltyRatePerDay:0.##}";

        UpdatePenaltyPreview(rental);

        _dpReturn.ValueChanged += (_, __) => UpdatePenaltyPreview(rental);

        var table = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            Padding = new Padding(12),
            AutoSize = true
        };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        table.Controls.Add(_lblInfo, 0, 0);
        table.SetColumnSpan(_lblInfo, 2);

        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        table.Controls.Add(new Label { Text = "Дата возврата", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
        table.Controls.Add(_dpReturn, 1, 1);

        table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        table.Controls.Add(_lblPenalty, 0, 2);
        table.SetColumnSpan(_lblPenalty, 2);

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, AutoSize = true };
        var btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Width = 90 };
        var btnCancel = new Button { Text = "Отмена", DialogResult = DialogResult.Cancel, Width = 90 };
        buttons.Controls.Add(btnOk);
        buttons.Controls.Add(btnCancel);

        table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        table.Controls.Add(buttons, 0, 3);
        table.SetColumnSpan(buttons, 2);

        Controls.Add(table);

        AcceptButton = btnOk;
        CancelButton = btnCancel;

        FormClosing += (_, e) =>
        {
            if (DialogResult != DialogResult.OK) return;

            try
            {
                ReturnDate = _dpReturn.Value.Date;
                if (ReturnDate < rental.StartDate.Date)
                    throw new ArgumentException("Дата возврата не может быть раньше даты начала аренды.");
            }
            catch (Exception ex)
            {
                UiHelpers.ShowError(this, ex);
                e.Cancel = true;
            }
        };
    }

    private void UpdatePenaltyPreview(Rental rental)
    {
        var preview = new Rental
        {
            StartDate = rental.StartDate,
            EndDate = rental.EndDate,
            PaymentAmount = rental.PaymentAmount,
            PenaltyRatePerDay = rental.PenaltyRatePerDay,
            ActualReturnDate = _dpReturn.Value.Date
        };

        _lblPenalty.Text = $@"Предварительный расчёт пени: {preview.CalculatePenalty():0.##}";
    }
}
