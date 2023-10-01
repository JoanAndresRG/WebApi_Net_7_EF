using MagicVillaApi.Data;
using MagicVillaApi.Repository.Implements;
using MagicVillaApi.Repository.Interfaces;
using MagicVillaApi.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var config = builder.Configuration;
builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MagicVillaSecurity", Version = "v1" });

    // Agregar la definición de seguridad para Bearer Token
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Autorización JWT esquema. \r\n\r\n Escribe  'Bearer', [espacio] e ingresa en token generado.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    };
    c.AddSecurityDefinition("Bearer", securityScheme);

    // Agregar el requerimiento de seguridad para Bearer Token

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                },
                new List<string>()
            }
        });
}
);
builder.Services.AddScoped<IVillaService, VillaService>();
builder.Services.AddScoped<INumberVillaService, NumberVillaService>();
builder.Services.AddScoped<IUserApiService, UserApiService>();
builder.Services.AddScoped<ILogginService, LogginService>();
builder.Services.AddScoped<IEncryptPasswordUser, EncryptPasswordUser>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(config.GetConnectionString("SQL"));
});
builder.Services.AddAutoMapper(typeof(MapperConfig));

//configura el servicio de autenticación para utilizar JWT Bearer Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = config["JWT:Issuer"],
        ValidAudience = config["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(config["JWT:ClaveSecreta"]))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("OrAdminOrUser", policy =>
    {
        policy.RequireRole("Admin", "User");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
