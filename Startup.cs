using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NYCTaxiTrips.Data;

namespace NYCTaxiTrips
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => {
                options.AddPolicy("AllowOrigin",builder => {
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod(); 
                });
            });
            services.AddControllers();
            services.AddScoped<IGoogeBigQueryRepo>(sp => new GoogleBigQueryRepo(sp.GetService<IWebHostEnvironment>(),"onyx-etching-309802", "onyx-etching-309802-022a4d77abf0.json", "tlc_yellow_trips_2015"));
            services.AddScoped<INYCTaxiTripsRepo>(sp=> new BigQueryNYCTaxiTripsRepo(sp.GetService<IGoogeBigQueryRepo>(),3));
            /*
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "NYCTaxiTrips", Version = "v1" });
            });
            */
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            /*
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NYCTaxiTrips v1"));
            }
            */

            

            app.UseRouting();

            //app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().WithHeaders("authorization", "accept", "content-type", "origin"));
            app.UseCors("AllowOrigin");
            app.UseFileServer(new FileServerOptions{
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Credentials")), 
                    RequestPath = "",
                    EnableDefaultFiles = true
            });

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            
        }
    }
}
