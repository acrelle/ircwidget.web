using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IrcWidget.Web.Controllers
{
    [Route("api/[controller]")]
    public class IrcWidgetController : Controller
    {
        private IConfiguration Configuration { get; }
        private ILogger Logger { get; }
        private string LogFile => Configuration["logfile"];
        private int BufferLength => ((Int32.TryParse(Configuration["bufferlength"], out int result)) ? result : 10000);
        private int LineCount => ((Int32.TryParse(Configuration["linecount"], out int result)) ? result : 30);


        public IrcWidgetController(IConfiguration configuration, ILogger<IrcWidgetController> logger)
        {
            Configuration = configuration;
            Logger = logger;

            // Error checking.
            if (String.IsNullOrEmpty(LogFile))
                Logger.LogError("No logfile has been supplied - ensure the environmental variables have been set if necessary (inspect the dockerfile).");

        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            try
            {
                using (FileStream fs = new FileStream(LogFile, FileMode.Open, FileAccess.Read))
                {
                    // Rather than Read all lines, seek to near the end of the file and 
                    // just parse the last 10k.

                    fs.Seek(Math.Max(-BufferLength, -fs.Length), SeekOrigin.End);
                    var s = new StreamReader(fs);
                    
                    return s.ReadToEnd().Split('\n').Reverse().Take(LineCount).Reverse();
                }
               
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Unable to read file {file}", LogFile);
                throw e;
            }           
        }
        
        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "not supported";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
