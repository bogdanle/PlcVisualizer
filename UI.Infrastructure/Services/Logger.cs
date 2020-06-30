using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using UI.Core.Extensions;
using UI.Infrastructure.Interfaces;
using UI.Infrastructure.Models;

namespace UI.Infrastructure.Services
{
    public class Logger : ILogger
    {
        private ConcurrentQueue<LogItem> _theQueue;

        public Logger()
        {
            Start();
        }

        public enum Category
        {
            Debug,
            Info,
            Warning,
            Error,
            Exception
        }

        public void Exception(Exception ex)
        {
            if (ex == null)
            {
                return;
            }

            Console.WriteLine(ex.Message);

            try
            {
                var obj = new LogItem
                {
                    Category = "Exception",
                    Message = ex.Message,
                    Content = $"{ex.GetType()} {ex.InnerException?.Message} {ex.InnerException?.GetType()}",
                    StackTrace = ex.StackTrace,
                    DateCreated = DateTime.Now,
                    UserName = Environment.UserName,
                    MachineName = Environment.MachineName
                };

                obj.Message = obj.Message.Substring(0, Math.Min(obj.Message.Length, 2000));

                _theQueue.Enqueue(obj);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Trace(string message)
        {
            throw new NotImplementedException();
        }

        public void Info(string message)
        {
            Write(Category.Info, message);
        }

        public void Debug(string message)
        {
#if DEBUG
            Write(Category.Debug, message);
#endif
        }

        public void Warning(string message)
        {
            Write(Category.Warning, message);
        }

        public void Error(string message)
        {
            Write(Category.Error, message);
        }

        private void Start()
        {
            if (_theQueue == null)
            {
                _theQueue = new ConcurrentQueue<LogItem>();

                Task.Run(async () =>
                {
                    while (true)
                    {
                        var items = new List<LogItem>();
                        while (_theQueue.TryDequeue(out var item))
                        {
                            items.Add(item);

                            Console.WriteLine($"Log: {item.Message}");
                        }

                        if (items.Count > 0)
                        {
                            try
                            {
                                Console.WriteLine($"Saving {items.Count} item(s) to the log");
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }

                        await Task.Delay(500);
                    }
                });
            }
        }

        private void Write(Category category, string message)
        {
            if (message == null)
            {
                return;
            }

            try
            {
                var sb = new StringBuilder();
                string[] lines = new StackTrace(true).ToString().GetLines();
                int startIndex = Math.Min(2, lines.Length);
                for (int i = startIndex; i < Math.Min(lines.Length, startIndex + 5); i++)
                {
                    sb.AppendLine(lines[i]);
                }

                var obj = new LogItem
                {
                    Category = category.ToString(),
                    Message = message,
                    StackTrace = sb.ToString(),
                    DateCreated = DateTime.Now,
                    UserName = Environment.UserName,
                    MachineName = Environment.MachineName
                };

                _theQueue.Enqueue(obj);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
