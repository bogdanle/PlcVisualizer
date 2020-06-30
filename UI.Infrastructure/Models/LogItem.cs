using System;

namespace UI.Infrastructure.Models
{
    public class LogItem
    {
        public string Category { get; set; }

        public string Message { get; set; }

        public string Content { get; set; }

        public string StackTrace { get; set; }

        public DateTime DateCreated { get; set; }

        public string UserName { get; set; }

        public string MachineName { get; set; }
    }
}
