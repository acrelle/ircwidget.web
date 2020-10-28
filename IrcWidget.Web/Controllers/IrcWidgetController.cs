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
    ///  
    ///     At a minimum, use Secrets Manager to configure the LogFile variable.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class IrcWidgetController : Controller
    {
        private IConfiguration _configuration { get; }
        private ILogger<IrcWidgetController> _logger { get; }

        // Reduce the lines of code and re-evaluate on each call.
        private string LogFile => _configuration["logfile"];
        private int BufferLength => ((Int32.TryParse(_configuration["bufferlength"], out int result)) ? result : 10000);
        private int LineCount => ((Int32.TryParse(_configuration["linecount"], out int result)) ? result : 30);


        public IrcWidgetController(IConfiguration configuration, ILogger<IrcWidgetController> logger)
        {
            _configuration = configuration;
            _logger = logger;

            // This is most likely a fatal error.
            if (String.IsNullOrEmpty(LogFile))
            {
                _logger.LogError("No logfile has been supplied - ensure the environmental variables have been set if necessary (inspect the dockerfile).");
            }

            _logger.LogDebug("Started with configuration of LogFile: {LogFile}, BufferLength: {BufferLength}, LineCount: {LineCount}", LogFile, BufferLength, LineCount);
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
                _logger.LogError(e, "Failure while reading file: {file}", LogFile);
                throw e;
            }
        }
    }
}
