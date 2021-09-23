using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatusPage.BLL;
using StatusPage.Mock;
using System;

namespace StatusPage.IntegrationTest
{
	[TestClass]
	public class ServiceDALMockTest : ServiceTestBase
	{
		private static IServiceProvider _ServiceProvider;

		protected override IServiceProvider ServiceProvider => _ServiceProvider;

		[ClassInitialize]
		public static void Init(TestContext context)
		{
			IServiceCollection services = new ServiceCollection();
			services.AddStatusPageBLL();
			services.AddStatusPageDALMock();
			_ServiceProvider = services.BuildServiceProvider();
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			_ServiceProvider = null;
		}
	}
}
