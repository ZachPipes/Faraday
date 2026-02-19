// src/Faraday.UI/Services/WindowService.cs

using System.Windows;
using Faraday.Application.Interfaces;
using Faraday.UI.ViewModels.DashboardViewModels.AccountManagementViewModels;
using Faraday.UI.Views.DashboardViews.AccountManagementViews;
using Microsoft.Win32;

namespace Faraday.UI.Services;

public class WindowService : IWindowService {
    // ============ //
    // Dependencies //
    // ============ //
    private readonly IContainerProvider _containerProvider;


    // ========= //
    // Variables //
    // ========= //
    private readonly Dictionary<object, Window> _openWindows = new();
    private readonly Dictionary<Type, Type> _windowMappings = new() {
        { typeof(AddAccountViewModel), typeof(AddAccountView) },
    };


    // =========== //
    // Constructor //
    // =========== //
    public WindowService(IContainerProvider containerProvider) {
        _containerProvider = containerProvider;
    }


    // =============== //
    // Implementations //
    // =============== //
    [Obsolete("Should use Prism's IDialogService", true)]
    public void Show<TViewModel>() where TViewModel : class {
        Window window = ResolveWindow<TViewModel>();
        window.Show();
    }

    [Obsolete("Should use Prism's IDialogService", true)]
    public bool? ShowDialog<TViewModel>() where TViewModel : class {
        Window window = ResolveWindow<TViewModel>();
        return window.ShowDialog();
    }

    [Obsolete("Should use Prism's IDialogService", true)]
    public void Close(object viewModel) {
        if (!_openWindows.TryGetValue(viewModel, out Window? window)) {
            Console.WriteLine($"No window found for: {viewModel}");
            return;
        }
        
        window.Close();
    }

    public string ShowFilePicker(string folderName = "Downloads", string fileTypes = "CSV") {
        // Pick an active window as the owner
        Window? owner = _openWindows.Values.FirstOrDefault(w => w.IsActive);

        OpenFileDialog openFileDialog = new() {
            Multiselect = false
        };

        // Set initial folder
        switch (folderName) {
            case "Downloads":
                openFileDialog.InitialDirectory = BasicUtilities.GetDownloadsFolder();
                break;
            default:
                return string.Empty;
        }

        // Set file type filter
        switch (fileTypes) {
            case "CSV":
                openFileDialog.Title = "Select CSV File";
                openFileDialog.Filter = "CSV files (*.csv)|*.csv";
                break;
            default:
                return string.Empty;
        }

        // Show dialog with owner if found, else without owner
        bool? result = owner != null ? openFileDialog.ShowDialog(owner) : openFileDialog.ShowDialog();

        if (result == true)
            return openFileDialog.FileName;

        // Optional warning
        MessageBox.Show("No file was selected.", "File Picker", MessageBoxButton.OK, MessageBoxImage.Warning);
        return string.Empty;
    }


    // ================= //
    // Private Functions //
    // ================= //
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private Window ResolveWindow<TViewModel>() where TViewModel : class {
        // Get current vm
        Type vmType = typeof(TViewModel);

        if (!_windowMappings.TryGetValue(vmType, out Type? windowType))
            throw new InvalidOperationException($"No Window registered for {vmType.Name}");

        Window window = (Window)_containerProvider.Resolve(windowType);

        if (window.DataContext is not TViewModel vm) {
            throw new InvalidOperationException($"Prism did not wire vm for {windowType.Name}");
        }
        
        _openWindows[vm] = window;
        window.Closed += (_, _) => _openWindows.Remove(window.DataContext);
        
        Console.WriteLine($"VM: {vm.GetType().Name}");
        Console.WriteLine($"REGISTERED WINDOW FOR VM {vm.GetHashCode()}");
        Console.WriteLine($"{_openWindows.Count} windows opened");
        Console.WriteLine($"TOTAL WINDOWS NOW: {_openWindows.Count}");

        return window;
    }
}