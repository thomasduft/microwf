namespace STS;

public static class ConfigureApplication
{
  public static void UseSTS(
    this IApplicationBuilder app,
    IWebHostEnvironment environment
  )
  {
    if (environment.IsDevelopment())
    {
      app.UseCors(builder =>
      {
        builder.WithOrigins("http://localhost:4200");
        builder.AllowAnyHeader();
        builder.AllowAnyMethod();
        builder.AllowCredentials();
      });

      app.UseDeveloperExceptionPage();
    }
    else
    {
      app.UseExceptionHandler("/Error");
    }

    app.UseDefaultFiles();
    app.UseStaticFiles();

    app.UseStatusCodePagesWithReExecute("/error");

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseEndpoints(builder =>
    {
      builder.MapControllers();
      builder.MapRazorPages();
      builder.MapDefaultControllerRoute();
    });
  }
}
