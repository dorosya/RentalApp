using CinemaRentalCourseworkApp.Models;

namespace CinemaRentalCourseworkApp.UI.Dialogs;

public sealed class CinemaEditDialog : Form
{
    private readonly TextBox _tbName = new() { Dock = DockStyle.Fill };
    private readonly TextBox _tbPhone = new() { Dock = DockStyle.Fill };

    private readonly TextBox _tbRegion = new() { Dock = DockStyle.Fill };
    private readonly TextBox _tbCity = new() { Dock = DockStyle.Fill };
    private readonly TextBox _tbStreet = new() { Dock = DockStyle.Fill };
    private readonly TextBox _tbHouse = new() { Dock = DockStyle.Fill };

    private readonly TextBox _tbSeats = new() { Dock = DockStyle.Fill };
    private readonly TextBox _tbDirector = new() { Dock = DockStyle.Fill };
    private readonly TextBox _tbOwner = new() { Dock = DockStyle.Fill };

    private readonly TextBox _tbInn = new() { Dock = DockStyle.Fill };
    private readonly TextBox _tbLegalAddress = new() { Dock = DockStyle.Fill };
    private readonly TextBox _tbBank = new() { Dock = DockStyle.Fill };
    private readonly TextBox _tbAccount = new() { Dock = DockStyle.Fill };

    public Cinema Cinema { get; }

    public CinemaEditDialog(Cinema cinema, bool isNew)
    {
        Cinema = cinema;

        Text = isNew ? "Добавить кинотеатр" : "Редактировать кинотеатр";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(640, 460);
        MaximizeBox = false;

        var table = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 13,
            Padding = new Padding(12),
            AutoScroll = true
        };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        int r = 0;
        AddRow(table, r++, "Название", _tbName);
        AddRow(table, r++, "Телефон", _tbPhone);
        AddRow(table, r++, "Регион", _tbRegion);
        AddRow(table, r++, "Город", _tbCity);
        AddRow(table, r++, "Улица", _tbStreet);
        AddRow(table, r++, "Дом", _tbHouse);
        AddRow(table, r++, "Кол-во мест", _tbSeats);
        AddRow(table, r++, "Директор (ФИО)", _tbDirector);
        AddRow(table, r++, "Владелец (ФИО)", _tbOwner);
        AddRow(table, r++, "ИНН", _tbInn);
        AddRow(table, r++, "Юр. адрес", _tbLegalAddress);
        AddRow(table, r++, "Банк", _tbBank);
        AddRow(table, r++, "Счёт", _tbAccount);

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(12) };
        var btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Width = 90 };
        var btnCancel = new Button { Text = "Отмена", DialogResult = DialogResult.Cancel, Width = 90 };
        buttons.Controls.Add(btnOk);
        buttons.Controls.Add(btnCancel);

        Controls.Add(table);
        Controls.Add(buttons);

        _tbName.Text = Cinema.Name;
        _tbPhone.Text = Cinema.Phone;

        _tbRegion.Text = Cinema.Address.Region;
        _tbCity.Text = Cinema.Address.City;
        _tbStreet.Text = Cinema.Address.Street;
        _tbHouse.Text = Cinema.Address.House;

        _tbSeats.Text = Cinema.SeatsCount.ToString();
        _tbDirector.Text = Cinema.DirectorFullName;
        _tbOwner.Text = Cinema.OwnerFullName;

        _tbInn.Text = Cinema.LegalInfo.Inn;
        _tbLegalAddress.Text = Cinema.LegalInfo.LegalAddress;
        _tbBank.Text = Cinema.BankDetails.BankName;
        _tbAccount.Text = Cinema.BankDetails.AccountNumber;

        AcceptButton = btnOk;
        CancelButton = btnCancel;

        FormClosing += (_, e) =>
        {
            if (DialogResult != DialogResult.OK) return;

            try
            {
                Apply();
                Cinema.UpdateInfo();
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
        Cinema.Name = (_tbName.Text ?? "").Trim();
        Cinema.Phone = (_tbPhone.Text ?? "").Trim();

        Cinema.Address.Region = (_tbRegion.Text ?? "").Trim();
        Cinema.Address.City = (_tbCity.Text ?? "").Trim();
        Cinema.Address.Street = (_tbStreet.Text ?? "").Trim();
        Cinema.Address.House = (_tbHouse.Text ?? "").Trim();

        Cinema.SeatsCount = UiHelpers.ParseInt((_tbSeats.Text ?? "").Trim(), "Кол-во мест");
        Cinema.DirectorFullName = (_tbDirector.Text ?? "").Trim();
        Cinema.OwnerFullName = (_tbOwner.Text ?? "").Trim();

        Cinema.LegalInfo.Inn = (_tbInn.Text ?? "").Trim();
        Cinema.LegalInfo.LegalAddress = (_tbLegalAddress.Text ?? "").Trim();
        Cinema.BankDetails.BankName = (_tbBank.Text ?? "").Trim();
        Cinema.BankDetails.AccountNumber = (_tbAccount.Text ?? "").Trim();

        if (string.IsNullOrWhiteSpace(Cinema.Name))
            throw new ArgumentException("Название кинотеатра обязательно.");
        if (Cinema.SeatsCount <= 0)
            throw new ArgumentException("Кол-во мест должно быть больше 0.");
        if (string.IsNullOrWhiteSpace(Cinema.LegalInfo.Inn))
            throw new ArgumentException("ИНН обязателен.");
    }
}
