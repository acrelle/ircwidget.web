namespace IrcWidget.Web.Services;

/// <summary>
///  Provides the last 30 lines of the configured file through a API interface.
///  
///  At a minimum, use Secrets Manager to configure the LogFile variable.
/// </summary>
public class FileTailService : IFileTailService
{
    private readonly LogOptions _logOptions;
    private readonly ILogger<FileTailService> _logger;

    public FileTailService(IOptions<LogOptions> logOptions, ILogger<FileTailService> logger)
    {
        _logOptions = logOptions.Value;
        _logger = logger;

        // This is most likely a fatal error.
        if (string.IsNullOrEmpty(_logOptions.LogFile))
        {
            _logger.LogError("No logfile has been supplied - ensure the environmental variables have been set if necessary (inspect the dockerfile).");
        }

        _logger.LogDebug($"Started with configuration of LogFile: {_logOptions.LogFile}, BufferLength: {_logOptions.BufferLength}, LineCount: {_logOptions.LineCount}");
    }

    // GET api/ircwidget
    [HttpGet]
    public IResult Get()
    {
        try
        {
            using var fs = new FileStream(_logOptions.LogFile, FileMode.Open, FileAccess.Read);
            // Rather than ReadAllLines(), seek to near the end of the file and 
            // just read the last 10,000 bytes. This should be enough to establish 
            // the last 30 lines.

            // Both BufferLength and LineCount can be overriden by environmental variables 
            // when running the Docker image.

            fs.Seek(Math.Max(-_logOptions.BufferLength, -fs.Length), SeekOrigin.End);
            var s = new StreamReader(fs);

            return Results.Ok(s.ReadToEnd().Split('\n').Reverse().Take(_logOptions.LineCount).Reverse());

        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Failure while reading file: {_logOptions.LogFile}");
            return Results.NoContent();
        }
    }
}
