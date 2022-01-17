using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.JsonWebTokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Logic
{
    public static class DiscoveryLogic
    {
        public static async Task<string> GetModulusAsync(string tenantID, string keyType, JsonWebToken token)
        {
            if (!string.IsNullOrWhiteSpace(keyType) && keyType.Equals("x5t", StringComparison.InvariantCultureIgnoreCase))
            {
                return await GetModulusByX5tAsync(tenantID, token.X5t);
            }
            else
            {
                return await GetModulusByKidAsync(tenantID, token.Kid);
            }
        }

        public static async Task<string> GetModulusByX5tAsync(string tenantID, string x5t)
        {
            // Get dictionaries
            var dictionaries = await GetDictionariesCachedAsync(tenantID);

            // Get the modulus from the dictionary
            if (dictionaries.X5tDictionary.TryGetValue(x5t, out var entry))
            {
                return entry;
            }
            return string.Empty;
        }

        public static async Task<string> GetModulusByKidAsync(string tenantID, string kid)
        {
            // Get dictionaries
            var dictionaries = await GetDictionariesCachedAsync(tenantID);

            // Get the modulus from the dictionary
            if (dictionaries.KidDictionary.TryGetValue(kid, out var entry))
            {
                return entry;
            }
            return string.Empty;
        }

        private static string _cacheKeyPrefix = Guid.NewGuid().ToString();
        private static readonly IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions() { });
        private static async Task<KeyCollectionDictionaries> GetDictionariesCachedAsync(string tenantID)
        {
            var cacheKey = $"{_cacheKeyPrefix}_{tenantID}";
            if (!_memoryCache.TryGetValue(cacheKey, out KeyCollectionDictionaries dictionaires))
            {
                dictionaires = await BuildDictionariesAsync(tenantID);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromHours(1));
                _memoryCache.Set(cacheKey, dictionaires, cacheEntryOptions);
            }
            return dictionaires;
        }

        private static async Task<KeyCollectionDictionaries> BuildDictionariesAsync(string tenantID)
        {
            var keyCollectionDictionaries = new KeyCollectionDictionaries();

            // Retrieve the keys for the tenant
            using var client = new HttpClient();
            string url = $@"https://login.microsoftonline.com/{tenantID}/discovery/v2.0/keys";
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode && response.Content != null)
            {
                var rawResponse = await response.Content.ReadAsStringAsync();

                // Deserialize the response
                var keysCollection = JsonSerializer.Deserialize<KeyCollection>(rawResponse);

                // Build dictionary for all the keys
                foreach (var key in keysCollection.Keys)
                {
                    if (!keyCollectionDictionaries.X5tDictionary.ContainsKey(key.X5t))
                    {
                        keyCollectionDictionaries.X5tDictionary.Add(key.X5t, key.N);
                    }
                    if (!keyCollectionDictionaries.KidDictionary.ContainsKey(key.Kid))
                    {
                        keyCollectionDictionaries.KidDictionary.Add(key.Kid, key.N);
                    }
                }
            }

            return keyCollectionDictionaries;
        }

        private class KeyCollectionDictionaries
        {
            public Dictionary<string, string> X5tDictionary { get; set; } = new Dictionary<string, string>();
            public Dictionary<string, string> KidDictionary { get; set; } = new Dictionary<string, string>();
        }

        private class Key
        {
            [JsonPropertyName("kty")]
            public string Kty { get; set; }

            [JsonPropertyName("use")]
            public string Use { get; set; }

            [JsonPropertyName("kid")]
            public string Kid { get; set; }

            [JsonPropertyName("x5t")]
            public string X5t { get; set; }

            [JsonPropertyName("n")]
            public string N { get; set; }

            [JsonPropertyName("e")]
            public string E { get; set; }

            [JsonPropertyName("x5c")]
            public List<string> X5c { get; set; }

            [JsonPropertyName("issuer")]
            public string Issuer { get; set; }
        }

        private class KeyCollection
        {
            [JsonPropertyName("keys")]
            public List<Key> Keys { get; set; }
        }
    }
}