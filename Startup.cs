using System.Text;
using IssueTracker.Data;
using IssueTracker.Exceptions;
using IssueTracker.Logging;
using IssueTracker.ServiceImplementations;
using IssueTracker.Services;
using IssueTracker.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace IssueTracker;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    [Obsolete("Obsolete")]
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<IssueTrackerContext>(options =>
            options.UseNpgsql(Decrypter.Decrypt(Configuration.GetConnectionString("DefaultConnection"))));
        services.AddDbContext<AuthContext>(options =>
            options.UseNpgsql(Decrypter.Decrypt(Configuration.GetConnectionString("DefaultConnection"))));
        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<AuthContext>()
            .AddDefaultTokenProviders();
        services.AddScoped<IIssueService, IssueServiceImpl>();
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
        });
        services.AddMvc(options => options.EnableEndpointRouting = false);
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["JWT:ValidAudience"],
                    ValidIssuer = Configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
                };
            });
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
        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/Issue Tracker v1/swagger.json", "Issue Tracker v1"); });

        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

        app.UseCors();

        app.UseExceptionMiddleware();

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMvc();

        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}