using Microsoft.EntityFrameworkCore;
using StudentMarksApi.Data;

try
{
    Console.WriteLine("First Console printed");
    var builder = WebApplication.CreateBuilder(args);

    // Controllers
    builder.Services.AddControllers();

    /*
     *
     For Support only PostgreSQL DbContext
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection")
        )
    );*/

    var databaseProvider = builder.Configuration["DatabaseProvoider"];
    Console.WriteLine($"DatabaseProvoider:{databaseProvider}");


    var SqlConnectionString = builder.Configuration.GetConnectionString("SqlServer");
    Console.WriteLine($"Sql Server connection string exists:{!string.IsNullOrEmpty(SqlConnectionString)}");
    builder.Services.AddDbContext<AppDbContext>(options =>
    {

        if (databaseProvider == "PostgreSQL")
        {
            Console.WriteLine("Using PostGre SERVER");
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("PostgreSQL")
            );
        }
        else
        {
            Console.WriteLine("Using SQL SERVER");
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("SqlServer")
            );
        }

    });

    /*
    // CORS for only Local Angular Project
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngular", policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });*/


    /*
    CORS for only Local Angular and React

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AngularApp", policy =>
        {
            policy.WithOrigins("http://localhost:4200", "http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });*/

    // CORS For Server react
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

    app.UseHttpsRedirection();


    app.UseCors("ReactApp");
    Console.WriteLine("CORS configuered");
    //app.UseCors("AllowAngular");
    //app.UseCors("AngularApp");
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"FATAL ERROR: {ex.Message}");
    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
    throw;
}