
using BlkCountriesProj.Models;
using BlkCountriesProj.Services.Iservice;
using BlkCountriesProj.Services.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

[ApiController]
[Route("api/countries")]
public class CountriesController : ControllerBase
{
    private readonly IBlockedCountryRepository _repo;

    public CountriesController(IBlockedCountryRepository repo)
    {
        _repo = repo;
    }

    [HttpPost("block")]
    public IActionResult Block([FromBody] BlockRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.CountryCode) || dto.CountryCode.Trim().Length != 2)
            return BadRequest("CountryCode must be 2 letters (ISO alpha-2).");

        var country = new BlockedCountry
        {
            CountryCode = dto.CountryCode.Trim().ToUpperInvariant(),
            CountryName = dto.CountryName ?? dto.CountryCode
        };

        var added = _repo.Add(country);
        if (!added) return Conflict("Country already blocked.");
        return CreatedAtAction(nameof(GetBlocked), new { countryCode = country.CountryCode }, country);
    }

    [HttpPost("temporal-block")]
    public IActionResult TemporalBlock([FromBody] TemporalBlockRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.CountryCode) || dto.CountryCode.Trim().Length != 2)
            return BadRequest("CountryCode must be 2 letters.");

        if (dto.DurationMinutes < 1 || dto.DurationMinutes > 1440)
            return BadRequest("DurationMinutes must be between 1 and 1440.");

        var code = dto.CountryCode.Trim().ToUpperInvariant();
        var existing = _repo.Get(code);
        var expires = DateTime.UtcNow.AddMinutes(dto.DurationMinutes);

        var country = new BlockedCountry
        {
            CountryCode = code,
            CountryName = dto.CountryName ?? code,
            IsTemporal = true,
            ExpiresAt = expires
        };

       
        _repo.AddOrUpdateTemporal(country);
        return Created("", country);
    }

    [HttpDelete("block/{countryCode}")]
    public IActionResult Unblock(string countryCode)
    {
        if (string.IsNullOrWhiteSpace(countryCode)) return BadRequest();
        var removed = _repo.Remove(countryCode);
        if (!removed) return NotFound();
        return NoContent();
    }

    [HttpGet("blocked")]
    public IActionResult GetBlocked([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? q = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var items = _repo.Search(q).OrderBy(c => c.CountryCode);
        var total = items.Count();
        var data = items.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return Ok(new { page, pageSize, total, data });
    }

    [HttpGet("blocked/{countryCode}")]
    public IActionResult GetBlocked(string countryCode)
    {
        var c = _repo.Get(countryCode);
        if (c == null) return NotFound();
        return Ok(c);
    }
}
