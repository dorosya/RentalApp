using System.Globalization;

namespace CinemaRentalCourseworkApp.UI;

internal static class UiHelpers
{
    public static void ShowError(Form owner, Exception ex, string title = "Ошибка")
    {
        MessageBox.Show(owner, ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public static void ShowInfo(Form owner, string message, string title = "Информация")
    {
        MessageBox.Show(owner, message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    public static double ParseDouble(string text, string fieldName)
    {
        if (!double.TryParse(text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
            throw new ArgumentException($"Поле {fieldName} должно быть числом.");
        return value;
    }

    public static int ParseInt(string text, string fieldName)
    {
        if (!int.TryParse(text, out var value))
            throw new ArgumentException($"Поле {fieldName} должно быть числом.");
        return value;
    }
}
