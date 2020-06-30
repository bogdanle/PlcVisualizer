using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace UI.Infrastructure
{
    public class ThemeManager
    {
        private static Dictionary<string, Color> _keyColors;
        private string _name;
        private Color _keyColor;

        public static Dictionary<string, Color> KeyColors => _keyColors ?? (_keyColors = new Dictionary<string, Color>
        {
            { "lime", Color.FromRgb(0xa4, 0xc4, 0x00) },
            { "green", Color.FromRgb(0x00, 0x9a, 0x50) },
            { "emerald", Color.FromRgb(0x00, 0x99, 0x00) },
            { "teal", Color.FromRgb(0x00, 0xab, 0xa9) },
            { "cyan", Color.FromRgb(0x1b, 0xa1, 0xe2) },
            { "cobalt", Color.FromRgb(0x00, 0x50, 0xef) },
            { "indigo", Color.FromRgb(0x6a, 0x00, 0xff) },
            { "violet", Color.FromRgb(0xaa, 0x00, 0xff) },
            { "pink", Color.FromRgb(0xf4, 0x72, 0xd0) },
            { "magenta", Color.FromRgb(0xd8, 0x00, 0x73) },
            { "crimson", Color.FromRgb(0xa2, 0x00, 0x25) },
            { "red", Color.FromRgb(0xe5, 0x14, 0x00) },
            { "orange", Color.FromRgb(0xfa, 0x68, 0x00) },
            { "amber", Color.FromRgb(0xf0, 0xa3, 0x0a) },
            { "yellow", Color.FromRgb(0xe3, 0xc8, 0x00) },
            { "brown", Color.FromRgb(0x82, 0x5a, 0x2c) },
            { "olive", Color.FromRgb(0x6d, 0x87, 0x64) },
            { "steel", Color.FromRgb(0x64, 0x76, 0x87) },
            { "mauve", Color.FromRgb(0x76, 0x60, 0x8a) },
            { "taupe", Color.FromRgb(0x87, 0x79, 0x4e) },
            { "cool blue", Color.FromRgb(45, 125, 154) }
        });

        public Color ThemeKeyColor { get; set; }

        public Color FocusGlowColor { get; set; }

        public Color ListItemSelectedActiveColor { get; set; }

        public Color ListItemSelectedInactiveColor { get; set; }

        public Color ListItemSelectedDisabledColor { get; set; }

        public Color ProgressBarIndicatorColor { get; set; }

        public void Apply(string name)
        {
            _name = name.ToLower();
            var keyColor = KeyColors[_name];
            ThemeKeyColor = keyColor;
            _keyColor = KeyColors[_name];

            Color color = _keyColor;

            Application.Current.Resources["FocusGlowColor"] = color;
            Application.Current.Resources["AccentColor"] = color;
            Application.Current.Resources["AccentColorBrush"] = new SolidColorBrush(color);
           
            Application.Current.Resources["WindowBorderActive"] = new SolidColorBrush(color);
            Application.Current.Resources["WindowBorder"] = new SolidColorBrush(color) { Opacity = 0.5 };

            var backgroundColor = (Color)Application.Current.Resources["DefaultBackgroundColor"];
            var hoverColor = BlendColors(backgroundColor, 1, color, 0.4f);
            var hoverBrush = new SolidColorBrush(hoverColor);
            var selectedTextColor = Colors.WhiteSmoke;
            var selectedTextBrush = new SolidColorBrush(selectedTextColor);
            var selectionColor = BlendColors(backgroundColor, 0.2f, color, 1f);
            var selectionBrush = new SolidColorBrush(selectionColor);
            var textBoxSelectionColor = BlendColors(backgroundColor, 0.2f, color, 1f);
        }

        private static Color BlendColors(Color clr1, float intensity1, Color clr2, float intensity2)
        {
            var clr = new Color
            {
                ScA = ((clr1.ScA * intensity1) + (clr2.ScA * intensity2)) / (intensity1 + intensity2),
                ScR = ((clr1.ScR * intensity1) + (clr2.ScR * intensity2)) / (intensity1 + intensity2),
                ScG = ((clr1.ScG * intensity1) + (clr2.ScG * intensity2)) / (intensity1 + intensity2),
                ScB = ((clr1.ScB * intensity1) + (clr2.ScB * intensity2)) / (intensity1 + intensity2)
            };

            return clr;
        }
    }
}
