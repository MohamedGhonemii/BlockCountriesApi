using System.Collections.Concurrent;
using BlkCountriesProj.Models;
using BlkCountriesProj.Services.Iservice;

namespace BlkCountriesProj.Services.Service
{
    public class LogService : ILogService
    {
        private readonly ConcurrentBag<LogEntry> _logs = new();

        public void Add(LogEntry entry)
        {
            _logs.Add(entry);
        }

        public IEnumerable<LogEntry> GetAll()
        {
            return _logs.OrderByDescending(l => l.Timestamp);
        }
    }
}
