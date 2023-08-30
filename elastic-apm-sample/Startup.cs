using System;
using System.Threading.Tasks;
using Elastic.Apm.Api;
using Elastic.Apm.NetCoreAll;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace elastic_apm_sample
{
    public class Startup

    {

        private readonly IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }


        private async Task SayHelloToTomo(HttpContext context)
        {


            var transaction = Elastic.Apm.Agent
                     .Tracer.StartTransaction("MyTransaction", ApiConstants.TypeRequest);

            try
            {
                //application code that is captured as a transaction
                await context.Response.WriteAsync("Hello tomo!");
            }
            catch (Exception e)
            {
                transaction.CaptureException(e);
                throw;
            }
            finally
            {
                transaction.End();
            }


            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAllElasticApm(Configuration);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });

                endpoints.MapGet("/tomo", SayHelloToTomo);
            });
        }


    }
}
