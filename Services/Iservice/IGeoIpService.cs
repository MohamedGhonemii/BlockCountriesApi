namespace BlkCountriesProj.Services.Iservice
{
    public interface IGeoIpService
    {
        Task<(bool Success, string? CountryCode, string? CountryName, string? Isp, string? Error)>
            LookupAsync(string ip, CancellationToken ct = default);
    }
}
