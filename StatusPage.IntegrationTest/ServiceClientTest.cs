using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
		private static ServiceProvider _ServiceProvider;
		private static FunctionHostFactory _Host;

		protected override IServiceProvider ServiceProvider => _ServiceProvider;

		[ClassInitialize]
		public static void Init(TestContext _)
		{
			IServiceCollection services = new ServiceCollection();

			string root = DetectScriptRoot();
			_Host = FunctionHostFactory.Create(root);
			HttpClient client = _Host.CreateClient();
			services.AddServiceClient(client);

			_ServiceProvider = services.BuildServiceProvider();
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			_Host?.Dispose();
			_Host = null;

			_ServiceProvider?.Dispose();
			_ServiceProvider = null;
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