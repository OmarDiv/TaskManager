using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Globalization;

namespace TaskManager.Application.Common.Localizations
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        private readonly IDistributedCache _cache;
        private readonly Newtonsoft.Json.JsonSerializer _jsonSerializer = new();
        private readonly ConcurrentDictionary<string, Dictionary<string, string>> _resourceCache = new();
        public JsonStringLocalizer(IDistributedCache cache)
        {
            _cache = cache;
        }
        public LocalizedString this[string name]
        {
            get
            {
                var Value = GetString(name);
                return new LocalizedString(name, Value);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var ActualValue = this[name];
                return !ActualValue.ResourceNotFound
                    ? new LocalizedString(name, string.Format(ActualValue.Value, arguments))
                     : ActualValue;
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var FilePath = $"Resources/{CultureInfo.CurrentCulture.Name}.json";
            using FileStream stream = new(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using StreamReader streamreader = new(stream);
            using JsonTextReader Reader = new(streamreader);
            while (Reader.Read())
            {
                if (Reader.TokenType != JsonToken.PropertyName)
                    continue;
                var Key = Reader.Value as string;
                Reader.Read();
                var value = _jsonSerializer.Deserialize<string>(Reader);
                yield return new LocalizedString(Key, value);
            }
        }
        private string ResolveFilePath(string culture)
        {
            var filePath = Path.GetFullPath($"Resources/{culture}.json");
            if (File.Exists(filePath)) return filePath;
            
            // Fallback to default
            var defaultPath = Path.GetFullPath("Resources/ar.json");
            return File.Exists(defaultPath) ? defaultPath : string.Empty;
        }

        private string GetString(string Key)
        {
            var resources = LoadResources(CultureInfo.CurrentCulture.Name);
            if (resources.TryGetValue(Key, out var value))
            {
                return value;
            }
            return Key;
        }

        private Dictionary<string, string> LoadResources(string culture)
        {
            return _resourceCache.GetOrAdd(culture, key =>
            {
                var filePath = ResolveFilePath(key);
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) 
                    return new Dictionary<string, string>();

                var json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(json)
                       ?? new Dictionary<string, string>();
            });
        }

    }
}
