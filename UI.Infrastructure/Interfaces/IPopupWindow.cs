using System;

namespace UI.Infrastructure.Interfaces;

/// <summary>
/// Provides the ability to configure and show views.
/// </summary>
public interface IPopupWindow
{
    /// <summary>
    /// Gets or sets the data context for an element when it participates in data binding.
    /// </summary>
    object DataContext { get; set; }

    /// <summary>
    /// Gets or sets the dialog result value, which is the value that is returned after
    /// window has been closed.
    /// </summary>
    bool? DialogResult { get; set; }

    /// <summary>
    /// Opens a view and returns only when the newly opened view is closed.
    /// </summary>
    /// <returns>A <see cref="Nullable{T}" /> value of type <see cref="bool" /> that
    /// signifies how a view was closed by the user.</returns>
    bool? ShowDialog();

    /// <summary>
    /// Opens a window and returns without waiting for the newly opened window to close.
    /// </summary>
    void Show();

    /// <summary>
    /// Manually closes the window.
    /// </summary>
    void Close();
}