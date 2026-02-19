using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Faraday.Application.Interfaces;
using Faraday.Domain.Entities;
using Faraday.Domain.Enums;
using Faraday.UI.Services;

namespace Faraday.UI.ViewModels.DashboardViewModels.AccountManagementViewModels;

public partial class AddAccountViewModel : ViewModelBase, IDialogAware {
    // ============ //
    // Dependencies //
    // ============ //
    private readonly IWindowService _windowService;
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICSVService _csvService;


    // ========= //
    // Variables //
    // ========= //
    [ObservableProperty] private BalanceInputMode _selectedMode = BalanceInputMode.CSV;
    [ObservableProperty] private Account? _newAccount;
    [ObservableProperty] private decimal _endingBalance;
    [ObservableProperty] private ObservableCollection<string> _currencyOptions;
    [ObservableProperty] private ObservableCollection<string> _institutionOptions;
    [ObservableProperty] private ObservableCollection<string> _typeOptions;

    [ObservableProperty] private string _enteredName;
    [ObservableProperty] private DateTime _selectedDate = DateTime.Today;
    [ObservableProperty] private CurrencyType _selectedCurrency;
    [ObservableProperty] private InstitutionType _selectedInstitution;
    [ObservableProperty] private AccountType _selectedAccountType;

    // CSV Data
    private string? CSVFilePath { get; set; }
    [ObservableProperty] private ObservableCollection<object> _csvData = [];
    [ObservableProperty] private ObservableCollection<string> _previewColumns = [];

    // ============== //
    // Dialog Options //
    // ============== //
    public static string Title => "Add New Account";

    
    // =========== //
    // Constructor //
    // =========== //
    public AddAccountViewModel(IWindowService windowService, IAccountRepository accountRepository,
        ITransactionRepository transactionRepository, ICSVService csvService) {
        // Dependency Injection //
        _windowService = windowService;
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _csvService = csvService;

        // Variables //
        _currencyOptions = new ObservableCollection<string>(AppState.Instance.Settings.Currencies);
        _institutionOptions = new ObservableCollection<string>(AppState.Instance.Settings.Institutions);
        _typeOptions = new ObservableCollection<string>(AppState.Instance.Settings.AccountTypes);
        _enteredName = "";
    }
    
    
    // ============ //
    // IDialogAware //
    // ============ //
    public bool CanCloseDialog() => true;
    public void OnDialogClosed() { }
    public void OnDialogOpened(IDialogParameters parameters) { }
    public DialogCloseListener RequestClose { get; private set; } = new();
    

    // ======== //
    // Commands //
    // ======== //
    /// <summary>
    /// Starts the process of uploading a CSV file to the program.
    /// </summary>
    [RelayCommand]
    private void UploadCSVFile() {
        // Allows user to select a CSV file and checks if it is empty
        CSVFilePath = _windowService.ShowFilePicker();
        if (string.IsNullOrEmpty(CSVFilePath)) return;

        CsvData.Clear();
        // Basic CSV Parsing ___FOR DISPLAY ONLY___
        string[] lines = File.ReadAllLines(CSVFilePath);

        // Skip header if needed
        foreach (string line in lines.Skip(1)) {
            List<string> columns = line.Split(',').Select(c => c.Trim('"')).ToList();

            CsvData.Add(new ObservableCollection<string>(columns));
        }

        // TODO DISPLAY ONLY, DO NOT MAKE ANY TRANSACTION OBJECTS
    }

    /// <summary>
    /// Checks for validity across the user's data input and creates the database entries for the account.
    /// </summary>
    /// <param name="parameter">The finish button's parent window</param>
    [RelayCommand]
    private async Task? FinishButton(object parameter) {
        NewAccount = new Account(
            EnteredName, SelectedAccountType, EndingBalance, SelectedCurrency, SelectedInstitution);

        if (NewAccount.Name == "") {
            Console.WriteLine("AddAccountViewModel::FinishButton() - New Account Name is empty!");
            return;
        }

        if (await _accountRepository.ExistsWithNameAsync(NewAccount.Name)) {
            Console.WriteLine("Account already exists! Returning...");
            return;
        }

        // Creates the new account in the Accounts table
        await _accountRepository.CreateAsync(NewAccount);

        // Getting account from Accounts table
        Account? account = await _accountRepository.GetByNameAsync(NewAccount.Name);
        if (account == null)
            throw new InvalidOperationException($"Account '{NewAccount.Name}' not found");
        Guid accountId = account.Id;

        // Each case takes in the data and sets CsvData to the data
        if (CSVFilePath == null) return;
        switch (SelectedInstitution) {
            case InstitutionType.Commerce:
                IEnumerable<Transaction> temp = _csvService.Parse<Transaction>(CSVFilePath, accountId);
                CsvData = new ObservableCollection<object>(temp);
                break;
            case InstitutionType.Simmons:
                //TODO - Implement Simmons CSV reading
                Console.WriteLine("Simmons CSV reading not implemented");
                break;
            case InstitutionType.Fidelity:
                //TODO - Implement Fidelity CSV reading
                Console.WriteLine("Fidelity CSV reading not implemented");
                break;
            case InstitutionType.Cash:
            default:
                throw new ArgumentOutOfRangeException(
                    $"Institution passed is not an implemented type: Instituion {SelectedInstitution}");
        }

        // Uploading transactions to the database
        foreach (Transaction transaction in CsvData) {
            await _transactionRepository.CreateAsync(transaction);
        }

        // Closing the window
        DialogResult result = new(ButtonResult.OK);
        RequestClose.Invoke(result);
    }
}