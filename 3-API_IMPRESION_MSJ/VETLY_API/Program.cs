using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using VETLY_DAL.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using System.Collections.Generic;
using Serilog;
using Serilog.Events;

public class Program
{
    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        // Añade servicios a la colección
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("NuevaPolitica", app =>
            {
                app.AllowAnyOrigin();
                app.AllowAnyMethod();
                app.AllowAnyHeader();
            });
        });

        // Add your UnitOfWork registration here
        builder.Services.AddScoped<UnitOfWork>();

        // Configura la autenticación JWT
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Authority = "https://dev-wy6iwpuipnbg51yz.us.auth0.com"; // URL de Auth0
            options.Audience = "https://dev-wy6iwpuipnbg51yz.us.auth0.com/api/v2/"; // Define la audiencia de tu aplicación

            // Descargar y configurar la clave pública desde el JWKS
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
                {
                    var jwksUrl = $"{options.Authority}/.well-known/jwks.json";
                    var httpClient = new HttpClient();
                    var response = httpClient.GetStringAsync(jwksUrl).Result;
                    var jwks = JsonConvert.DeserializeObject<Jwks>(response);

                    var rsa = new RSACryptoServiceProvider();
                    rsa.ImportParameters(
                        new RSAParameters()
                        {
                            Modulus = Base64UrlEncoder.DecodeBytes(jwks.Keys[0].N),
                            Exponent = Base64UrlEncoder.DecodeBytes(jwks.Keys[0].E)
                        });

                    return new List<SecurityKey> { new RsaSecurityKey(rsa) };
                },
                ValidateIssuer = true,
                ValidIssuer = "https://dev-wy6iwpuipnbg51yz.us.auth0.com", // URL de Auth0
                ValidateAudience = true,
                ValidAudience = "https://dev-wy6iwpuipnbg51yz.us.auth0.com/api/v2/", // Define la audiencia de tu aplicación
                ClockSkew = TimeSpan.Zero
            };
        });

        var app = builder.Build();

        // Configura el middleware
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors("NuevaPolitica");
        app.UseRouting();
        app.UseAuthentication(); // Habilita la autenticación JWT
        app.UseAuthorization(); // Asegúrate de que esté en esta ubicación

        app.MapControllers();

        app.Run();
    }
}

public class Jwks
{
    public List<Jwk> Keys { get; set; }
}

public class Jwk
{
    public string Kty { get; set; }
    public string Use { get; set; }
    public string Kid { get; set; }
    public string N { get; set; }
    public string E { get; set; }
}
