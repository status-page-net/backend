using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using StatusPage.Api;

namespace StatusPage.MongoDB
{
	public static class ServiceCollectionExt
	{
		public static void AddStatusPageMongoDB(this IServiceCollection services, MongoClientSettings settings, string name)
		{
			services.AddSingleton(sp =>
			{
				var client = new MongoClient(settings);
				return client.GetDatabase(name);
			});
			services.AddSingleton<IServiceDAL, ServiceDAL>();
		}
	}
}