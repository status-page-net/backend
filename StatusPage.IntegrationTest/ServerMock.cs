using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using StatusPage.Mock;
using StatusPage.Server;

namespace StatusPage.IntegrationTest
{
	public class ServerMock : WebApplicationFactory<Startup>
	{
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			base.ConfigureWebHost(builder);

			builder.ConfigureServices((IServiceCollection services) =>
			{
				services.AddStatusPageDALMock();
			});
		}
	}
}