using System.CodeDom;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Faraday.Application.Interfaces;
using Faraday.Domain.Entities;

namespace Faraday.UI.ViewModels.SettingsViewModels;

public partial class SettingsViewModel : ViewModelBase {
    // ============ //
    // Dependencies //
    // ============ //
    private readonly IAccountRepository _accountRepository;


    // ========= //
    // Variables //
    // ========= //
    [ObservableProperty] private ObservableCollection<Account> _accounts;


    // =========== //
    // Constructor //
    // =========== //
    SettingsViewModel(IAccountRepository accountRepository) {
        _accountRepository = accountRepository;
        Accounts = [];

        _ = LoadAccounts();
    }


    // ======== //
    // Commands //
    // ======== //
    [RelayCommand]
    private async Task EditAccount(Account account) {
        // TODO - Reuse AddAccount view or create new edit view(use outcome for adding transactions as well)
        // await _accountRepository.UpdateAsync(account);
        // await LoadAccounts();
    }

    [RelayCommand]
    private async Task ActivateAccount(Account account) {
        account.Activate();
        await _accountRepository.UpdateAsync(account);
        await LoadAccounts();
    }

    [RelayCommand]
    private async Task DeactivateAccount(Account account) {
        account.Deactivate();
        await _accountRepository.UpdateAsync(account);
        await LoadAccounts();
    }


    // ========= //
    // Functions //
    // ========= //
    /// <summary>
    /// Loads all accounts for editing
    /// </summary>
    private async Task LoadAccounts() {
        Accounts.Clear();
        IEnumerable<Account> accounts = await _accountRepository.GetAllAsync();
        foreach (Account account in accounts) {
            Accounts.Add(account);
        }
    }
}