using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml;

namespace Remotelog.Net.Server
{
    public class LogConfigHandler : IConfigurationSectionHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="configContext"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        object IConfigurationSectionHandler.Create(object parent, object configContext, XmlNode section)
        {
            List<LogConfig> items = new List<LogConfig>();

            foreach (XmlNode node in section.ChildNodes)
            {
                string name;
                XmlNode storeConfig;
                bool isEnabled = true;
                int maxLogSize = 1000;
                DateTime? logUntil = null;
                IEnumerable<string> origins = new List<string>();

                if (node.Attributes == null)
                    throw new ConfigurationErrorsException("Required attributes missing.");

                if (node.Attributes["name"] == null || node.Attributes["name"].Value.Length == 0)
                    throw new ConfigurationErrorsException("Required attribute 'name' missing from config item.");
                name = node.Attributes["name"].Value;

                // try parse optional contents
                if (node.Attributes["enabled"] != null && node.Attributes["enabled"].Value.Length > 0)
                {
                    string raw = node.Attributes["enabled"].Value;
                    if (!bool.TryParse(raw, out isEnabled))
                        throw new ConfigurationErrorsException(string.Format("'{0}' is not a valid value for 'enabled' (boolean expected).", raw));
                }

                if (node.Attributes["maxsize"] != null && node.Attributes["maxsize"].Value.Length > 0)
                {
                    string raw = node.Attributes["maxsize"].Value;
                    if (!int.TryParse(raw, out maxLogSize))
                        throw new ConfigurationErrorsException(string.Format("'{0}' is not a valid value for 'maxsize' (int expected).", raw));
                }

                if (node.Attributes["origins"] != null && node.Attributes["origins"].Value.Length > 0)
                {
                    origins = node.Attributes["origins"].Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                }


                if (node.ChildNodes.Count == 0 || node.ChildNodes.Count > 1 || node.ChildNodes[0].Name != "store")
                    throw new Exception("A logToApi node must contain one store child node");
                storeConfig = node.ChildNodes[0];

                if (node.Attributes["expire"] != null && node.Attributes["expire"].Value.Length > 0)
                {
                    string raw = node.Attributes["expire"].Value;
                    if (raw.Length != 8)
                        throw new ConfigurationErrorsException(string.Format("'{0}' is not a valid value for 'expire' (ISO data YYYYMMDDD expected).", raw));
                    
                    string rawYear = raw.Substring(0, 4);
                    string rawMonth = raw.Substring(4, 2);
                    string rawDay = raw.Substring(6, 2);

                    int year;
                    int month;
                    int day;
                    int.TryParse(rawYear, out year);
                    int.TryParse(rawMonth, out month);
                    int.TryParse(rawDay, out day);

                    try
                    {
                        logUntil = new DateTime(year, month, day);
                    }
                    catch (FormatException ex) 
                    {
                        throw new ConfigurationErrorsException(string.Format("'{0}-{1}-{2}' did not produce a valid date' (ISO data YYYYMMDDD expected).", rawYear, rawMonth, rawDay), ex);
                    }
                }

                if (items.Any(r => r.Name == name))
                    throw new Exception(string.Format("Log name {0} is defined more than once. Name must be unique.", name));

                items.Add(new LogConfig
                { 
                    Name = name,
                    IsEnabled = isEnabled,
                    LogUntil = logUntil,
                    MaxLogSize = maxLogSize,
                    Origins = origins,
                    StoreConfig = storeConfig
                });
            }

            return items;
        }
    }
}
