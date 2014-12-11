using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml;

namespace Remotelog.Net.Server
{
    public class LogConfig
    {
        /// <summary>
        /// The web.config section name our settings are place in.
        /// </summary>
        private const string ConfigSectionName = "remotelog";

        public static IEnumerable<LogConfig> GetConfigs()
        {
            return ConfigurationManager.GetSection(ConfigSectionName) as IEnumerable<LogConfig>;
        }

        public static LogConfig GetConfig(string name)
        {
            IEnumerable<LogConfig> configs = ConfigurationManager.GetSection(ConfigSectionName) as IEnumerable<LogConfig>;
            return configs.SingleOrDefault(r => r.Name == name);
        }

        public XmlNode StoreConfig { get; set; }

        /// <summary>
        /// Table name for logs. Default is "LogEntry". Optional
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Set to false to disable login. Default is true. Optional.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Max number of log entries. If exceeded older log entries will be deleted in blocks. 
        /// Default and maximum allowed value is 10000. Optional.
        /// </summary>
        public int MaxLogSize { get; set; }

        /// <summary>
        /// Comma-separated list of origin domains if logging must be origin limited. Optional.
        /// </summary>
        public IEnumerable<string> Origins { get; set; }

        /// <summary>
        /// Date at which to stop logging. Must be in iso format (YYYYMMDD). Optional.
        /// </summary>
        public DateTime? LogUntil { get; set; }
    }
}