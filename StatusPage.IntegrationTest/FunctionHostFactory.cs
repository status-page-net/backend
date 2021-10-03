using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Azure.WebJobs.Script;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Azure.WebJobs.Script.WebHost.Configuration;
using Microsoft.Azure.WebJobs.Script.WebHost.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using StatusPage.BLL;
using StatusPage.Mock;
using System;
using System.Linq;

namespace StatusPage.IntegrationTest
{
	public class FunctionHostFactory : WebApplicationFactory<Startup>
	{
		private FunctionHostFactory()
		{
		}

		public static FunctionHostFactory Create(string root)
		{
			Environment.SetEnvironmentVariable(
				EnvironmentSettingNames.AzureWebJobsScriptRoot,
				root);
			return new FunctionHostFactory();
		}

		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			base.ConfigureWebHost(builder);

			builder.ConfigureServices(services =>
			{
				var factory = new WebHostServiceProviderFactory();
				services.Replace(ServiceDescriptor.Singleton<IServiceProviderFactory<IServiceCollection>>(factory));
				services.AddStatusPageBLL();
				services.AddStatusPageDALMock();
			});
			builder.ConfigureAppConfiguration((context, config) =>
			{
				IConfigurationSource source = config.Sources.OfType<EnvironmentVariablesConfigurationSource>().FirstOrDefault();
				if (source != null)
				{
					config.Sources.Remove(source);
				}

				config.Add(new ScriptEnvironmentVariablesConfigurationSource());

				config.Add(new WebScriptHostConfigurationSource
				{
					IsAppServiceEnvironment = SystemEnvironment.Instance.IsAppService(),
					IsLinuxContainerEnvironment = SystemEnvironment.Instance.IsLinuxConsumption(),
					IsLinuxAppServiceEnvironment = SystemEnvironment.Instance.IsLinuxAppService()
				});
			});
			builder.ConfigureLogging((context, builder) =>
			{
				builder.ClearProviders();
			});
		}
	}
}