using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Luval.Common
{
    public static class ConfigHelper
    {
        private static Dictionary<Type, IConfiguration> _configs = new Dictionary<Type, IConfiguration>() { { typeof(ConfigManagerWrapper), new ConfigManagerWrapper() } };
        private static ObjectCache<string, IConfiguration> _cacheConfig = new ObjectCache<string, IConfiguration>();
        private static ObjectCache<string, string> _cacheValue = new ObjectCache<string, string>();

        public static void RegisterProvider(IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration", "value cannot be null");
            if (_configs.ContainsKey(configuration.GetType())) return;
            _configs.Add(configuration.GetType(), configuration);
        }

        /// <summary>
        /// Gets a configuration value from one of the registered providers
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <param name="cacheValue">Indicates if the value should be cached in memory once found to minimize the number of requests to the config provider</param>
        /// <returns>Configuration value</returns>
        public static string Get(string key, bool cacheValue)
        {
            if (cacheValue) return _cacheValue.Get(key, () => { return Get(key); });
            return Get(key);
        }

        /// <summary>
        /// Gets a configuration value from one of the registered providers
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <returns>Configuration value</returns>
        public static string Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException("Parameter cannot be null or empty");
            return _cacheConfig.Get(key, () => {
                return GetValue(key);
            })[key];
        }

        private static IConfiguration GetValue(string key)
        {
            var exs = new List<Exception>();
            foreach (var item in _configs)
            {
                string value = null;
                try
                {
                    value = item.Value[key];
                    if (!string.IsNullOrWhiteSpace(value)) return item.Value;
                }
                catch (Exception ex)
                {
                    exs.Add(new InvalidOperationException("Unable to get value for type {0}".Format(item.Key), ex));
                }
            }
            if (exs.Any()) throw new ArgumentException("Unable to locate the setting\n {0}".Format(string.Join("\n", exs.Select(i => i.ToString()))));
            return new EmptyConfiguration();
        }
    }

    public class ConfigManagerWrapper : IConfiguration
    {
        public string this[string key] { get { return Get(key); } set { ConfigurationManager.AppSettings[key] = value; } }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            throw new NotImplementedException();
        }

        public IChangeToken GetReloadToken()
        {
            throw new NotImplementedException();
        }

        public IConfigurationSection GetSection(string key)
        {
            throw new NotImplementedException();
        }

        private string Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException("Parameter cannot be null or empty");
            string value = null;
            try
            {
                value = ConfigurationManager.AppSettings[key];
                if (string.IsNullOrWhiteSpace(value) && ConfigurationManager.ConnectionStrings[key] != null) value = ConfigurationManager.ConnectionStrings[key].ConnectionString;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Unable to retrieve {0} from the Configuration Manager domain", ex);
            }
            return value;
        }
    }

    public class EmptyConfiguration : IConfiguration
    {
        public string this[string key] { get => null; set => throw new NotImplementedException(); }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            throw new NotImplementedException();
        }

        public IChangeToken GetReloadToken()
        {
            throw new NotImplementedException();
        }

        public IConfigurationSection GetSection(string key)
        {
            throw new NotImplementedException();
        }
    }
}
