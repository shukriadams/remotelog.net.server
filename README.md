Remotelog.Server.Net
====================
A log that can be written to via a GET call to an API. Comes as Web API controller that can be dropped in any host application.


Supports
--------
- multiple persistence backends (currently only AzureStorage is implemented)
- multiple logs per application instance
- logs can be created in web.config
- logs can be set to expire on a date
- logs can have a max size
- logs can limit calling origin


Run using the included Web API site
-----------------------------------
- Build in Visual Studio or use the MSbuild file \Remotelog.Net.Host\Remotelog.Net.Host.build.release.bat.


Settings
--------
The Web.config section for remotelog looks like. 
- <remoteLog> contain multiple <log> children
- <log> should contain one <store> child. Store child contains whatever config info is expected the persistence layer. Currently only Azurestorage is supported.
- log.name is mandatory and must be unique. Your log calls to the API must include this value.
- log.origins is an optional, comma-separated list of ips or host names to restrict logging to. 
- log.expire is optional. It is an ISO date for the cut-off date after which logging calls will be ignored.
- log.maxsize is an optional in for the maximum number of entries the log can have. Once exceeded oldest entries will deleted until max size is reached.
- store.table is the Azure storage table name to write logs to.
- store.connectionString is the Azure storage connection string.

  <remotelog>
    <log name="AUniqueLogName" origins="someDomain,anotherDomain" expire="20151226" maxsize="10">
      <store table="testTable" connectionString="UseDevelopmentStorage=true" />  
    </log>
    <log name="AnotherLogName">
      <store table="anotherTestTable" connectionString="UseDevelopmentStorage=true" />
    </log>
  </remotelog>
