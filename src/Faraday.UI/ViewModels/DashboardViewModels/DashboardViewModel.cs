using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Faraday.Domain.Entities;

namespace Faraday.UI.ViewModels.DashboardViewModels;

public partial class DashboardViewModel : ViewModelBase, INavigationAware {
    // ============ //
    // Dependencies //
    // ============ //
    private readonly IRegionManager _regionManager;

    
    
    // ========= //
    // Variables //
    // ========= //
    [ObservableProperty] private string? _currentAccount;
    [ObservableProperty] private ObservableCollection<Account> _activeAccounts = [];

    
    
    // =========== //
    // Constructor //
    // =========== //
    public DashboardViewModel(IRegionManager regionManager) {
        // Injecting Dependencies
        _regionManager = regionManager;

        // Load Accounts to Dashboard button list
        LoadAccounts();
    }

    
    
    // ========= //
    // Functions //
    // ========= //
    /// <summary>
    /// Loads all accounts from the database and puts them in ActiveAccounts,
    /// which shows them in the button panel on the left side of the screen. 
    /// </summary>
    private void LoadAccounts() {
        // Wipes the current members of ActiveAccounts
        ActiveAccounts.Clear();

        // Iterates through each member of the accounts in the database and calculates the balance of them,
        // then adds them to the ActiveAccounts variable
        // foreach (Account account in Database.GetAllActiveAccounts()) {
        //     // Calculate balances for this account
        //     ObservableCollection<Transaction> transactions = Database.GetAllTransactionsOfAccount(account.ID);
        //     BasicUtilities.CalculateBalances(transactions);
        //
        //     // Optionally, set the latest balance on the account itself
        //     account.Balance = transactions.LastOrDefault()?.Balance ?? 0;
        //
        //     ActiveAccounts.Add(account);
        // }
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
    
    public void OnNavigatedFrom(NavigationContext navigationContext) {
        
    }

    
    
    // ======== //
    // Commands //
    // ======== //
    /// <summary>
    /// Refreshes(or initializes) the ActiveAccounts variable.
    /// </summary>
    [RelayCommand]
    private void AllAccountsButton() {
        _regionManager.RequestNavigate("WidgetAccountRegion", "WidgetView");
        LoadAccounts();
    }

    /// <summary>
    /// Switches CurrentAccount from whatever to the account (name) that has been passed
    /// </summary>
    /// <param name="account">The name of the account to switch to.</param>
    // TODO - Change the passed account name to the ID of the account
    [RelayCommand]
    private void SwitchCurrentAccount(string account) {
        // CurrentAccount = account;
        // int accountID = Database.GetAccountIDFromName(account);
        //
        // NavigationParameters parameters = new() {
        //     { "AccountID", accountID }
        // };
        //
        // _regionManager.RequestNavigate("WidgetAccountRegion", "AccountTemplateView", parameters);
    }

    /// <summary>
    /// Opens the AddAccountWindow from the MainWindow
    /// </summary>
    [RelayCommand]
    private void OpenAddAccountWindow() {
        // _windowService.ShowWindow<AddAccountWindow, AddAccountViewModel>();
    }
}