var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<IFileTailService, FileTailService>();
builder.Services.Configure<LogOptions>(builder.Configuration.GetSection(nameof(LogOptions)));
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

var logService = app.Services.GetRequiredService<IFileTailService>();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("api/IrcWidget", logService.Get);
});

app.Run();
