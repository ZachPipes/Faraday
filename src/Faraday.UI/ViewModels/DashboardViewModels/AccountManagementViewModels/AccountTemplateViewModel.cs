using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.Input;
using Faraday.Application.Interfaces;
using Faraday.Domain.Entities;
using Faraday.Domain.Enums;
using Faraday.Domain.Exceptions;
using Faraday.UI.Models;

namespace Faraday.UI.ViewModels.DashboardViewModels.AccountManagementViewModels;

public partial class AccountTemplateViewModel : ViewModelBase, INavigationAware {
    // ======== //
    // Services //
    // ======== //
    private readonly IWindowService _windowService;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IStockRepository _stockRepository;
    private readonly ICSVService _csvService;


    // ========= //
    // Variables //
    // ========= //
    private Guid _accountId;

    public ObservableCollection<TransactionDisplay> DisplayTransactions { get; set; } = [];
    private string? _csvFilePath;
    private ObservableCollection<object> _csvData = [];
    private Account _selectedAccount;


    // ====== //
    // Events //
    // ====== //
    public event Action? TransactionsLoaded;


    // =========== //
    // Constructor //
    // =========== //
    public AccountTemplateViewModel(IWindowService windowService, ITransactionRepository transactionRepository,
        IAccountRepository accountRepository, IStockRepository stockRepository, ICSVService csvService) {
        // Dependency Injection
        _windowService = windowService;
        _transactionRepository = transactionRepository;
        _accountRepository = accountRepository;
        _stockRepository = stockRepository;
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

        _ = FindAccount();
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
        IEnumerable<TransactionDisplay> transactionDisplays;

        if (_selectedAccount.Institution == InstitutionType.Commerce) {
            transactionDisplays = _selectedAccount.GetTransactionRunningBalances(
                    await _transactionRepository.GetByAccountIdAsync(_selectedAccount.Id))
                .Select(rb => new TransactionDisplay {
                    Date = rb.Transaction.Date,
                    Amount = rb.Transaction.Amount,
                    Description = rb.Transaction.Description,
                    Balance = rb.RunningBalance
                });
        }
        else if (_selectedAccount.Institution == InstitutionType.Fidelity) {
            transactionDisplays = _selectedAccount.GetStockRunningBalances(
                    await _stockRepository.GetByAccountIdAsync(_selectedAccount.Id))
                .Select(rb => new TransactionDisplay {
                    Date = rb.Transaction.RunDate,
                    Amount = rb.Transaction.Amount,
                    Description = rb.Transaction.Description,
                    Balance = rb.RunningBalance
                });
        }
        else {
            throw new InvalidAccountOperationException(_selectedAccount.Id, "Invalid account type tried to parse!");
        }


        DisplayTransactions.Clear();
        foreach (TransactionDisplay transaction in transactionDisplays) {
            DisplayTransactions.Add(transaction);
        }

        TransactionsLoaded?.Invoke();
    }

    /// <summary>
    /// Finds the account and assigns the _selectedAccount variable
    /// </summary>
    /// <exception cref="InvalidOperationException">If the account cannot be found</exception>
    private async Task FindAccount() {
        Account account = await _accountRepository.GetByIdAsync(_accountId)
                          ?? throw new InvalidOperationException("Account not found");

        _selectedAccount = account;
        _ = LoadTransactions();
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
                _csvFilePath = _windowService.ShowFilePicker();
                if (string.IsNullOrEmpty(_csvFilePath)) return;
                _csvData.Clear();

                try {
                    switch (_selectedAccount.Institution) {
                        case InstitutionType.Commerce:
                            IEnumerable<Transaction> commerceData =
                                _csvService.Parse<Transaction>(_csvFilePath, _accountId);
                            IEnumerable<Transaction> transactions =
                                commerceData as Transaction[] ?? commerceData.ToArray();
                            foreach (Transaction transaction in transactions) {
                                _csvData.Add(transaction);
                                await _transactionRepository.CreateAsync(transaction);
                            }

                            break;

                        case InstitutionType.Fidelity:
                            IEnumerable<Stock> fidelityData = _csvService.Parse<Stock>(_csvFilePath, _accountId);
                            IEnumerable<Stock> stocks = fidelityData as Stock[] ?? fidelityData.ToArray();
                            _csvData = new ObservableCollection<object>(stocks);
                            foreach (Stock stock in stocks) {
                                _csvData.Add(stock);
                                await _stockRepository.CreateAsync(stock);
                            }

                            break;

                        case InstitutionType.Cash:
                        case InstitutionType.Simmons:
                        default:
                            throw new ArgumentOutOfRangeException(
                                $"Institution passed is not an implemented type: Instituion {_selectedAccount.Institution}");
                    }
                    
                    _ = LoadTransactions();
                }
                catch (IOException ex) {
                    Console.WriteLine($"File access error: {ex.Message}");
                }
                catch (Exception ex) {
                    Console.WriteLine(ex);
                }

                break;
            default:
                Console.WriteLine($"AccountTemplateViewModel::AccountNavbarSelection: Unknown command: {command}");
                return;
        }
    }
}