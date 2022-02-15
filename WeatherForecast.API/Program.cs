using Microsoft.EntityFrameworkCore;
using WeatherForecast.API.BLL.Services.Concretes;
using WeatherForecast.API.BLL.Services.Interfaces;
using WeatherForecast.API.DAL;
using WeatherForecast.API.DAL.Repositories.Concretes;
using WeatherForecast.API.DAL.Repositories.Interfaces;
using WeatherForecast.API.Helpers.Constants;
using WeatherForecast.API.Middlewares;

var myAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
                      b =>
                      {
                          b.WithOrigins(builder.Configuration[SettingKeys.AccuWeatherUIUrl])
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                      });
});

// Add services to the container.
builder.Services.AddHttpClient();

builder.Services.AddTransient<IIPService, IPService>();
builder.Services.AddTransient<IForecastService, ForecastService>();
builder.Services.AddTransient<ILocationService, LocationService>();
builder.Services.AddTransient<IAccuWeatherHttpRequestService, AccuWeatherHttpRequestService>();
builder.Services.AddTransient<ILocationForecastRepository, LocationForecastRepository>();

builder.Services.AddDbContext<ApplicationDBContext>(options
    => options.UseSqlite(builder.Configuration.GetConnectionString(SettingKeys.TestTaskDBConnectionString)));

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<CustomExceptionHandlerMiddleware>();

app.UseCors(myAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();
