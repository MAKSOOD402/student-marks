using Microsoft.EntityFrameworkCore;
using StudentMarksApi.Data;

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

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (databaseProvider == "PostgreSQL")
    {
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("PostgreSQL")
        );
    }
    else
    {
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
        policy.WithOrigins("https://your-site.azurestaticapps.net")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});



var app = builder.Build();

app.UseHttpsRedirection();

//app.UseCors("AllowAngular");
//app.UseCors("AngularApp");

app.UseCors("ReactApp");

app.MapControllers();

app.Run();