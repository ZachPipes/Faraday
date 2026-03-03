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
    private readonly IDialogService _dialogService;


    // ========= //
    // Variables //
    // ========= //
    [ObservableProperty] private ObservableCollection<Account> _accounts;


    // =========== //
    // Constructor //
    // =========== //
    SettingsViewModel(IAccountRepository accountRepository, IDialogService dialogService) {
        _accountRepository = accountRepository;
        _dialogService = dialogService;
        
        Accounts = [];

        _ = LoadAccounts();
    }


    // ======== //
    // Commands //
    // ======== //
    [RelayCommand]
    private async Task EditAccount(Account account) {
        DialogParameters parameters = new() { { "SelectedAccount", account } };
        _dialogService.ShowDialog("EditAccountView", parameters, result => {
            if (result.Result == ButtonResult.OK) {
                _ = LoadAccounts();
            }
        });
        await _accountRepository.UpdateAsync(account);
        await LoadAccounts();
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