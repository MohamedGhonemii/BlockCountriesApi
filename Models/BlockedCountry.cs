
using System;


namespace BlkCountriesProj.Models
{
    public class BlockedCountry
{
    public string CountryCode { get; set; } = "";
    public string CountryName { get; set; } = "";
    public bool IsTemporal { get; set; } = false;
    public DateTime? ExpiresAt { get; set; }
}
}
