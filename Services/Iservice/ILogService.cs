using BlkCountriesProj.Models;

namespace BlkCountriesProj.Services.Iservice
{
    public interface ILogService
    {
        void Add(LogEntry entry);
        IEnumerable<LogEntry> GetAll();
    }
}
