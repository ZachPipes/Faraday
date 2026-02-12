// src/Faraday.UI/App.xaml.cs

using System.Windows;
using Faraday.Application.Interfaces;
using Faraday.Infrastructure.Repositories;
using Faraday.UI.ViewModels;
using Faraday.UI.ViewModels.DashboardViewModels;
using Faraday.UI.Views;
using Faraday.UI.Views.DashboardViews;

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
        // ======== //
        // Services //
        // ======== //
        containerRegistry.Register<IAccountRepository, AccountRepository>();
        containerRegistry.Register<ITransactionRepository, TransactionRepository>();

        // ===== //
        // Views //
        // ===== //
        // Navigation Views
        containerRegistry.RegisterForNavigation<NavigationBarsView, NavigationBarsViewModel>();

        // Dashboard Views
        containerRegistry.RegisterForNavigation<DashboardView, DashboardViewModel>();
        containerRegistry.RegisterForNavigation<WidgetView, WidgetViewModel>();

        // Settings Views
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