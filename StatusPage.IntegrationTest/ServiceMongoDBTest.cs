using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using StatusPage.BLL;
using StatusPage.MongoDB;
using System;

namespace StatusPage.IntegrationTest
{
	[TestClass]
	public class ServiceMongoDBTest : ServiceTestBase
	{
		private static ServiceProvider _ServiceProvider;
		private static readonly string _DBName = "StatusPage_" + Guid.NewGuid().ToString("N");

		protected override IServiceProvider ServiceProvider => _ServiceProvider;

		[ClassInitialize]
		public static void Init(TestContext _)
		{
			IServiceCollection services = new ServiceCollection();
			services.AddStatusPageBLL();
			services.AddStatusPageMongoDB(options =>
			{
				options.ConnectionString = "mongodb://localhost:27017/";
				options.Name = _DBName;
			});
			_ServiceProvider = services.BuildServiceProvider();
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			IMongoDatabase database = _ServiceProvider.GetService<IMongoDatabase>();
			IMongoClient client = database.Client;
			client.DropDatabase(_DBName);

			_ServiceProvider.Dispose();
			_ServiceProvider = null;
		}
	}
}