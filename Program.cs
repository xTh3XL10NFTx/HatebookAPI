global using Microsoft.EntityFrameworkCore;
global using Hatebook.Data;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Identity;
global using Hatebook.Services;
global using Hatebook.Common;
global using Hatebook.Models;
using Hatebook.Configurations;
using Microsoft.OpenApi.Models;
using Hatebook.Controllers;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));
builder.Services.ConfigureJWT(configuration);

builder.Services.AddAutoMapper(typeof(MapperInitializer));
builder.Services.AddScoped<IAuthManager, AuthManager>();
builder.Services.AddScoped<IControllerConstructor, ControllerConstructor>();
builder.Services.AddScoped<AccountController>();
builder.Services.AddScoped<AccountServices>();
builder.Services.AddScoped<GroupServices>();
builder.Services.ConfigureIdentity();

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
            Scheme = "Bearer",
            BearerFormat = "JWT"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement() {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference 
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new List<string>()
            }
        });

        c.SwaggerDoc("v1", new OpenApiInfo { Title = "HatebookAPI", Version = "v9 000" });
    });
}

//builder.Services.AddIdentity<IdentityUser, IdentityRole>()
//    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
