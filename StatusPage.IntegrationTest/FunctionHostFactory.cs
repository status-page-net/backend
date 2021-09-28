using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Azure.WebJobs.Script;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Azure.WebJobs.Script.WebHost.Configuration;
using Microsoft.Azure.WebJobs.Script.WebHost.DependencyInjection;
using Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace StatusPage.IntegrationTest
{
	public class FunctionHostFactory : WebApplicationFactory<Startup>
	{
		private readonly string _root;

		private FunctionHostFactory(string root)
		{
			_root = root ?? throw new ArgumentNullException(nameof(root));
		}

		public static FunctionHostFactory Create<T>()
		{
			Assembly assembly = typeof(T).Assembly;
			string root = Path.GetDirectoryName(assembly.Location);
			return new FunctionHostFactory(root);
		}

		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			base.ConfigureWebHost(builder);

			builder.ConfigureServices(sc =>
			{
				var factory = new WebHostServiceProviderFactory();
				sc.Replace(ServiceDescriptor.Singleton<IServiceProviderFactory<IServiceCollection>>(factory));
			});
			builder.ConfigureAppConfiguration((context, config) =>
			{
				IConfigurationSource envVarsSource = config.Sources.OfType<EnvironmentVariablesConfigurationSource>().FirstOrDefault();
				if (envVarsSource != null)
				{
					config.Sources.Remove(envVarsSource);
				}

				config.Add(new ScriptEnvironmentVariablesConfigurationSource());

				config.Add(new WebScriptHostConfigurationSource
				{
					IsAppServiceEnvironment = SystemEnvironment.Instance.IsAppService(),
					IsLinuxContainerEnvironment = SystemEnvironment.Instance.IsLinuxConsumption(),
					IsLinuxAppServiceEnvironment = SystemEnvironment.Instance.IsLinuxAppService()
				});
			});
			builder.ConfigureLogging((context, loggingBuilder) =>
			{
				loggingBuilder.ClearProviders();

				loggingBuilder.AddDefaultWebJobsFilters();
				loggingBuilder.AddWebJobsSystem<WebHostSystemLoggerProvider>();
			});
		}
	}
}