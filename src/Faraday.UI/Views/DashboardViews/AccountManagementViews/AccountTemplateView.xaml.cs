using System.Windows.Controls;
using Faraday.UI.Models;
using Faraday.UI.ViewModels.DashboardViewModels.AccountManagementViewModels;

namespace Faraday.UI.Views.DashboardViews.AccountManagementViews;

public partial class AccountTemplateView : UserControl {
    // =========== //
    // Constructor //
    // =========== //
    public AccountTemplateView() {
        InitializeComponent();
        Loaded += (_, _) => {
            if (DataContext is AccountTemplateViewModel vm) {
                vm.TransactionsLoaded += () => {
                    // Update first time
                    PlotBalances(vm.DisplayTransactions);

                    // Update on collection change
                    vm.DisplayTransactions.CollectionChanged += (_, _) =>
                        PlotBalances(vm.DisplayTransactions);
                };
            }
        };
    }

    
    // ========= //
    // Functions //
    // ========= //
    /// <summary>
    /// Plots the passed transactions on the AccountGraph object in the AccountTemplateView.xaml
    /// </summary>
    /// <param name="transactions">The transactions to display</param>
    private void PlotBalances(IEnumerable<TransactionDisplay> transactions) {
        AccountGraph.Plot.Clear();

        IEnumerable<TransactionDisplay> transactionDisplays =
            transactions as TransactionDisplay[] ?? transactions.ToArray();
        double[] xs = transactionDisplays.Select(t => t.Date.ToOADate()).ToArray();
        decimal[] ys = transactionDisplays.Select(t => t.Balance).ToArray();

        AccountGraph.Plot.Add.Scatter(xs, ys);
        AccountGraph.Plot.Axes.DateTimeTicksBottom();

        AccountGraph.Refresh();
    }
}