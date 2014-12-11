using System;
using System.Configuration;

namespace Remotelog.Net.Server.AzureStorage
{
    public class AzureConfig
    {
        public static AzureConfig Load(LogConfig logConfig)
        {
            if (logConfig.StoreConfig == null)
                throw new Exception("Storeconfig of logoconfig is null");

            string connectionString;
            string tableName;

            if (logConfig.StoreConfig.Attributes == null)
                throw new ConfigurationErrorsException("Required attributes missing.");

            if (logConfig.StoreConfig.Attributes["connectionString"] == null || logConfig.StoreConfig.Attributes["connectionString"].Value.Length == 0)
                throw new ConfigurationErrorsException("Required attribute 'connectionString' missing from config item.");
            connectionString = logConfig.StoreConfig.Attributes["connectionString"].Value;

            if (logConfig.StoreConfig.Attributes["table"] == null || logConfig.StoreConfig.Attributes["table"].Value.Length == 0)
                throw new ConfigurationErrorsException("Required attribute 'table' missing from config item.");
            tableName = logConfig.StoreConfig.Attributes["table"].Value;

            return new AzureConfig
            {
                ConnectionString = connectionString,
                Name = logConfig.Name,
                TableName = tableName
            };
            
        }


        public string Name { get; set; }
        public string TableName { get; set; }
        public string ConnectionString { get; set; }
    }
}
