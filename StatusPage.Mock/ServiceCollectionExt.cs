using Microsoft.Extensions.DependencyInjection;
using StatusPage.Api;

namespace StatusPage.Mock
{
	public static class ServiceCollectionExt
	{
		public static void AddStatusPageDALMock(this IServiceCollection sc)
		{
			sc.AddSingleton<IServiceDAL, ServiceDALMock>();
		}
	}
}