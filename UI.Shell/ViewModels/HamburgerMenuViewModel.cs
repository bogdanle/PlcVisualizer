using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using Prism.Commands;
using Prism.Events;
using UI.Core.Extensions;
using UI.Infrastructure;
using UI.Infrastructure.Events;
using Unity;

namespace UI.Shell.ViewModels
{
    public class HamburgerMenuViewModel : ViewModelCore<HamburgerMenuViewModel>
    {
        private readonly IEventAggregator _eventAggregator;
        private ICommand _hideCommand;
        private string _environment;
        private string _buildDate;
        private string _applicationVersion;
        private IEnumerable<Theme> _themes;
        private Theme _selectedTheme;

        public HamburgerMenuViewModel(
            IEventAggregator eventAggregator,
            IUnityContainer container)
            : base(container)
        {
            _eventAggregator = eventAggregator;

            EventAggregator.GetEvent<SwitchViewEvent>().Subscribe(OnViewSwitched, ThreadOption.UIThread);

            var asm = System.Reflection.Assembly.GetEntryAssembly();
            ApplicationVersion = asm.GetName().Version.ToString();
            BuildDate = File.GetCreationTime(asm.Location).ToString("dd MMMM yyyy HH:mm");
            Environment = (ConfigurationManager.AppSettings["Environment"] ?? "dev").Capitalize();

            var list = new List<Theme>
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

            Themes = list;
        }

        public Theme SelectedTheme
        {
            get => _selectedTheme;

            set
            {
                SetProperty(ref _selectedTheme, value);
                _eventAggregator.GetEvent<ThemeChangedEvent>().Publish(value);
            }
        }

        public IEnumerable<Theme> Themes
        {
            get => _themes;
            set => SetProperty(ref _themes, value);
        }

        public string ApplicationVersion
        {
            get => _applicationVersion;
            set => SetProperty(ref _applicationVersion, value);
        }

        public string BuildDate
        {
            get => _buildDate;
            set => SetProperty(ref _buildDate, value);
        }

        public string Environment
        {
            get => _environment;
            set => SetProperty(ref _environment, value);
        }

        public bool IsColorPickerVisible { get; set; } = true;

        public ICommand HideCommand => _hideCommand ?? (_hideCommand = new DelegateCommand(OnHide));

        private void OnViewSwitched(object param)
        {
        }

        private void OnMenuItemClicked(object sender, System.EventArgs e)
        {
             OnHide(); 
             _eventAggregator.GetEvent<SwitchViewEvent>().Publish(sender);
        }

        private void OnHide()
        {
            if (View != null)
            {
                dynamic view = View;
                view.IsOpen = false;
            }
        }
    }
}