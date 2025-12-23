using System;
using System.IO;
using System.Text.Json;
using CinemaRentalCourseworkApp.Services;

namespace CinemaRentalCourseworkApp.Persistence;

public sealed class JsonDataStore
{
    private readonly string _filePath;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public JsonDataStore(string filePath)
    {
        _filePath = filePath;
    }

    public string FilePath => _filePath;

    /// <summary>
    /// Путь по умолчанию: <корень проекта>/data/data.json
    /// (корень проекта определяется по наличию *.sln или *.csproj).
    /// </summary>
    public static string DefaultPath()
    {
        string projectRoot = FindProjectRoot() ?? AppContext.BaseDirectory;

        string dir = Path.Combine(projectRoot, "data");
        Directory.CreateDirectory(dir);

        return Path.Combine(dir, "data.json");
    }

    public RentalSystemState Load()
    {
        if (!File.Exists(_filePath))
            return new RentalSystemState();

        var json = File.ReadAllText(_filePath);
        if (string.IsNullOrWhiteSpace(json))
            return new RentalSystemState();

        var state = JsonSerializer.Deserialize<RentalSystemState>(json, _jsonOptions);
        return state ?? new RentalSystemState();
    }

    public void Save(RentalSystemState state)
    {
        var json = JsonSerializer.Serialize(state, _jsonOptions);

        var dir = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(dir))
            Directory.CreateDirectory(dir);

        File.WriteAllText(_filePath, json);
    }

    public void LoadInto(RentalSystem system)
    {
        var state = Load();
        system.ReplaceAll(state.Cinemas, state.Suppliers, state.Films, state.Rentals);
    }

    public void SaveFrom(RentalSystem system)
    {
        var state = new RentalSystemState
        {
            Cinemas = system.Cinemas.ToList(),
            Suppliers = system.Suppliers.ToList(),
            Films = system.Films.ToList(),
            Rentals = system.Rentals.ToList()
        };
        Save(state);
    }

    private static string? FindProjectRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);

        for (int i = 0; i < 12 && dir != null; i++)
        {
            if (dir.GetFiles("*.sln").Length > 0 || dir.GetFiles("*.csproj").Length > 0)
                return dir.FullName;

            dir = dir.Parent;
        }

        return null;
    }
}
