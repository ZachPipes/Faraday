namespace Faraday.Application.Interfaces;

public interface IAppState {
    string AppFolderPath { get; }
    AppSettings Settings { get; }
    Defaults AccountDefaults { get; }
}

public class AppSettings {
    public string Version { get; init; } = "";
    public string Theme { get; init; } = "";
    public string[] AccountTypes { get; init; } = [];
    public string[] CategoryTypes { get; init; } = [];
    public string[] Institutions { get; init; } = [];
    public string[] Currencies { get; init; } = [];
}

public class Defaults {
    public Domain.Enums.AccountType AccountType = Domain.Enums.AccountType.Checking;
    public Domain.Enums.InstitutionType Institution = Domain.Enums.InstitutionType.Cash;
    public Domain.Enums.CurrencyType Currency = Domain.Enums.CurrencyType.Usd;
}