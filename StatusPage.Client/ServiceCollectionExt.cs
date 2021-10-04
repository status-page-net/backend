using Microsoft.Extensions.DependencyInjection;
using StatusPage.Api;
using System.Net.Http;

namespace StatusPage.Client
{
	public static class ServiceCollectionExt
	{
		public static void AddServiceClient(this IServiceCollection sc, HttpClient client)
		{
			sc.AddSingleton<IServiceBLL>(new ServiceClient(client));
		}
	}
}