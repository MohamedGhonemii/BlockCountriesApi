
using BlkCountriesProj.Services.Iservice;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

[ApiController]
[Route("api/logs")]
public class LogsController : ControllerBase
{
    private readonly ILogService _log;

    public LogsController(ILogService log)
    {
        _log = log;
    }

    [HttpGet("blocked-attempts")]
    public IActionResult GetLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 200) pageSize = 20;

        var items = _log.GetAll();
        var total = items.Count();
        var data = items.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Ok(new { page, pageSize, total, data });
    }
}
