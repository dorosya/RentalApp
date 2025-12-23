using CinemaRentalCourseworkApp.Models;

namespace CinemaRentalCourseworkApp.UI.Dialogs;

public sealed class SupplierEditDialog : Form
{
    private readonly TextBox _tbName = new() { Dock = DockStyle.Fill };
    private readonly TextBox _tbPhone = new() { Dock = DockStyle.Fill };
    private readonly TextBox _tbInn = new() { Dock = DockStyle.Fill };
    private readonly TextBox _tbLegalAddress = new() { Dock = DockStyle.Fill };
    private readonly TextBox _tbBank = new() { Dock = DockStyle.Fill };
    private readonly TextBox _tbAccount = new() { Dock = DockStyle.Fill };

    public Supplier Supplier { get; }

    public SupplierEditDialog(Supplier supplier, bool isNew)
    {
        Supplier = supplier;

        Text = isNew ? "Добавить поставщика" : "Редактировать поставщика";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(520, 320);
        MaximizeBox = false;

        var table = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 7,
            Padding = new Padding(12),
            AutoSize = true
        };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        AddRow(table, 0, "Название", _tbName);
        AddRow(table, 1, "Телефон", _tbPhone);
        AddRow(table, 2, "ИНН", _tbInn);
        AddRow(table, 3, "Юр. адрес", _tbLegalAddress);
        AddRow(table, 4, "Банк", _tbBank);
        AddRow(table, 5, "Счёт", _tbAccount);

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, AutoSize = true };
        var btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Width = 90 };
        var btnCancel = new Button { Text = "Отмена", DialogResult = DialogResult.Cancel, Width = 90 };
        buttons.Controls.Add(btnOk);
        buttons.Controls.Add(btnCancel);
        table.Controls.Add(buttons, 0, 6);
        table.SetColumnSpan(buttons, 2);

        Controls.Add(table);

        _tbName.Text = Supplier.Name;
        _tbPhone.Text = Supplier.Phone;
        _tbInn.Text = Supplier.LegalInfo.Inn;
        _tbLegalAddress.Text = Supplier.LegalInfo.LegalAddress;
        _tbBank.Text = Supplier.BankDetails.BankName;
        _tbAccount.Text = Supplier.BankDetails.AccountNumber;

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
        Supplier.Name = (_tbName.Text ?? "").Trim();
        Supplier.Phone = (_tbPhone.Text ?? "").Trim();
        Supplier.LegalInfo.Inn = (_tbInn.Text ?? "").Trim();
        Supplier.LegalInfo.LegalAddress = (_tbLegalAddress.Text ?? "").Trim();
        Supplier.BankDetails.BankName = (_tbBank.Text ?? "").Trim();
        Supplier.BankDetails.AccountNumber = (_tbAccount.Text ?? "").Trim();

        if (string.IsNullOrWhiteSpace(Supplier.Name))
            throw new ArgumentException("Название поставщика обязательно.");
        if (string.IsNullOrWhiteSpace(Supplier.LegalInfo.Inn))
            throw new ArgumentException("ИНН обязателен.");
    }
}
