using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TesteADLogin.Data;
using TesteADLogin.Services;
using Microsoft.EntityFrameworkCore;
using TesteADLogin.Data;


namespace TesteADLogin
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
            services.AddScoped<AzureAdAuthService>();

            var dbConfig = Configuration.GetSection("DatabaseConfig").Get<DatabaseConfig>();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseMySql(dbConfig.ConnectionString,
                    ServerVersion.AutoDetect(dbConfig.ConnectionString), 
                    mysqlOptions =>
                    {
                        mysqlOptions.EnableRetryOnFailure(
                            maxRetryCount: dbConfig.MaxRetryCount,
                            maxRetryDelay: TimeSpan.FromSeconds(5),
                            errorNumbersToAdd: null);
                        mysqlOptions.CommandTimeout(dbConfig.CommandTimeout);
                        mysqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    });

                if (dbConfig.EnableSensitiveDataLogging)
                {
                    options.EnableSensitiveDataLogging();
                }
            });
            // Add this line here
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TesteADLogin", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TesteADLogin v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
