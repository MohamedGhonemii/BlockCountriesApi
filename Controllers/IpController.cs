
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Net;
using BlkCountriesProj.Services.Iservice;
using Microsoft.Extensions.Logging.Abstractions;
using BlkCountriesProj.Models;

[ApiController]
[Route("api/ip")]
public class IpController : ControllerBase
{
    private readonly IGeoIpService _geo;
    private readonly IBlockedCountryRepository _repo;
    private readonly ILogService _log;

    public IpController(IGeoIpService geo, IBlockedCountryRepository repo, ILogService log)
    {
        _geo = geo;
        _repo = repo;
        _log = log;
    }

    [HttpGet("lookup")]
    public async Task<IActionResult> Lookup([FromQuery] string? ipAddress)
    {
        var ip = ipAddress;
        if (string.IsNullOrWhiteSpace(ip))
            ip = GetClientIp();

        if (!System.Net.IPAddress.TryParse(ip, out _))
            return BadRequest("Invalid IP address.");

        var res = await _geo.LookupAsync(ip);
        if (!res.Success) return StatusCode(503, new { error = res.Error });

        return Ok(new { ip, countryCode = res.CountryCode, countryName = res.CountryName });
    }

    [HttpGet("check-block")]
    public async Task<IActionResult> CheckBlock([FromQuery] string? ipAddress)
    {
        var ip = ipAddress;
        if (string.IsNullOrWhiteSpace(ip))
            ip = GetClientIp();

        if (!System.Net.IPAddress.TryParse(ip, out _))
            return BadRequest("Invalid IP address.");

        var res = await _geo.LookupAsync(ip);
        if (!res.Success)
        {
         
            return StatusCode(503, new { error = res.Error });
        }

        var blocked = false;
        if (!string.IsNullOrWhiteSpace(res.CountryCode))
        {
            var bc = _repo.Get(res.CountryCode);
            blocked = bc != null;
        }

        _log.Add(new LogEntry
        {
            Ip = ip,
            Timestamp = DateTime.UtcNow,
            CountryCode = res.CountryCode ?? "",
            CountryName = res.CountryName ?? "",
            Blocked = blocked,
            UserAgent = Request.Headers["User-Agent"].ToString()
        });

        return Ok(new { ip, countryCode = res.CountryCode, countryName = res.CountryName, blocked });
    }

    private string GetClientIp()
    {
        if (Request.Headers.TryGetValue("X-Forwarded-For", out var vals))
            return vals.ToString().Split(',')[0].Trim();

        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
    }
}
