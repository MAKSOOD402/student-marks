using Microsoft.EntityFrameworkCore;
using StudentMarksApi.Data;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add logging
    builder.Services.AddLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    });

    var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();

    logger.LogInformation("First Console printed");

    builder.Services.AddControllers();

    var databaseProvider = builder.Configuration["DatabaseProvider"];
    logger.LogInformation($"DatabaseProvider: {databaseProvider}");

    var SqlConnectionString = builder.Configuration.GetConnectionString("SqlServer");
    logger.LogInformation($"Sql Server connection string exists: {!string.IsNullOrEmpty(SqlConnectionString)}");

    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        if (databaseProvider == "PostgreSQL")
        {
            logger.LogInformation("Using PostgreSQL SERVER");
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("PostgreSQL")
            );
        }
        else
        {
            logger.LogInformation("Using SQL SERVER");
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("SqlServer")
            );
        }
    });

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("ReactApp", policy =>
        {
            policy.WithOrigins("https://lively-river-03fbf9310.6.azurestaticapps.net")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });

    var app = builder.Build();
    logger.LogInformation("Application building...");

    app.UseHttpsRedirection();
    app.UseCors("ReactApp");
    logger.LogInformation("CORS configured");
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"FATAL ERROR: {ex.Message}");
    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
    throw;
}