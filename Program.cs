using System;
using System.Windows.Forms;
using CinemaRentalCourseworkApp.UI;

namespace CinemaRentalCourseworkApp;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
