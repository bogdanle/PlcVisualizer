using System;

namespace UI.Controls
{
    public enum NotificationMessageType
    {
        Success,
        Info,
        Warning,
        Error,
    }

    public class NotificationMessageData
    {
        public NotificationMessageData(string title, string message, NotificationMessageType type = NotificationMessageType.Error)
        {
            Title = title;
            Message = message;
            Type = type;
            Timestamp = DateTime.Now;
            AutoFade = true;
        }

        public NotificationMessageData(string message, NotificationMessageType type = NotificationMessageType.Error)
        {
            Title = "Plc Visualizer";
            Message = message;
            Type = type;
            Timestamp = DateTime.Now;
            AutoFade = true;
        }

        public string Title { get; }

        public string Message { get; }

        public object Source { get; set; }

        public DateTime Timestamp { get; }

        public NotificationMessageType Type { get; }

        public bool AutoFade { get; set; }
    }    
}