using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatusPage.Api;
using StatusPage.Client;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;

namespace StatusPage.IntegrationTest
{
	[TestClass]
	public class ServiceClientTest : ServiceTestBase
	{
		private static IServiceProvider _ServiceProvider;
		private static FunctionHostFactory _Host;

		protected override IServiceProvider ServiceProvider => _ServiceProvider;

		[ClassInitialize]
		public static void Init(TestContext context)
		{
			string root = DetectScriptRoot();
			_Host = FunctionHostFactory.Create(root);
			IServiceCollection services = new ServiceCollection();
			services.AddSingleton<IServiceBLL>(sp =>
			{
				HttpClient client = _Host.CreateClient();
				return new ServiceClient(client);
			});
			_ServiceProvider = services.BuildServiceProvider();
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			_ServiceProvider = null;
			_Host = null;
		}

		private static string DetectScriptRoot()
		{
			Assembly assembly = typeof(FunctionHostFactory).Assembly;
			string project = Path.GetFileNameWithoutExtension(assembly.Location);
			string path = Path.GetDirectoryName(assembly.Location);
			return path.Replace(project, "StatusPage.Function");
		}
	}
}