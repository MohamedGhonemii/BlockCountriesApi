 
namespace BlkCountriesProj.Models
{
    public class LookupResponseDto
{
    public string Ip { get; set; } = "";
    public string? CountryCode { get; set; }
    public string? CountryName { get; set; }
    public string? Isp { get; set; }
    public bool Blocked { get; set; } = false;
    public string? Error { get; set; }
}
}
