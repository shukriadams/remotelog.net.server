using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json.Linq;

namespace Remotelog.Net.Server
{
    public class LogController : ApiController
    {
        private ILogWriter _writer;
        public LogController(ILogWriter writer)
        {
            _writer = writer;
        }

        [AcceptVerbs("GET")]
        public JToken Write(string log, string text, string type = "", string person = "")
        {
            try
            {
                if (string.IsNullOrEmpty(log))
                    return JToken.FromObject(new { code = "1", message = "Log name is required." });

                if (string.IsNullOrEmpty(text))
                    return JToken.FromObject(new { code = "3", message = "Text empty, ignoring." });

                text = HttpContext.Current.Server.UrlDecode(text);

                LogConfig config = LogConfig.GetConfig(log);
                if (config== null)
                    return JToken.FromObject(new { code = "2", message = string.Format("Log {0} is not defined.", log) });
                if (DateTime.Now > config.LogUntil) 
                    return JToken.FromObject(new { code = "5", message = string.Format("Log has expired.", log) });

                //  enforce origin rules
                if (config.Origins.Any()) {

                    bool fail = !config.Origins.Contains(HttpContext.Current.Request.UserHostAddress) && !config.Origins.Contains(HttpContext.Current.Request.UserHostName);
                    
                    // finally, always allow localhost through
                    if (HttpContext.Current.Request.UserHostAddress == "::1" ||
                        HttpContext.Current.Request.UserHostName == "::1")
                        fail = false;
                    
                    if (fail)
                        return JToken.FromObject(new { code = "4", message = string.Format("Origin address {0}|name {1} not whitelisted.", HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.UserHostName) });
                }


                LogEntry entry = new LogEntry
                {
                    Person = person,
                    Text = text,
                    Type = type,
                    Date = DateTime.UtcNow.Ticks
                };

                _writer.Write(config, entry);

                string cachekey = config.Name + "_checkcount";
                int? writes = HttpContext.Current.Cache[cachekey] as int? ?? 0;
                // check at 0 incase app is unstable and cached value never gets a chance to reach max size
                if (writes == 0 || writes > config.MaxLogSize)
                {
                    _writer.EnforeSizeQuota(config);
                    writes = 0;
                }
                HttpContext.Current.Cache[cachekey] = writes + 1;

                return JToken.FromObject(new { code = "0", message = "Item logged." });
            }
            catch(Exception ex)
            {
                HttpContext.Current.Response.AppendToLog(string.Format("Unexpected error from {0} - {1} : {2}", this.GetType().FullName, MethodBase.GetCurrentMethod().Name, ex));
                return JToken.FromObject(new { code = "6", message = "An unexpected error occurred." });
            }
        }
	}
}