using BastetAPI.Repositories;
using BastetAPI.Settings;
using BastetFTMAPI.Authentication;
using BastetFTMAPI.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace BastetAPI
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
            BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(MongoDB.Bson.BsonType.String));

            var mongoDbSettings = Configuration.GetSection(nameof(MongoDbSetting)).Get<MongoDbSetting>();

            services.AddSingleton<IMongoClient>(serviceProvider =>
            {
                return new MongoClient(mongoDbSettings.ConnectionString);
            })
                    .AddTransient<IUserService, UserService>()
                    .AddTransient<IClientsRepository, MongoDbClientsRepository>()
                    .AddTransient<IAddressesRepository, MongoDbAddressesRepository>()
                    .AddTransient<IEmailRepository, MongoDbEmailRepository>()
                    .AddTransient<IPhonesRepository, MongoDbPhonesRepository>()
                    .AddTransient<ITicketsRepository, MongoDbTicketsRepository>()
                    .AddTransient<INotesRepository, MongoDbNotesRepository>();
            
            //Add Auto Mapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Remove Suppress Async suffix in action names at run time o => { o.SuppressAsyncSuffixInActionNames = false; }
            services.AddControllers(o => { o.SuppressAsyncSuffixInActionNames = false; });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v4", new OpenApiInfo { Title = "Bastet API", Version = "v4" });
            });

            services.AddAuthentication(x => 
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
              x.RequireHttpsMetadata = false;
              x.SaveToken = true;
              x.TokenValidationParameters = new()
              {
                ValidateIssuer = false,
                //ValidIssuer = "",
                ValidateAudience = false,
                //ValidAudience = "",
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("455578741455578741455578741455578741455578741"))
              };
            });

            //Health Checks
            services.AddRouting()
                    .AddHealthChecks()
                    .AddMongoDb(mongoDbSettings.ConnectionString,
                                name: "MongoDb",
                                timeout: TimeSpan.FromSeconds(3),
                                tags: new[] { "Ready" });
            //services
            //    .AddHealthChecksUI(setup =>
            //                 {
            //                     setup.SetHeaderText("Storage providers demo");
            //                     //Maximum history entries by endpoint
            //                     setup.MaximumHistoryEntriesPerEndpoint(50);
            //                     //One endpoint is configured in appsettings, let's add another one programatically
            //                     setup.AddHealthCheckEndpoint("Endpoint", "/Health");
            //                 });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v4/swagger.json", "BastetAPI v4"));

                app.UseHttpsRedirection();
            }

            app.UseRouting()
               .UseHttpsRedirection();

            app.UseAuthentication()
               .UseAuthorization();

            app.UseStatusCodePages();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                //endpoints.MapHealthChecksUI();
                //https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks
                endpoints.MapHealthChecks("/Health/Ready", new HealthCheckOptions
                {
                    Predicate = (check) => check.Tags.Contains("Ready"),
                    ResponseWriter = async (context, report) =>
                    {
                        var mongoDbSettings = Configuration.GetSection(nameof(MongoDbSetting)).Get<MongoDbSetting>();
                        var result = JsonSerializer.Serialize(
                            new
                            {
                                Status = report.Status.ToString(),
                                Checks = report.Entries.Select(entry => new
                                {
                                    Name = entry.Key,
                                    Status = entry.Value.Status.ToString(),
                                    Exception = entry.Value.Exception != null ? entry.Value.Exception.Message : "None",
                                    Duration = entry.Value.Duration.ToString(),
                                    ConnectionInfo = mongoDbSettings.ConnectionString

                                })
                            });
                        context.Response.ContentType = MediaTypeNames.Application.Json;
                        await context.Response.WriteAsync(result);
                    }
                });

                endpoints.MapHealthChecks("/Health/Live", new HealthCheckOptions { Predicate = (_) => false });
            });
        }
    }
}
