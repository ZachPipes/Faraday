using System.Windows;
using Faraday.UI.ViewModels;
using Faraday.UI.ViewModels.DashboardViewModels;
using Faraday.UI.Views;
using Faraday.UI.Views.DashboardViews;

namespace Faraday.UI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : PrismApplication {
    protected override Window CreateShell() {
        return Container.Resolve<Shell>();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry) {
        // Services
        // containerRegistry.Register<IAccountService, AccountService>();

        // Views
        // Navigation Views
        containerRegistry.RegisterForNavigation<NavigationBarsView, NavigationBarsViewModel>();

        // Dashboard Views
        containerRegistry.RegisterForNavigation<DashboardView, DashboardViewModel>();
        containerRegistry.RegisterForNavigation<WidgetView, WidgetViewModel>();

        // Settings Views
        
    }

    protected override void OnInitialized() {
        base.OnInitialized();
        
        IRegionManager regionManager = Container.Resolve<IRegionManager>();
        regionManager.RequestNavigate("NavbarRegion", "NavigationBarsView"); 
        regionManager.RequestNavigate("MainRegion", "DashboardView");
    }
}