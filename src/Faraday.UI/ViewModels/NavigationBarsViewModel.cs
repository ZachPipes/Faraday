using CommunityToolkit.Mvvm.Input;

namespace Faraday.UI.ViewModels;

public partial class NavigationBarsViewModel : ViewModelBase {
    // ============ //
    // Dependencies //
    // ============ //
    private readonly IRegionManager _regionManager;

    
    
    // =========== //
    // Constructor //
    // =========== //
    public NavigationBarsViewModel(IRegionManager regionManager) {
        _regionManager = regionManager;
    }



    // ======== //
    // Commands //
    // ======== //
    /// <summary>
    /// Interprets user input to switch which page is presented.
    /// </summary>
    /// <param name="parameter">Text received from the button(CommandParameter).</param>
    /// <exception cref="InvalidOperationException">If the text received(parameter) is not recognized by the switch statement.</exception>
    [RelayCommand]
    private void NavbarSelection(string parameter) {
        switch (parameter) {
            case "Dashboard":
                _regionManager.RequestNavigate("MainRegion", "DashboardView");
                break;

            case "TEMPSettings":
                // _regionManager.RequestNavigate("MainRegion", "SettingsView");
                _regionManager.RequestNavigate("MainRegion", "SettingsView", result => {
                    if (!result.Success && result.Exception != null)
                        Console.WriteLine($"Navigation failed: {result.Exception.Message}");
                });
                break;

            default:
                throw new InvalidOperationException(
                    $"NavigationBarsViewModel:NavbarSelection - Unknown parameter: {parameter}");
        }
    }
}