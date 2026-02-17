using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using UI.Core.Extensions;
using UI.Infrastructure;
using UI.Infrastructure.Interfaces;
using UI.Infrastructure.Messaging;

namespace UI.Shell.ViewModels;

public partial class HamburgerMenuViewModel : ViewModelCore<HamburgerMenuViewModel>
{
    [ObservableProperty]
    private string _environment;

    [ObservableProperty]
    private string _buildDate;

    [ObservableProperty]
    private string _applicationVersion;

    [ObservableProperty]
    private IEnumerable<Theme> _themes;

    [ObservableProperty]
    private Theme _selectedTheme;

    public HamburgerMenuViewModel(
        IMessenger messenger,
        IMessageBoxService messageBox,
        IFileDialogService fileDialog,
        IErrorDialogService errorDialog,
        ILogger<HamburgerMenuViewModel> logger)
        : base(messenger, messageBox, fileDialog, errorDialog, logger)
    {
        Messenger.Register<HamburgerMenuViewModel, SwitchViewMessage>(this, static (r, m) => r.OnViewSwitched(m.Value));

        var asm = System.Reflection.Assembly.GetEntryAssembly();
        ApplicationVersion = asm?.GetName().Version?.ToString() ?? "1.0.0";
        BuildDate = asm == null ? DateTime.Now.ToString("dd MMMM yyyy HH:mm") : File.GetCreationTime(asm.Location).ToString("dd MMMM yyyy HH:mm");
        Environment = (ConfigurationManager.AppSettings["Environment"] ?? "dev").Capitalize();

        Themes = new List<Theme>
        {
            new Theme { Name = "Lime", AccentColor = Color.FromRgb(0xa4, 0xc4, 0x00) },
            new Theme { Name = "Green", AccentColor = Color.FromRgb(0x00, 0x9a, 0x50) },
            new Theme { Name = "Emerald", AccentColor = Color.FromRgb(0x00, 0x99, 0x00) },
            new Theme { Name = "Teal", AccentColor = Color.FromRgb(0x00, 0xab, 0xa9) },
            new Theme { Name = "Cyan", AccentColor = Color.FromRgb(0x1b, 0xa1, 0xe2) },
            new Theme { Name = "Cobalt", AccentColor = Color.FromRgb(0x00, 0x50, 0xef) },
            new Theme { Name = "Indigo", AccentColor = Color.FromRgb(0x6a, 0x00, 0xff) },
            new Theme { Name = "Violet", AccentColor = Color.FromRgb(0xaa, 0x00, 0xff) },
            new Theme { Name = "Pink", AccentColor = Color.FromRgb(0xf4, 0x72, 0xd0) },
            new Theme { Name = "Magenta", AccentColor = Color.FromRgb(0xd8, 0x00, 0x73) },
            new Theme { Name = "Crimson", AccentColor = Color.FromRgb(0xa2, 0x00, 0x25) },
            new Theme { Name = "Red", AccentColor = Color.FromRgb(0xe5, 0x14, 0x00) },
            new Theme { Name = "Orange", AccentColor = Color.FromRgb(0xfa, 0x68, 0x00) },
            new Theme { Name = "Amber", AccentColor = Color.FromRgb(0xf0, 0xa3, 0x0a) },
            new Theme { Name = "Yellow", AccentColor = Color.FromRgb(0xe3, 0xc8, 0x00) },
            new Theme { Name = "Brown", AccentColor = Color.FromRgb(0x82, 0x5a, 0x2c) },
            new Theme { Name = "Olive", AccentColor = Color.FromRgb(0x6d, 0x87, 0x64) },
            new Theme { Name = "Steel", AccentColor = Color.FromRgb(0x64, 0x76, 0x87) },
            new Theme { Name = "Mauve", AccentColor = Color.FromRgb(0x76, 0x60, 0x8a) },
            new Theme { Name = "Taupe", AccentColor = Color.FromRgb(0x87, 0x79, 0x4e) },
            new Theme { Name = "Cool Blue", AccentColor = Color.FromRgb(45, 125, 154) }
        };
    }

    public bool IsColorPickerVisible { get; set; } = true;

    partial void OnSelectedThemeChanged(Theme value)
    {
        if (value != null)
        {
            Messenger.Send(new ThemeChangedMessage(value));
        }
    }

    [RelayCommand]
    private void Hide()
    {
        if (View != null)
        {
            dynamic view = View;
            view.IsOpen = false;
        }
    }

    private void OnViewSwitched(object param)
    {
    }

    private void OnMenuItemClicked(object sender, EventArgs e)
    {
        Hide();
        Messenger.Send(new SwitchViewMessage(sender));
    }
}
