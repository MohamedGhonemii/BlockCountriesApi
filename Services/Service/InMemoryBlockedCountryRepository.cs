using System.Collections.Concurrent;
using BlkCountriesProj.Models;
using BlkCountriesProj.Services.Iservice;

namespace BlkCountriesProj.Services
{
    public class InMemoryBlockedCountryRepository : IBlockedCountryRepository
    {
      
        private readonly ConcurrentDictionary<string, BlockedCountry> _store = new(StringComparer.OrdinalIgnoreCase);

        public bool Add(BlockedCountry country)
        {
            var key = country.CountryCode.ToUpperInvariant();
            return _store.TryAdd(key, country);
        }

        public bool Remove(string countryCode)
        {
            return _store.TryRemove(countryCode.ToUpperInvariant(), out _);
        }

        public BlockedCountry? Get(string countryCode)
        {
            _store.TryGetValue(countryCode.ToUpperInvariant(), out var c);
            return c;
        }

        public IEnumerable<BlockedCountry> GetAll() => _store.Values;

        public IEnumerable<BlockedCountry> Search(string? query)
        {
            if (string.IsNullOrWhiteSpace(query)) return _store.Values;

            var q = query.Trim().ToUpperInvariant();
            return _store.Values.Where(c =>
                c.CountryCode.ToUpperInvariant().Contains(q) ||
                c.CountryName.ToUpperInvariant().Contains(q));
        }

        public void AddOrUpdateTemporal(BlockedCountry country)
        {
            country.IsTemporal = true;
            _store.AddOrUpdate(country.CountryCode.ToUpperInvariant(), country, (_, __) => country);
        }

        public IEnumerable<BlockedCountry> GetExpiredTemporal(DateTime now)
        {
           var Expired =  _store.Values.Where(x => x.IsTemporal == true && x.ExpiresAt.HasValue && x.ExpiresAt.Value <= now).ToList();
            return Expired;
            //return _store.Where(c =>
            //    c.IsTemporal = true &&
            //    c.ExpiresAt.HasValue &&
            //    c.ExpiresAt.Value <= now).ToList();
        }

        public void RemoveExpired(IEnumerable<string> countryCodes)
        {
            foreach (var code in countryCodes)
                _store.TryRemove(code.ToUpperInvariant(), out _);
        }
    }
}
