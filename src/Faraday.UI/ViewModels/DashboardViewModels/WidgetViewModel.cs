// src/Faraday.UI/ViewModels/DashboardViewModels/WidgetViewModel.cs

using CommunityToolkit.Mvvm.Input;

namespace Faraday.UI.ViewModels.DashboardViewModels;

public partial class WidgetViewModel : ViewModelBase {
    // =========== //
    // Constructor //
    // =========== //
    public WidgetViewModel() { }


    // ======== //
    // Commands //
    // ======== //
    [RelayCommand]
    public void AddWidgetButton() {
        throw new NotImplementedException();
    }
}