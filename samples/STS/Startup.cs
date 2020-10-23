using System;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace STS
{
  public class Startup
  {
    public IWebHostEnvironment Environment { get; }
    public IConfiguration Configuration { get; }

    public Startup(IWebHostEnvironment environment, IConfiguration configuration)
    {
      Environment = environment;
      Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllersWithViews();

      // cookie policy to deal with temporary browser incompatibilities
      services.AddSameSiteCookiePolicy();

      // configures IIS out-of-proc settings (see https://github.com/aspnet/AspNetCore/issues/14882)
      services.Configure<IISOptions>(iis =>
      {
        iis.AuthenticationDisplayName = "Windows";
        iis.AutomaticAuthentication = false;
      });

      // configures IIS in-proc settings
      services.Configure<IISServerOptions>(iis =>
      {
        iis.AuthenticationDisplayName = "Windows";
        iis.AutomaticAuthentication = false;
      });

      var builder = services.AddIdentityServer(options =>
          {
            options.Events.RaiseErrorEvents = true;
            options.Events.RaiseInformationEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseSuccessEvents = true;
            options.Authentication = new AuthenticationOptions()
            {
              CookieLifetime = TimeSpan.FromHours(4), // ID server cookie timeout set to 10 hours
              CookieSlidingExpiration = true
            };
          })
          .AddInMemoryIdentityResources(Config.Ids)
          .AddInMemoryApiScopes(Config.ApiScopes)
          // .AddInMemoryApiResources(Config.ApiResources)
          .AddInMemoryClients(Config.Clients)
          .AddTestUsers(Config.GetUsers())
          .AddProfileService<ProfileService>();

      // not recommended for production - you need to store your key material somewhere secure
      builder.AddDeveloperSigningCredential();

      // services.AddAuthentication()
      //     .AddGoogle(options =>
      //     {
      //       // register your IdentityServer with Google at https://console.developers.google.com
      //       // enable the Google+ API
      //       // set the redirect URI to http://localhost:5000/signin-google
      //       options.ClientId = "copy client ID from Google here";
      //       options.ClientSecret = "copy client secret from Google here";
      //     });
    }

    public void Configure(IApplicationBuilder app)
    {
      app.UseCookiePolicy();

      if (Environment.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseStaticFiles();

      app.UseSerilogRequestLogging();

      app.UseRouting();

      app.UseIdentityServer();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapDefaultControllerRoute();
      });
    }
  }
}