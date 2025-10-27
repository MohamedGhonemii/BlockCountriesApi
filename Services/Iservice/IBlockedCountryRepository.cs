using BlkCountriesProj.Models;

namespace BlkCountriesProj.Services.Iservice
{
    public interface IBlockedCountryRepository
    {
        bool Add(BlockedCountry country);
        bool Remove(string countryCode);
        BlockedCountry? Get(string countryCode);
        IEnumerable<BlockedCountry> GetAll();
        IEnumerable<BlockedCountry> Search(string? query);
        void AddOrUpdateTemporal(BlockedCountry country);
        IEnumerable<BlockedCountry> GetExpiredTemporal(DateTime now);
        void RemoveExpired(IEnumerable<string> countryCodes);
    }
}
