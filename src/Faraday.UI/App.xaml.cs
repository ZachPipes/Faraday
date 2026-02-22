// src/Faraday.UI/App.xaml.cs

using System.IO;
using System.Windows;
using Faraday.Application.Interfaces;
using Faraday.Infrastructure.Persistence;
using Faraday.Infrastructure.Repositories;
using Faraday.UI.Services;
using Faraday.UI.ViewModels;
using Faraday.UI.ViewModels.DashboardViewModels;
using Faraday.UI.ViewModels.DashboardViewModels.AccountManagementViewModels;
using Faraday.UI.ViewModels.SettingsViewModels;
using Faraday.UI.Views;
using Faraday.UI.Views.DashboardViews;
using Faraday.UI.Views.DashboardViews.AccountManagementViews;
using Faraday.UI.Views.SettingsViews;
using Microsoft.EntityFrameworkCore;

namespace Faraday.UI;

/// <summary>
/// Prism application bootstrapper responsible for configuring the dependency
/// injection container, registering navigation mappings, and initializing
/// the application shell.
/// </summary>
public partial class App : PrismApplication {
    /// <summary>
    /// Creates and returns the main application window (Shell).
    /// Prism calls this during application startup.
    /// </summary>
    /// <returns>The resolved <see cref="Shell"/> instance.</returns>
    protected override Window CreateShell() {
        return Container.Resolve<Shell>();
    }

    /// <summary>
    /// Registers services, repositories, and navigable views with the Prism
    /// dependency injection container.
    /// </summary>
    /// <param name="containerRegistry">
    /// The Prism container registry used to map interfaces to implementations
    /// and register views for navigation.
    /// </param>
    protected override void RegisterTypes(IContainerRegistry containerRegistry) {
        // ================ //
        // Database Handler //
        // ================ //
        DbContextOptionsBuilder<FaradayDbContext> optionsBuilder = new();
        optionsBuilder.UseSqlite($"Data Source={AppState.Instance.DbFolderPath}");

        FaradayDbContext dbContext = new(optionsBuilder.Options);
        dbContext.Database.EnsureCreated();
        containerRegistry.GetContainer().RegisterInstance(dbContext);

        // ======== //
        // Services //
        // ======== //
        containerRegistry.Register<IAccountRepository, AccountRepository>();
        containerRegistry.Register<ITransactionRepository, TransactionRepository>();
        containerRegistry.Register<IStockRepository, StockRepository>();
        containerRegistry.Register<IWindowService, WindowService>();
        containerRegistry.Register<ICSVService, CSVService>();

        // ===== //
        // Views //
        // ===== //
        // Navigation Views
        containerRegistry.RegisterForNavigation<NavigationBarsView, NavigationBarsViewModel>();

        // Dashboard Views
        containerRegistry.RegisterForNavigation<DashboardView, DashboardViewModel>();
        containerRegistry.RegisterForNavigation<WidgetView, WidgetViewModel>();
        containerRegistry.RegisterForNavigation<AccountTemplateView, AccountTemplateViewModel>();
        containerRegistry.RegisterDialog<AddAccountView, AddAccountViewModel>();

        // Settings Views
        containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModel>();
    }

    /// <summary>
    /// Performs post-initialization configuration after the shell has been created.
    /// Initializes default region navigation for the application layout.
    /// </summary>
    protected override void OnInitialized() {
        base.OnInitialized();

        IRegionManager regionManager = Container.Resolve<IRegionManager>();
        regionManager.RequestNavigate("NavbarRegion", "NavigationBarsView");
        regionManager.RequestNavigate("MainRegion", "DashboardView");
    }
}