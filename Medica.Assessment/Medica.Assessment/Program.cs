using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

Serilog.Log.Logger = new LoggerConfiguration()
    .WriteTo.File("Medica.Assessment.CustomerLog.txt")
    .CreateLogger();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

