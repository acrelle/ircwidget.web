namespace IrcWidget.Web.Services;

public interface IFileTailService
{
    [HttpGet]
    IResult Get();
}
