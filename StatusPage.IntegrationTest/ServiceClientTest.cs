using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatusPage.Client;
using System;
using System.Net.Http;

namespace StatusPage.IntegrationTest
{
	[TestClass]
	public class ServiceClientTest : ServiceTestBase
	{
		private static ServiceProvider _ServiceProvider;
		private static ServerMock _Server;

		protected override IServiceProvider ServiceProvider => _ServiceProvider;

		[ClassInitialize]
		public static void Init(TestContext _)
		{
			IServiceCollection services = new ServiceCollection();

			_Server = new ServerMock();
			HttpClient client = _Server.CreateClient();
			services.AddServiceClient(client);

			_ServiceProvider = services.BuildServiceProvider();
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			_Server?.Dispose();
			_Server = null;

			_ServiceProvider?.Dispose();
			_ServiceProvider = null;
		}
	}
}