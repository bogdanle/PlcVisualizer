using System;
using System.Windows;

namespace UI.Controls
{
    public class WindowPos
    {
        public double Left { get; set; }
        
        public double Top { get; set; }
        
        public double Width { get; set; }
        
        public double Height { get; set; }
        
        public WindowState State { get; set; }

        public static WindowPos FromString(string state)
        {            
            var parts = state.Split(',');
            if (parts.Length == 5)
            {
                var obj = new WindowPos()
                {
                    Left = Convert.ToDouble(parts[0]),
                    Top = Convert.ToDouble(parts[1]),
                    Width = Convert.ToDouble(parts[2]),
                    Height = Convert.ToDouble(parts[3]),
                    State = FromString(parts[4], WindowState.Normal)
                };

                return obj;
            }

            return null;
        }

        public static T FromString<T>(string description, T defaultValue = default(T))
        {
            if (!string.IsNullOrEmpty(description))
            {
                return (T)Enum.Parse(typeof(T), description);
            }

            return defaultValue;
        }

        public override string ToString()
        {
            return $"{Left},{Top},{Width},{Height},{State}";
        }
    }
}
