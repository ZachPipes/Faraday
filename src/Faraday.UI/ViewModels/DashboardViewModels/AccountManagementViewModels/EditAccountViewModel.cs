using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Faraday.Application.Interfaces;
using Faraday.Domain.Entities;
using Faraday.Domain.Enums;
using Faraday.UI.Services;

namespace Faraday.UI.ViewModels.DashboardViewModels.AccountManagementViewModels;

public partial class EditAccountViewModel : ViewModelBase, IDialogAware {
    // ============ //
    // Dependencies //
    // ============ //
    private readonly IWindowService _windowService;
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IStockRepository _stockRepository;
    private readonly ICSVService _csvService;


    // ========= //
    // Variables //
    // ========= //
    [ObservableProperty] private BalanceInputMode _selectedMode = BalanceInputMode.CSV;
    [ObservableProperty] private Account? _editAccount;
    [ObservableProperty] private decimal _endingBalance;
    [ObservableProperty] private ObservableCollection<string> _currencyOptions;
    [ObservableProperty] private ObservableCollection<string> _institutionOptions;
    [ObservableProperty] private ObservableCollection<string> _typeOptions;

    [ObservableProperty] private string _enteredName;
    [ObservableProperty] private DateTime _selectedDate = DateTime.Today;
    [ObservableProperty] private string _selectedCurrency;
    [ObservableProperty] private string _selectedInstitution;
    [ObservableProperty] private string _selectedAccountType;

    // CSV Data
    private string? CSVFilePath { get; set; }
    [ObservableProperty] private ObservableCollection<object> _csvData = [];
    [ObservableProperty] private ObservableCollection<string> _previewColumns = [];


    // =========== //
    // Constructor //
    // =========== //
    public EditAccountViewModel(IWindowService windowService, IAccountRepository accountRepository,
        ITransactionRepository transactionRepository, IStockRepository stockRepository, ICSVService csvService) {
        // Dependency Injection //
        _windowService = windowService;
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _stockRepository = stockRepository;
        _csvService = csvService;

        // Variables //
        CurrencyOptions = new ObservableCollection<string>(AppState.Instance.Settings.Currencies);
        InstitutionOptions = new ObservableCollection<string>(AppState.Instance.Settings.Institutions);
        TypeOptions = new ObservableCollection<string>(AppState.Instance.Settings.AccountTypes);
        EnteredName = "";
    }


    // ============ //
    // IDialogAware //
    // ============ //
    public bool CanCloseDialog() => true;
    public void OnDialogClosed() { }

    public void OnDialogOpened(IDialogParameters parameters) {
        try {
            Account account = parameters.GetValue<Account>("SelectedAccount")
                              ?? throw new ArgumentNullException(nameof(parameters),
                                  "SelectedAccount was not passed to the dialog.");

            EditAccount = account;
            EnteredName = account.Name;
            SelectedCurrency = CurrencyOptions.FirstOrDefault(x => 
                string.Equals(x, account.Currency.ToString(), StringComparison.OrdinalIgnoreCase)) ?? throw new InvalidOperationException();
            SelectedInstitution = InstitutionOptions.FirstOrDefault(x => 
                string.Equals(x, account.Institution.ToString(), StringComparison.OrdinalIgnoreCase)) ?? throw new InvalidOperationException();
            SelectedAccountType = TypeOptions.FirstOrDefault(x => 
                string.Equals(x, account.Type.ToString(), StringComparison.OrdinalIgnoreCase)) ?? throw new InvalidOperationException();
            
            // Start the async data loading without making this method 'async void'
            _ = LoadInitialDataAsync(account);
        }
        catch (Exception e) {
            _windowService.ShowMessage($"Error initializing dialog: {e.Message}");
        }
    }

    private async Task LoadInitialDataAsync(Account account) {
        try {
            if (SelectedInstitution == nameof(InstitutionType.Fidelity)) {
                IEnumerable<Stock> stocks = await _stockRepository.GetByAccountIdAsync(account.Id);
                CsvData = new ObservableCollection<object>(stocks);
            }
            else {
                IEnumerable<Transaction> txs = await _transactionRepository.GetByAccountIdAsync(account.Id);
                CsvData = new ObservableCollection<object>(txs);
            }
        }
        catch (Exception e) {
            _windowService.ShowMessage($"Error loading account data: {e.Message}");
        }
    }

    public DialogCloseListener RequestClose { get; private set; } = new();


    // ============== //
    // Dialog Options //
    // ============== //
    // This is used for the window title (Prism handles it)
    public static string Title => "Edit Account";


    // ======== //
    // Commands //
    // ======== //
    /// <summary>
    /// Starts the process of uploading a CSV file to the program.
    /// </summary>
    [RelayCommand]
    private async Task UploadCSVFile() {
        // Allows user to select a CSV file and checks if it is empty
        CSVFilePath = _windowService.ShowFilePicker();
        if (string.IsNullOrEmpty(CSVFilePath)) return;

        CsvData.Clear();
        // Basic CSV Parsing ___FOR DISPLAY ONLY___
        string[] lines = await File.ReadAllLinesAsync(CSVFilePath);

        // Skip header if needed
        foreach (string line in lines.Skip(1)) {
            List<string> columns = line.Split(',').Select(c => c.Trim('"')).ToList();

            CsvData.Add(new ObservableCollection<string>(columns));
        }
    }

    /// <summary>
    /// Checks for validity across the user's data input and creates the database entries for the account.
    /// </summary>
    /// <param name="parameter">The finish button's parent window</param>
    [RelayCommand]
    private async Task? FinishButton(object parameter) {
        if (EnteredName == "") {
            Console.WriteLine("EditAccountViewModel::FinishButton() - New Account Name is empty!");
            return;
        }

        Guid accountIdWithName = Guid.Empty;
        if (await _accountRepository.ExistsWithNameAsync(EnteredName)) {
            accountIdWithName = (await _accountRepository.GetByNameAsync(EnteredName)).Id;
        }
        
        if (await _accountRepository.ExistsWithNameAsync(EnteredName) && EditAccount.Id != accountIdWithName ) {
            Console.WriteLine("Account with that name already exists! Returning...");
            return;
        }

        // Creates the new account in the Accounts table
        await _accountRepository.UpdateAsync(EditAccount ?? throw new InvalidOperationException());
        
        // Getting account from Accounts table
        Account? account = await _accountRepository.GetByIdAsync(EditAccount.Id);
        if (account == null)
            throw new InvalidOperationException($"Account '{EditAccount.Name}' not found");
        Guid accountId = account.Id;

        // Each case takes in the data and sets CsvData to the data
        if (CSVFilePath == null) return;
        switch (SelectedInstitution) {
            case nameof(InstitutionType.Commerce):
                IAsyncEnumerable<Transaction> commerceData = _csvService.Parse<Transaction>(CSVFilePath, accountId);
                await foreach (Transaction transaction in commerceData) {
                    await _transactionRepository.CreateAsync(transaction);
                }

                break;
            case nameof(InstitutionType.Simmons):
                // TODO - Implement Simmons CSV reading
                // IEnumerable<Transaction> simmonsData = _csvService.Parse<Transaction>(CSVFilePath, accountId);
                // CsvData = new ObservableCollection<object>(simmonsData);
                Console.WriteLine("Simmons CSV reading not implemented");
                break;
            case nameof(InstitutionType.Fidelity):
                IAsyncEnumerable<Stock> fidelityData = _csvService.Parse<Stock>(CSVFilePath, accountId);

                await foreach (Stock stock in fidelityData) {
                    await _stockRepository.CreateAsync(stock);
                }

                break;
            case nameof(InstitutionType.Cash):
            default:
                throw new ArgumentOutOfRangeException(
                    $"Institution passed is not an implemented type: Instituion {SelectedInstitution}");
        }

        // Closing the window
        DialogResult result = new(ButtonResult.OK);
        RequestClose.Invoke(result);
    }
}