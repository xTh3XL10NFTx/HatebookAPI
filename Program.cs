global using Microsoft.EntityFrameworkCore;
global using Hatebook.Data;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Identity;
using Hatebook;
using Hatebook.Configurations;
using Hatebook.Services;
using FluentAssertions.Common;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

builder.Services.AddAuthentication();
builder.Services.ConfigureIdentity();
builder.Services.ConfigureJWT(configuration);

builder.Services.AddAutoMapper(typeof(MapperInitializer));
builder.Services.AddScoped<IAuthManager, AuthManager>();

builder.Services.AddEndpointsApiExplorer();

AddSwaggerDoc(builder.Services);

void AddSwaggerDoc(IServiceCollection services)
{
    builder.Services.AddSwaggerGen(c =>
    {
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = @"JWT Authorization header using the Bearer scheme.
            Enter 'Bearer' [space] and then your token in the next input below.
            Example: 'Bearer 12345abcdef'",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement() {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme ="0auth2",
                    Name="Bearer",
                    In=ParameterLocation.Header,

                },
                new List<string>()
            }
        });

        c.SwaggerDoc("v1", new OpenApiInfo { Title = "HotelListing", Version = "v1" });
    });
}

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();
