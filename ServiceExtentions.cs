using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

namespace Hatebook
{
    public static class ServiceExtentions
    {
        public static void ConfigureIdentity(this IServiceCollection service)
        {
            var builder = service.AddIdentityCore<DbIdentityExtention>(q => q.User.RequireUniqueEmail = true);

            //builder = new IdentityBuilder(DbIdentityExtention, typeof(IdentityRole), service);
            builder.AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
              //  builder.AddSignInManager<SignInManager<DbIdentityExtention>>();
        }

        public static void ConfigureJWT(this IServiceCollection service, IConfiguration Configuration)
        {
            // Retrieve the secret from appsettings.json
        //    var secret = Configuration["Jwt:Key"];

            var jwtSettings = Configuration.GetSection("Jwt");

            service.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(o =>
                {
                    o.Events = new JwtBearerEvents()
                    {
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";

                            // Ensure we always have an error and error description.
                            if (string.IsNullOrEmpty(context.Error))
                                context.Error = "invalid_token";
                            if (string.IsNullOrEmpty(context.ErrorDescription))
                                context.ErrorDescription = "This request requires a valid JWT access token to be provided";

                            // Add some extra context for expired tokens.
                            if (context.AuthenticateFailure != null && context.AuthenticateFailure.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                var authenticationException = context.AuthenticateFailure as SecurityTokenExpiredException;
                                context.Response.Headers.Add("x-token-expired", authenticationException.Expires.ToString("o"));
                                context.ErrorDescription = $"The token expired on {authenticationException.Expires.ToString("o")}";
                            }

                            return context.Response.WriteAsync(JsonSerializer.Serialize(new
                            {
                                error = context.Error,
                                error_description = context.ErrorDescription
                            }));
                        }
                    };
                    o.IncludeErrorDetails = true;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("secretkeysecretkeysecretkeysecretkeysecretkey"))
                    };
                });
        }
    }
}
