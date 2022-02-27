var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<IFileTailService, FileTailService>();
builder.Services.Configure<LogOptions>(builder.Configuration.GetSection(nameof(LogOptions)));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

var fileTailService = app.Services.GetRequiredService<IFileTailService>();

app.MapGet("api/IrcWidget", fileTailService.Get);
app.Run();
