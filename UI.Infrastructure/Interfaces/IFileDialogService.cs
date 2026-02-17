namespace UI.Infrastructure.Interfaces;

/// <summary>
/// Represents an interface to save/open file dialog services.
/// </summary>
public interface IFileDialogService
{
    /// <summary>
    /// Gets the 0-based index of the filter, selected in a file dialog on last call to <see cref="OpenFile"/>.
    /// </summary>
    int LastFilter { get; }

    /// <summary>
    /// Invoke the save file dialog service.
    /// </summary>
    /// <param name="filter">The file filters to use.</param>
    /// <param name="title">The dialog caption.</param>
    /// <returns>The file name saved. The implementation is expected to return <see cref="string.Empty"/> if the dialog is cancelled.</returns>
    string SaveFile(string filter, string title);

    /// <summary>
    /// Invoke the save file dialog service.
    /// </summary>
    /// <param name="filter">The file filters to use.</param>
    /// <param name="title">The dialog caption.</param>
    /// <param name="initialDirectory">The initial directory to use.</param>
    /// <param name="fileName">The default file name to use.</param>
    /// <returns>The file name saved. The implementation is expected to return <see cref="string.Empty"/> if the dialog is cancelled.</returns>
    string SaveFile(string filter, string title, string initialDirectory, string fileName);

    /// <summary>
    /// Invoke the open file dialog service.
    /// </summary>
    /// <param name="filter">The file filters to use.</param>
    /// <param name="title">The dialog caption.</param>
    /// <param name="initialDirectory">The initial directory to use.</param>
    /// <returns>The file name of the file selected to open.
    /// The implementation is expected to return <see cref="string.Empty"/> if the dialog is cancelled.
    /// </returns>
    string OpenFile(string filter, string title, string initialDirectory = null);

    /// <summary>
    /// Invoke the open file dialog service.
    /// </summary>
    /// <param name="filter">The file filters to use.</param>
    /// <param name="title">The dialog caption.</param>
    /// <param name="initialDirectory">The initial directory to use.</param>
    /// <returns>The names of the files selected to open.
    /// The implementation is expected to return <b>null</b> if the dialog is cancelled.
    /// </returns>
    string[] OpenFiles(string filter, string title, string initialDirectory = null);
}