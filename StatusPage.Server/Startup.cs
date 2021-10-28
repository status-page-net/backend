using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using StatusPage.BLL;
using StatusPage.MongoDB;

namespace StatusPage.Server
{
	public class Startup
	{
		private readonly IConfigurationRoot _configuration;

		public Startup(IWebHostEnvironment env)
		{
			_configuration = BuildConfiguration(env);
		}

		private static IConfigurationRoot BuildConfiguration(IWebHostEnvironment env)
		{
			var builder = new ConfigurationBuilder();

			builder.SetBasePath(env.ContentRootPath);
			builder.AddJsonFile(
				$"{typeof(Startup).Namespace}.json",
				optional: false,
				reloadOnChange: false);
			builder.AddJsonFile(
				$"{typeof(Startup).Namespace}.{env.EnvironmentName}.json",
				optional: true,
				reloadOnChange: false);
			builder.AddEnvironmentVariables();

			return builder.Build();
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();

			services.AddStatusPageBLL();
			services.AddStatusPageMongoDB(
				_configuration.GetSection("MongoDB"));
		}

		public void Configure(IApplicationBuilder app)
		{
			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}