using System.Text.Json.Serialization;
using FluentValidation;
using LeedsBeerQuest;
using LeedsBeerQuest.Data;
using LeedsBeerQuest.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IValidator<VenueQuery>, VenueQueryValidator>();
builder.Services.AddSingleton<IVenueRepository, VenueRepository>();
builder.Services.AddSingleton<IVenueRawData, VenueRawData>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
