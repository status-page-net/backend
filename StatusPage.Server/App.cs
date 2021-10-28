using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace StatusPage.Server
{
	public class App
	{
		public static void Main(string[] args)
		{
			IHostBuilder builder = CreateHostBuilder(args);
			IHost host = builder.Build();
			host.Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args)
		{
			IHostBuilder builder = Host.CreateDefaultBuilder(args);
			builder.ConfigureWebHostDefaults((IWebHostBuilder builder) =>
				{
					builder.UseStartup<Startup>();
				});
			return builder;
		}
	}
}