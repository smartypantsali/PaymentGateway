using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Framework.Configuration
{
    /// <summary>
    /// PaymentGateway global configuration data, gets from appsettings.json on Startup.cs
    /// </summary>
    public static class PGConfiguration
    {
        private static Dictionary<string, string> _keyValues = new Dictionary<string, string>();

        /// <summary>
        /// Set values by key
        /// </summary>
        /// <param name="configs"></param>
        public static void SetKeyValues(IEnumerable<IConfigurationSection> configs)
        {
            foreach (var config in configs)
            {
                _keyValues[config.Key] = config.Value;
            }
        }

        /// <summary>
        /// Attempt to find value of key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string TryGetValue(string key)
        {
            return _keyValues[key];
        }
    }
}
