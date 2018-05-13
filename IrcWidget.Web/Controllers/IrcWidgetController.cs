using System;
using System.Collections.Generic;
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

        public IrcWidgetController(IConfiguration configuration, ILogger<IrcWidgetController> logger)
        {
            Configuration = configuration;
            Logger = logger;

            // Error checking.
            if (String.IsNullOrEmpty(LogFile))
                Logger.LogError("No username has been supplied - ensure the environmental variables have been set if necessary (inspect the dockerfile).");

        }

        //private String LogFile => Config
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            try
            {
                List<string> text = System.IO.File.ReadAllLines(LogFile).Reverse().Take(30).ToList();
                return text;
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
