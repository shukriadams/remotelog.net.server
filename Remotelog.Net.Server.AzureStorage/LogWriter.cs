using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using core = Remotelog.Net.Server;

namespace Remotelog.Net.Server.AzureStorage
{
    public class LogWriter: ILogWriter
    {
        public void Initialize()
        {
            IEnumerable<LogConfig> configs = LogConfig.GetConfigs();
            if (configs == null)
                throw new Exception("No logs have not be set up yet.");


            foreach (LogConfig config in configs)
            {
                AzureConfig log = AzureConfig.Load(config);
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(log.ConnectionString);
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                CloudTable table = tableClient.GetTableReference(log.TableName);
                table.CreateIfNotExists();
            }

        }

        void ILogWriter.EnforeSizeQuota(LogConfig logConfig) 
        {
            AzureConfig config = AzureConfig.Load(logConfig);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(config.ConnectionString);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference(config.TableName);
            var query = from entity in table.CreateQuery<LogEntry>() 
                        select entity;

            // todo : horrible tolist() here was necessary to fix a casting problem, replace this.
            var  overflow = query.ToList().OrderByDescending(r => r.Date).Skip(logConfig.MaxLogSize); 
            foreach (LogEntry entry in overflow){
                TableOperation deleteOperation = TableOperation.Delete(entry);
                table.Execute(deleteOperation);
            }

        }

        void ILogWriter.Write(LogConfig logConfig, core.LogEntry entry)
        {
            AzureConfig config = AzureConfig.Load(logConfig);

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(config.ConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(config.TableName);

            LogEntry item = new LogEntry
            {
                Text = entry.Text,
                Person = entry.Person,
                Type = entry.Type,
                Date = entry.Date
            };

            TableOperation insertOperation = TableOperation.Insert(item);
            table.Execute(insertOperation);
        }
    }
}
