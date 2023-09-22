using MagicVillaApi.Data;
using MagicVillaApi.Services.Implements;
using MagicVillaApi.Services.Intefaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var confi = builder.Configuration;
builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IVillaService, VillaService>();
builder.Services.AddDbContext<ApplicationDbContext>( options =>
{
    options.UseSqlServer(confi.GetConnectionString("SQL"));
} );

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
