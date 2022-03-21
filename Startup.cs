using IssueTracker.Data;
using IssueTracker.Exceptions;
using IssueTracker.Logging;
using IssueTracker.ServiceImplementations;
using IssueTracker.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace IssueTracker;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<IssueTrackerContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<IIssueService, IssueServiceImpl>();
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder => builder.AllowAnyOrigin().AllowAnyHeader());
        });
        services.AddMvc(options => options.EnableEndpointRouting = false);
        services.AddControllers();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("Issue Tracker v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Issue Tracker v1",
                Description = "API for an issue tracker to use as practice for client interviews",
                Contact = new OpenApiContact
                {
                    Name = "Jeffery (Tyler) Dudley",
                    Email = "curlyq3756@gmail.com"
                }
            });
            c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
        });
    }
    
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
        loggerFactory.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "Logs"));
        
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/Issue Tracker v1/swagger.json", "Issue Tracker v1");
        });

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseCors();

        app.UseExceptionMiddleware();

        app.UseHttpsRedirection();

        app.UseMvc();

        app.UseRouting();

        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}