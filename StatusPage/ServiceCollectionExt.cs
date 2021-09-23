using Microsoft.Extensions.DependencyInjection;
using StatusPage.Api;

namespace StatusPage.BLL
{
	public static class ServiceCollectionExt
	{
		public static void AddStatusPageBLL(this IServiceCollection sc)
		{
			sc.AddSingleton<IServiceBLL, ServiceBLL>();
		}
	}
}