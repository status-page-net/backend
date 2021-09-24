using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using StatusPage.BLL;
using StatusPage.Mock;

[assembly: FunctionsStartup(typeof(StatusPage.Function.Startup))]

namespace StatusPage.Function
{
	public class Startup : FunctionsStartup
	{
		public override void Configure(IFunctionsHostBuilder builder)
		{
			builder.Services.AddStatusPageBLL();
			builder.Services.AddStatusPageDALMock();
		}
	}
}
