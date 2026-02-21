// src/Faraday.UI/Services/AppState.cs

using System.IO;
using System.Text.Json;
using Faraday.Application.Interfaces;
using Faraday.Domain.Entities;

namespace Faraday.UI.Services;

public class AppState : IAppState {
    private static readonly Lazy<AppState> _instance = new(() => new AppState());
    public static AppState Instance => _instance.Value;

    private const string AppFolderName = "Faraday";
    public Account? CurrentAccount;
    public string AppFolderPath { get; private set; }
    public string DbFolderPath { get; private set; }
    public AppSettings Settings { get; private set; }
    public Defaults AccountDefaults { get; private set; } = new();

    private AppState() {
        AppFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            AppFolderName);
        DbFolderPath = Path.Combine(AppFolderPath, "faraday.sqlite");
        Directory.CreateDirectory(AppFolderPath);
        
        string settingsFile = Path.Combine(AppFolderPath, "Settings.json");

        if (!File.Exists(settingsFile)) {
            Console.WriteLine("Settings.json not found, creating default.");
            CreateDefaultSettings(settingsFile);
        }

        string json = File.ReadAllText(settingsFile);
        Settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
    }

    private static void CreateDefaultSettings(string settingsPath) {
        AppSettings defaultSettings = new() {
            Version = "0.0.1",
            Theme = "Light",
            AccountTypes = ["Checking", "Savings", "Credit", "Brokerage"],
            CategoryTypes = ["Income", "Expense", "Transfer"],
            Institutions = ["Cash", "Simmons", "Fidelity", "Commerce"],
            Currencies = ["USD", "CAN", "EUR"]
        };

        string json = JsonSerializer.Serialize(defaultSettings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(settingsPath, json);
    }
}