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
using System.Data.SqlClient;

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


        private async Task SayHelloToAkito(HttpContext context)
        {


            var transaction = Elastic.Apm.Agent
                     .Tracer.StartTransaction("MyTransaction", ApiConstants.TypeRequest);

            try
            {
                //application code that is captured as a transaction
                await context.Response.WriteAsync("Hello Akito!");
                var just_wait = true;
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


        private async Task SelectSomething(HttpContext context)
        {




            //"Persist Security Info=False;User ID=*****;Password=*****;Initial Catalog=AdventureWorks;Server=MySqlServer;Encrypt=True;"


            string connectionString = "Server=localhost;Database=;User Id=sa;Password=Nzn6M_3M-X1s;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sqlQuery = "SELECT * FROM MySampleTable";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string firstName = reader.GetString(1);
                            string lastName = reader.GetString(2);
                            int age = reader.GetInt32(3);
                            string email = reader.GetString(4);

                            Console.WriteLine($"ID: {id}, FirstName: {firstName}, LastName: {lastName}, Age: {age}, Email: {email}");
                        }
                    }
                }
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
                endpoints.MapGet("/akito", SayHelloToAkito);
                endpoints.MapGet("/select", SelectSomething);
            });
        }


    }
}
