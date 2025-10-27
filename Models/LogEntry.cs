
using System;
namespace BlkCountriesProj.Models
{
    public class LogEntry
{
    public string Ip { get; set; } = "";
    public DateTime Timestamp { get; set; }
    public string CountryCode { get; set; } = "";
    public string CountryName { get; set; } = "";
    public bool Blocked { get; set; }
    public string? UserAgent { get; set; }
}
}
