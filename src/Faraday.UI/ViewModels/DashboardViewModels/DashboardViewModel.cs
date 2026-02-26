// src/Faraday.UI/ViewModels/DashboardViewModels/DashboardViewModel.cs

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Faraday.Application.Interfaces;
using Faraday.Domain.Entities;
using Faraday.Domain.Enums;

namespace Faraday.UI.ViewModels.DashboardViewModels;

public partial class DashboardViewModel : ViewModelBase, INavigationAware {
    // ============ //
    // Dependencies //
    // ============ //
    private readonly IRegionManager _regionManager;
    private readonly IDialogService _dialogService;
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IStockRepository _stockRepository;


    // ========= //
    // Variables //
    // ========= //
    [ObservableProperty] private Account? _currentAccount;
    [ObservableProperty] private ObservableCollection<Account> _activeAccounts = [];


    // =========== //
    // Constructor //
    // =========== //
    public DashboardViewModel(IRegionManager regionManager, IDialogService dialogService,
        IAccountRepository accountRepository,
        ITransactionRepository transactionRepository, IStockRepository stockRepository) {
        // Injecting Dependencies
        _regionManager = regionManager;
        _dialogService = dialogService;
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _stockRepository = stockRepository;

        // Load Accounts to Dashboard button list
        _ = LoadAccounts();
    }


    // ========= //
    // Functions //
    // ========= //
    /// <summary>
    /// Loads all accounts from the database and puts them in ActiveAccounts,
    /// which shows them in the button panel on the left side of the screen. 
    /// </summary>
    private async Task LoadAccounts() {
        // Wipes the current members of ActiveAccounts
        ActiveAccounts.Clear();

        // Iterates through each member of the accounts in the database and calculates the balance of them,
        // then adds them to the ActiveAccounts variable
        foreach (Account account in await _accountRepository.GetAllActiveAsync()) {
            if (!account.IsActive) continue;

            // Calculate balances for this account
            if (account.Institution is InstitutionType.Commerce) {
                _ = account.CalculateCurrentBalance(await _transactionRepository.GetByAccountIdAsync(account.Id));
            }

            if (account.Institution is InstitutionType.Fidelity) {
                _ = account.CalculateCurrentBalance(await _stockRepository.GetByAccountIdAsync(account.Id));
            }

            ActiveAccounts.Add(account);
        }
    }


    // ================ //
    // INavigationAware //
    // ================ //
    public void OnNavigatedTo(NavigationContext navigationContext) {
        _regionManager.RequestNavigate("WidgetAccountRegion", "WidgetView");
    }

    public bool IsNavigationTarget(NavigationContext navigationContext) {
        // True if you want to reuse the same instance when navigating again
        return true;
    }

    public void OnNavigatedFrom(NavigationContext navigationContext) { }


    // ======== //
    // Commands //
    // ======== //
    /// <summary>
    /// Refreshes(or initializes) the ActiveAccounts variable.
    /// </summary>
    [RelayCommand]
    private void AllAccountsButton() {
        _regionManager.RequestNavigate("WidgetAccountRegion", "WidgetView");
        _ = LoadAccounts();
    }

    /// <summary>
    /// Switches CurrentAccount from whatever to the account (name) that has been passed
    /// </summary>
    /// <param name="accountName">The name of the account to switch to.</param>
    [RelayCommand]
    private void SwitchCurrentAccount(string accountName) {
        Account account = _accountRepository.GetByNameAsync(accountName).Result ??
                          throw new InvalidOperationException("GetByNameAsync: Account not found");
        CurrentAccount = account;

        NavigationParameters parameters = new() {
            { "AccountID", CurrentAccount.Id }
        };

        _regionManager.RequestNavigate("WidgetAccountRegion", "AccountTemplateView", parameters);
    }

    /// <summary>
    /// Opens the AddAccountView from the MainWindow
    /// </summary>
    [RelayCommand]
    private void OpenAddAccountView() {
        _dialogService.ShowDialog("AddAccountView", null, result => {
            if (result.Result == ButtonResult.OK) {
                _ = LoadAccounts();
            }
        });
    }
}