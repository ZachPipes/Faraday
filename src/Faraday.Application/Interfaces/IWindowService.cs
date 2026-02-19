// src/Faraday.Application/Interfaces/IWindowService.cs

namespace Faraday.Application.Interfaces;

/// <summary>
/// Provides a service for opening, showing, and closing windows
/// associated with specific ViewModel types in the UI layer.
/// The implementation is responsible for resolving the correct
/// window type and setting its DataContext to the ViewModel.
/// </summary>
public interface IWindowService {
    /// <summary>
    /// Opens a window associated with the specified ViewModel type.
    /// The window is displayed non-modally (like calling Show()).
    /// </summary>
    /// <typeparam name="TViewModel">The type of the ViewModel to display in the window.</typeparam>
    void Show<TViewModel>() where TViewModel : class;

    /// <summary>
    /// Opens a window associated with the specified ViewModel type
    /// as a modal dialog (like calling ShowDialog()).
    /// </summary>
    /// <typeparam name="TViewModel">The type of the ViewModel to display in the window.</typeparam>
    /// <returns>
    /// A nullable boolean representing the dialog result:
    /// - true if the dialog was accepted,
    /// - false if it was canceled,
    /// - null if no result was set.
    /// </returns>
    bool? ShowDialog<TViewModel>() where TViewModel : class;

    /// <summary>
    /// Closes the currently open window associated with the specified
    /// ViewModel type, if one exists.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the ViewModel whose window should be closed.</typeparam>
    void Close(object viewModel);

    /// <summary>
    /// Opens a file picker dialog allowing the user to select a file.
    /// </summary>
    /// <param name="folderName">
    /// The starting folder name. Default is "Downloads".
    /// </param>
    /// <param name="fileTypes">
    /// The type of files to select. Default is "CSV".
    /// </param>
    /// <returns>
    /// The full path of the selected file, or an empty string if no file was selected.
    /// </returns>
    string ShowFilePicker(string folderName = "Downloads", string fileTypes = "CSV");
}