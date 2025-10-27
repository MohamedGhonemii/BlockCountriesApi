using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

using BlkCountriesProj.Services.Iservice;
using Newtonsoft.Json.Linq;

namespace BlkCountriesProj.Services.Service
{
    public class GeoIpService : IGeoIpService
    {
        private readonly HttpClient _http;
        private readonly string _provider;
        private readonly string _apiKey;

        public GeoIpService(HttpClient http, IOptions<GeoIpOptions> opts)
        {
            _http = http;
            _provider = opts.Value.Provider;
            _apiKey = opts.Value.ApiKey;
        }

        public async Task<(bool Success, string? CountryCode, string? CountryName, string? Isp, string? Error)> LookupAsync(string ip, CancellationToken ct = default)
        {
            try
            {
                if (_provider == "ipapi")
                {
                    var url = $"https://ipapi.co/{ip}/json/";
                    var res = await _http.GetAsync(url, ct);
                    if (!res.IsSuccessStatusCode)
                        return (false, null, null, null, $"Geo API status: {res.StatusCode}");

                    var json = await res.Content.ReadAsStringAsync(ct);
                    var obj = JObject.Parse(json);
                    var code = (string?)obj["country_code"];
                    var name = (string?)obj["country_name"];
                    var isp = (string?)obj["org"] ?? (string?)obj["asn_org"];
                    return (true, code, name, isp, null);
                }

                if (_provider == "ipgeolocation")
                {
                    var url = $"https://api.ipgeolocation.io/ipgeo?apiKey={_apiKey}&ip={ip}";
                    var res = await _http.GetAsync(url, ct);
                    if (!res.IsSuccessStatusCode)
                        return (false, null, null, null, $"Geo API status: {res.StatusCode}");

                    var json = await res.Content.ReadAsStringAsync(ct);
                    var obj = JObject.Parse(json);
                    var code = (string?)obj["country_code2"];
                    var name = (string?)obj["country_name"];
                    var isp = (string?)obj["isp"];
                    return (true, code, name, isp, null);
                }

                return (false, null, null, null, "Unknown provider configured");
            }
            catch (Exception ex)
            {
                return (false, null, null, null, ex.Message);
            }
        }
    }

    public record GeoIpOptions
    {
        public string Provider { get; init; } = "ipapi";
        public string ApiKey { get; init; } = "";
    }
}
