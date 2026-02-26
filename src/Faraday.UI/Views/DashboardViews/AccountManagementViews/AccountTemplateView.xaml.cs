using System.Windows;
using System.Windows.Controls;
using Faraday.UI.Models;
using Faraday.UI.ViewModels.DashboardViewModels.AccountManagementViewModels;

namespace Faraday.UI.Views.DashboardViews.AccountManagementViews;

public partial class AccountTemplateView : UserControl {
    // ========= //
    // Variables //
    // ========= //
    private AccountTemplateViewModel? _viewModel;
    
    // =========== //
    // Constructor //
    // =========== //
    public AccountTemplateView() {
        InitializeComponent();
        
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    
    // ========= //
    // Functions //
    // ========= //
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnLoaded(object sender, RoutedEventArgs e) {
        if (DataContext is not AccountTemplateViewModel vm) return;
        _viewModel = vm;

        // Subscribe to events
        _viewModel.TransactionsLoaded += OnTransactionsLoaded;
        _viewModel.DisplayTransactions.CollectionChanged += OnCollectionChanged;

        // Initial Plot
        if (_viewModel.DisplayTransactions.Any()) {
            PlotBalances(_viewModel.DisplayTransactions);
        }
    }

    /// <summary>
    /// Unloads data to prevent memory leaks
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnUnloaded(object sender, RoutedEventArgs e) {
        if (_viewModel == null) return;
        
        // Unsubscribe to prevent memory leaks and ghost updates
        _viewModel.TransactionsLoaded -= OnTransactionsLoaded;
        _viewModel.DisplayTransactions.CollectionChanged -= OnCollectionChanged;
        _viewModel = null;
    }
    
    /// <summary>
    /// Plots balances when the transactions are loaded
    /// </summary>
    /// <exception cref="InvalidOperationException">Throws if the transactions are null</exception>
    private void OnTransactionsLoaded() {
        if (_viewModel?.DisplayTransactions != null) PlotBalances(_viewModel?.DisplayTransactions ?? throw new InvalidOperationException());
    }

    /// <summary>
    /// Plots balances when the collection is changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="InvalidOperationException">Throws if the transactions are null</exception>
    private void OnCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
        if (_viewModel?.DisplayTransactions != null) PlotBalances(_viewModel?.DisplayTransactions ?? throw new InvalidOperationException());
    }

    /// <summary>
    /// Plots the passed transactions on the AccountGraph object in the AccountTemplateView.xaml
    /// </summary>
    /// <param name="transactions">The transactions to display</param>
    private void PlotBalances(IEnumerable<TransactionDisplay> transactions) {
        AccountGraph.Plot.Clear();

        IEnumerable<TransactionDisplay> transactionDisplays =
            transactions as TransactionDisplay[] ?? transactions.ToArray();
        var groupedTransactions = transactionDisplays
            .GroupBy(t => t.Date.Date)
            .Select(g => new 
            { 
                Date = g.Key.ToOADate(), 
                TotalBalance = g.Sum(t => (double)t.Balance)
            })
            .OrderBy(g => g.Date)
            .ToList();
        
        double[] xs = groupedTransactions.Select(g => g.Date).ToArray();
        double[] ys = groupedTransactions.Select(g => g.TotalBalance).ToArray();

        AccountGraph.Plot.Add.Scatter(xs, ys);
        AccountGraph.Plot.Axes.DateTimeTicksBottom();

        AccountGraph.Refresh();
    }
}