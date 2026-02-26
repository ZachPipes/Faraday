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
    private readonly IDialogService _dialogService;


    // ========= //
    // Variables //
    // ========= //
    private Guid _accountId;

    public ObservableCollection<TransactionDisplay> DisplayTransactions { get; } = [];
    private string? _csvFilePath;
    private Account _selectedAccount = null!;


    // ====== //
    // Events //
    // ====== //
    public event Action? TransactionsLoaded;


    // =========== //
    // Constructor //
    // =========== //
    public AccountTemplateViewModel(IWindowService windowService, ITransactionRepository transactionRepository,
        IAccountRepository accountRepository, IStockRepository stockRepository, ICSVService csvService, IDialogService dialogService) {
        // Dependency Injection
        _windowService = windowService;
        _transactionRepository = transactionRepository;
        _accountRepository = accountRepository;
        _stockRepository = stockRepository;
        _csvService = csvService;
        _dialogService = dialogService;
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

        switch (_selectedAccount.Institution) {
            case InstitutionType.Commerce:
                transactionDisplays = _selectedAccount.GetRunningBalances(
                        await _transactionRepository.GetByAccountIdAsync(_selectedAccount.Id))
                    .Select(rb => new TransactionDisplay {
                        Date = rb.transaction.Date,
                        Amount = rb.transaction.Amount,
                        Description = rb.transaction.Description,
                        Balance = rb.RunningBalance
                    });
                break;
            
            case InstitutionType.Fidelity:
                transactionDisplays = _selectedAccount.GetRunningBalances(
                        await _stockRepository.GetByAccountIdAsync(_selectedAccount.Id))
                    .Select(rb => new TransactionDisplay {
                        Date = rb.stock.RunDate,
                        Amount = rb.stock.Amount,
                        Description = rb.stock.Description,
                        Balance = rb.RunningBalance
                    });
                break;
            
            case InstitutionType.Simmons:
            case InstitutionType.Cash:
            default:
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
                DialogParameters parameters = new() { { "SelectedAccount", _selectedAccount } };
                _dialogService.ShowDialog("EditAccountView", parameters, result => {
                    if (result.Result == ButtonResult.OK) {
                        _ = LoadTransactions();
                    }
                });
                
                // _csvFilePath = _windowService.ShowFilePicker();
                // if (string.IsNullOrEmpty(_csvFilePath)) return;
                //
                try {
                    //     switch (_selectedAccount.Institution) {
                    //         case InstitutionType.Commerce:
                    //             IEnumerable<Transaction> commerceData =
                    //                 _csvService.Parse<Transaction>(_csvFilePath, _accountId);
                    //             IEnumerable<Transaction> transactions =
                    //                 commerceData as Transaction[] ?? commerceData.ToArray();
                    //             foreach (Transaction transaction in transactions) {
                    //                 await _transactionRepository.CreateAsync(transaction);
                    //             }
                    //
                    //             break;
                    //
                    //         case InstitutionType.Fidelity:
                    //             IEnumerable<Stock> fidelityData = _csvService.Parse<Stock>(_csvFilePath, _accountId);
                    //             IEnumerable<Stock> stocks = fidelityData as Stock[] ?? fidelityData.ToArray();
                    //             foreach (Stock stock in stocks) {
                    //                 await _stockRepository.CreateAsync(stock);
                    //             }
                    //
                    //             break;
                    //
                    //         case InstitutionType.Cash:
                    //         case InstitutionType.Simmons:
                    //         default:
                    //             throw new ArgumentOutOfRangeException(
                    //                 $"Institution passed is not an implemented type: Instituion {_selectedAccount.Institution}");
                    // }
                    
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