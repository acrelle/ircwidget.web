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
    /// <summary>
    ///  Provides the last 30 lines of the configured file through a API interface.
    /// </summary>
    [Route("api/[controller]")]
    public class IrcWidgetController : Controller
    {
        private IConfiguration Configuration { get; }
        private ILogger Logger { get; }

        // Reduce the lines of code and re-evaluate on each call.
        private string LogFile => Configuration["logfile"];
        private int BufferLength => ((Int32.TryParse(Configuration["bufferlength"], out int result)) ? result : 10000);
        private int LineCount => ((Int32.TryParse(Configuration["linecount"], out int result)) ? result : 30);


        public IrcWidgetController(IConfiguration configuration, ILogger<IrcWidgetController> logger)
        {
            Configuration = configuration;
            Logger = logger;

            // This is most likely a fatal error.
            if (String.IsNullOrEmpty(LogFile))
                Logger.LogError("No logfile has been supplied - ensure the environmental variables have been set if necessary (inspect the dockerfile).");

            Logger.LogDebug("Started with configuration of LogFile: {LogFile}, BufferLength: {BufferLength}, LineCount: {LineCount}", LogFile, BufferLength, LineCount);
        }

        // GET api/ircwidget
        [HttpGet]
        public IEnumerable<string> Get()
        {
            try
            {
                using (FileStream fs = new FileStream(LogFile, FileMode.Open, FileAccess.Read))
                {
                    // Rather than ReadAllLines(), seek to near the end of the file and 
                    // just read the last 10,000 bytes. This should be enough to establish 
                    // the last 30 lines.

                    // Both BufferLength and LineCount can be override by environmental variables 
                    // when running the Docker image.

                    fs.Seek(Math.Max(-BufferLength, -fs.Length), SeekOrigin.End);
                    var s = new StreamReader(fs);
                    
                    return s.ReadToEnd().Split('\n').Reverse().Take(LineCount).Reverse();
                }
               
            }
            catch (Exception e)
            {
                // Fatal error, give up.
                Logger.LogError(e, "Failure while reading file: {file}", LogFile);
                throw e;
            }           
        }

        // GET api/ircwidget/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "not supported";
        }

        // POST api/ircwidget
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/ircwidget/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/ircwidget/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
