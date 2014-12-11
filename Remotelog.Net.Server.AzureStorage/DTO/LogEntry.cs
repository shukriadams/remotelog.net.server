using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Remotelog.Net.Server.AzureStorage
{
    public class LogEntry : TableEntity
    {
        public LogEntry()
        {
            this.PartitionKey = string.Empty;
            this.RowKey = Guid.NewGuid().ToString();
        }
        
        public string Text { get; set; }
        
        public string Type { get; set; }

        public string Person { get; set; }

        public long Date { get; set; }
    }
}
