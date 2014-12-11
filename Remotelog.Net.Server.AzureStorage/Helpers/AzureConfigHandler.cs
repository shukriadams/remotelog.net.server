using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace Remotelog.Net.Server.AzureStorage
{
    public class AzureConfigHandler : IConfigurationSectionHandler
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
            List<AzureConfig> items = new List<AzureConfig>();

            foreach (XmlNode node in section.ChildNodes)
            {
                XmlNode storeNode = null;
                if (node.ChildNodes.Count == 0 || node.ChildNodes.Count > 1 || node.ChildNodes[0].Name != "store")
                    throw new Exception("A logToApi node must contain one store child node");

                storeNode = node.ChildNodes[0];
                string connectionString;
                string name;
                string tableName;

                if (storeNode.Attributes == null)
                    throw new ConfigurationErrorsException("Required attributes missing.");

                if (storeNode.Attributes["connectionString"] == null || storeNode.Attributes["connectionString"].Value.Length == 0)
                    throw new ConfigurationErrorsException("Required attribute 'connectionString' missing from config item.");
                connectionString = storeNode.Attributes["connectionString"].Value;

                if (storeNode.Attributes["table"] == null || storeNode.Attributes["table"].Value.Length == 0)
                    throw new ConfigurationErrorsException("Required attribute 'table' missing from config item.");
                tableName = storeNode.Attributes["table"].Value;

                if (node.Attributes["name"] == null || node.Attributes["name"].Value.Length == 0)
                    throw new ConfigurationErrorsException("Required attribute 'name' missing from config item.");
                name = node.Attributes["name"].Value;

                items.Add(new AzureConfig
                {
                    ConnectionString = connectionString,
                    Name = name,
                    TableName = tableName
                });
            }

            return items;
        }
    }
}
