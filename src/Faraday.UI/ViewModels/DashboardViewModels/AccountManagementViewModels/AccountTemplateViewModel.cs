using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.Input;
using Faraday.Application.Interfaces;
using Faraday.Domain.Entities;
using Faraday.UI.Models;

namespace Faraday.UI.ViewModels.DashboardViewModels.AccountManagementViewModels;

public partial class AccountTemplateViewModel : ViewModelBase, INavigationAware {
    // ======== //
    // Services //
    // ======== //
    private readonly IWindowService _windowService;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ICSVService _csvService;


    // ========= //
    // Variables //
    // ========= //
    private Guid _accountId;
    public ObservableCollection<TransactionDisplay> DisplayTransactions { get; set; } = [];
    private string? CSVFilePath;
    private ObservableCollection<object> _csvData = [];
    
    
    // ====== //
    // Events //
    // ====== //
    public event Action? TransactionsLoaded;


    // =========== //
    // Constructor //
    // =========== //
    public AccountTemplateViewModel(IWindowService windowService, ITransactionRepository transactionRepository, IAccountRepository accountRepository, ICSVService csvService) {
        // Dependency Injection
        _windowService = windowService;
        _transactionRepository = transactionRepository;
        _accountRepository = accountRepository;
        _csvService = csvService;
    }


    // ================ //
    // INavigationAware //
    // ================ //
    public void OnNavigatedTo(NavigationContext navigationContext) {
        _accountId = (Guid)(navigationContext.Parameters["AccountID"] ?? -1);
        if (_accountId.ToString() == (-1).ToString()) {
            Console.WriteLine($"AccountTemplateViewModel::OnNavigatedTo: Account ID {_accountId} invalid!");
            throw new ArgumentException("Account ID invalid!");
        }
        
        _ = LoadTransactions();
    }

    public void OnNavigatedFrom(NavigationContext navigationContext) { }

    public bool IsNavigationTarget(NavigationContext navigationContext) => true;
    

    // ========= //
    // Functions //
    // ========= //
    /// <summary>
    /// Gets account data from the database based on the accountID.
    /// </summary>
    /// <returns>Returns both the headers and data for the account's data</returns>
    private async Task LoadTransactions() {
        Account account = await _accountRepository.GetByIdAsync(_accountId) 
                          ?? throw new InvalidOperationException("Account not found");
        
        ObservableCollection<TransactionDisplay> newTransactions = new(
            account.GetTransactionRunningBalances(await _transactionRepository.GetByAccountIdAsync(account.Id))
                .Select(rb => new TransactionDisplay {
                    Date = rb.Transaction.Date,
                    Amount = rb.Transaction.Amount,
                    Description = rb.Transaction.Description,
                    Balance = rb.RunningBalance
                })
        );

        DisplayTransactions.Clear();
        foreach (TransactionDisplay transaction in newTransactions) {
            DisplayTransactions.Add(transaction);
        }
        
        TransactionsLoaded?.Invoke();
    }


    // ======== //
    // Commands //
    // ======== //
    /// <summary>
    /// Interprets user input to switch which account-specific button is pressed.
    /// </summary>
    /// <param name="command">A textual form of the command passed by the frontend.</param>
    [RelayCommand]
    private async Task AccountNavbarSelection(string command) {
        switch (command) {
            case "Add_Transactions":
                CSVFilePath = _windowService.ShowFilePicker();
                if (string.IsNullOrEmpty(CSVFilePath)) return;

                _csvData.Clear();
                string[] lines = await File.ReadAllLinesAsync(CSVFilePath);

                foreach (string line in lines.Skip(1)) {
                    List<string> columns = line.Split(',').Select(c => c.Trim('"')).ToList();
                    _csvData.Add(columns);
                }
                
                IEnumerable<Transaction> temp = _csvService.Parse<Transaction>(CSVFilePath, _accountId);
                foreach (Transaction transaction in temp) {
                    await _transactionRepository.CreateAsync(transaction);
                }
                
                break;
            default:
                Console.WriteLine($"AccountTemplateViewModel::AccountNavbarSelection: Unknown command: {command}");
                return;
        }
    }
}