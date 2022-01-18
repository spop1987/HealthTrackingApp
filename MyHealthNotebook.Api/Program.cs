using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MyHealthNotebook.DataService.Data;
using MyHealthNotebook.DataService.IConfiguration;
using MyHealthNotebook.Entities.Translators;
using MyHealthNoteBook.DataService.Data;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IToDtoTranslator, ToDtoTranslator>();
builder.Services.AddScoped<IToEntityTranslator, ToEntityTranslator>();

builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo{Title = "MyHealthNotebook.Api", Version = "v1"});
});

builder.Services.AddApiVersioning(opt => {
    // Provide to the client the different Api versions that we have.
    opt.ReportApiVersions = true;
    // In case the version is not provide, the api provide the default version
    opt.AssumeDefaultVersionWhenUnspecified = true;

    opt.DefaultApiVersion = ApiVersion.Default;
});

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
