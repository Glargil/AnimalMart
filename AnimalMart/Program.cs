using AnimalMart.Interfaces;
using AnimalMart.Repos;
using AnimalMart.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;
using System.Text.Json.Serialization;

namespace AnimalMart
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            builder.Services.AddSwaggerGen();

            //register repo/service
            builder.Services.AddScoped<IUserRepo, UserRepo>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IPasswordAnalyzer, PasswordAnalyzerService>();
            builder.Services.AddSingleton<ILoginAttemptTracker, LoginAttemptTracker>();

            //Password reset services
            builder.Services.AddScoped<IPasswordResetTokenRepo, PasswordResetTokenRepo>();
            builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();
            //emailsender temporary swap for SmtpEmailSender later
            builder.Services.AddScoped<IEmailSender, ConsoleEmailSender>();

            builder
                .Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Login";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SameSite = SameSiteMode.Strict;
                    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
                        ? CookieSecurePolicy.SameAsRequest
                        : CookieSecurePolicy.Always;
                    options.ExpireTimeSpan = TimeSpan.FromHours(4);
                    options.SlidingExpiration = true;
                });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowAll",
                    builder =>
                    {
                        builder
                            //allows any domain or site to access API
                            .AllowAnyOrigin()
                            //allows any HTTP method when accessing API (GET, POST, PUT, DELETE, etc.)
                            .AllowAnyMethod()
                            //allows any HTTP header to be included in the request when accessing API
                            .AllowAnyHeader();
                    }
                );
            });

            var app = builder.Build();

            var forwardedHeadersOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                    ForwardedHeaders.XForwardedProto,
                ForwardLimit = 1
            };
            if (!app.Environment.IsDevelopment())
            {
                forwardedHeadersOptions.KnownProxies.Add(IPAddress.Parse("172.18.0.2"));
            }
            app.UseForwardedHeaders(forwardedHeadersOptions);


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseSwagger();
            app.UseSwaggerUI();

            // Enable CORS
            app.UseCors("AllowAll");


            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages().WithStaticAssets();

            app.MapControllers();
            app.Run();
        }
    }
}
