using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

namespace The9Books
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            
            // Configure strongly-typed settings
            services.Configure<Models.ApiSettings>(
                Configuration.GetSection("ApiSettings"));

            // Add Swagger/OpenAPI
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "The 9 Books API",
                    Version = "v1",
                    Description = "An API to retrieve hadith from nine famous books",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "The 9 Books API"
                    }
                });
            });

            // Add CORS
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            // Add Health Checks
            services.AddHealthChecks()
                .AddDbContextCheck<SQLiteDBContext>();

            services.AddDbContext<SQLiteDBContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=SunnahDb.db"));
            services.AddScoped<IDBContext>(provider => provider.GetRequiredService<SQLiteDBContext>());
            services.AddSingleton<IRandom, RandomGenerator>();

            // Add Response Caching
            services.AddResponseCaching();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            // Add error handling middleware after routing but before endpoints
            app.UseMiddleware<Middleware.ErrorHandlingMiddleware>();

            // Enable Swagger UI at /swagger route
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "The 9 Books API v1");
                c.RoutePrefix = "swagger"; // Set Swagger UI at /swagger
            });

            app.UseResponseCaching();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}