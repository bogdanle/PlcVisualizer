using UI.Infrastructure.Interfaces;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace UI.Infrastructure.Services;

/// <summary>
/// Represents the save/open file dialog services.
/// </summary>
public class FileDialogService : IFileDialogService
{
    /// <inheritdoc />
    public int LastFilter { get; private set; }

    /// <inheritdoc />
    public string SaveFile(string filter, string title)
    {
        return SaveFile(filter, title, null, null);
    }

    /// <inheritdoc />
    public string SaveFile(string filter, string title, string initialDirectory, string fileName)
    {
        var saveFileDialog = new SaveFileDialog
        {
            Filter = filter,
            Title = title,
            InitialDirectory = initialDirectory,
            FileName = fileName,
            OverwritePrompt = true
        };

        return saveFileDialog.ShowDialog() == true ? saveFileDialog.FileName : string.Empty;
    }

    /// <inheritdoc />
    public string OpenFile(string filter, string title, string initialDirectory = null)
    {
        var openFileDialog = new OpenFileDialog { Filter = filter, Title = title, InitialDirectory = initialDirectory ?? string.Empty };
        if (openFileDialog.ShowDialog() == true)
        {
            LastFilter = openFileDialog.FilterIndex - 1;
            return openFileDialog.FileName;
        }

        return string.Empty;
    }

    /// <inheritdoc />
    public string[] OpenFiles(string filter, string title, string initialDirectory = null)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = filter,
            Title = title,
            InitialDirectory = initialDirectory ?? string.Empty,
            Multiselect = true,
            ShowReadOnly = true
        };

        if (openFileDialog.ShowDialog() == true)
        {
            LastFilter = openFileDialog.FilterIndex - 1;
            return openFileDialog.FileNames;
        }

        return null;
    }
}
