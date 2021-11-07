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
		private static HttpClient _Client;

		protected override IServiceProvider ServiceProvider => _ServiceProvider;

		[ClassInitialize]
		public static void Init(TestContext _)
		{
			IServiceCollection services = new ServiceCollection();

			_Server = new ServerMock();
			_Client = _Server.CreateClient();
			services.AddServiceClient(_Client);

			_ServiceProvider = services.BuildServiceProvider();
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			_Client = null;

			_Server?.Dispose();
			_Server = null;

			_ServiceProvider?.Dispose();
			_ServiceProvider = null;
		}
	}
}