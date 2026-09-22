using System.Text.Json.Serialization;
using AnimalMart.Interfaces;
using AnimalMart.Repos;
using AnimalMart.Services;
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

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder
                    //allows any domain or site to access API
                    .AllowAnyOrigin()
                    //allows any HTTP method when accessing API (GET, POST, PUT, DELETE, etc.)
                    .AllowAnyMethod()
                    //allows any HTTP header to be included in the request when accessing API
                    .AllowAnyHeader();
                });
            });

            var app = builder.Build();
            

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

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.MapControllers();
            app.Run();
        }
    }
}
