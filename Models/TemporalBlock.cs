 
namespace BlkCountriesProj.Models
{
    public class TemporalBlock
{
    public string CountryCode { get; set; } = "";
    public string CountryName { get; set; } = "";
    public DateTime ExpiresAt { get; set; }


    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}
}
