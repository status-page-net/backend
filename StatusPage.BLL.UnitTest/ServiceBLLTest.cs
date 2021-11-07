using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatusPage.Api;
using StatusPage.Mock;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StatusPage.BLL.UnitTest
{
	[TestClass]
	public class ServiceBLLTest
	{
		private delegate Task ActionAsync(
			IServiceBLL serviceBLL,
			ServiceDALMock mock,
			CancellationToken ct);

		private static async Task RunAsync(ActionAsync action)
		{
			var mock = new ServiceDALMock();
			var serviceBLL = new ServiceBLL(mock);
			await action(serviceBLL, mock, CancellationToken.None);
		}

		[TestMethod]
		public async Task Create_Fail_Service()
		{
			await RunAsync(async (serviceBLL, mock, ct) =>
			{
				Service service = null;
				async Task action() => await serviceBLL.CreateAsync(service, ct);

				await Assert.ThrowsExceptionAsync<ApiArgumentException>(action);

				service = CreateDefaultService();
				service.Title = null;
				await Assert.ThrowsExceptionAsync<ApiArgumentException>(action);
			});
		}

		[TestMethod]
		public async Task List_Fail_ServiceFilter()
		{
			await RunAsync(async (serviceBLL, mock, ct) =>
			{
				ServiceFilter filter = null;
				var pager = new ServicePager();
				async Task action() => await serviceBLL.ListAsync(filter, pager, ct);

				await Assert.ThrowsExceptionAsync<ApiArgumentException>(action);
			});
		}

		[TestMethod]
		public async Task List_Fail_ServicePager()
		{
			await RunAsync(async (serviceBLL, mock, ct) =>
			{
				var filter = new ServiceFilter();
				ServicePager pager = null;
				async Task action() => await serviceBLL.ListAsync(filter, pager, ct);

				await Assert.ThrowsExceptionAsync<ApiArgumentException>(action);

				pager = new ServicePager
				{
					Limit = ServicePager.MinLimit - 1
				};
				await Assert.ThrowsExceptionAsync<ApiArgumentException>(action);

				pager.Limit = ServicePager.MaxLimit + 1;
				await Assert.ThrowsExceptionAsync<ApiArgumentException>(action);

				pager.Limit = ServicePager.MinLimit;
				pager.Offset = ServicePager.MinOffset - 1;
				await Assert.ThrowsExceptionAsync<ApiArgumentException>(action);
			});
		}

		[TestMethod]
		public async Task Update_Fail_Service()
		{
			await RunAsync(async (serviceBLL, mock, ct) =>
			{
				Service service = null;
				async Task action() => await serviceBLL.UpdateAsync(service, ct);

				await Assert.ThrowsExceptionAsync<ApiArgumentException>(action);

				service = CreateDefaultService();
				service.Title = null;
				await Assert.ThrowsExceptionAsync<ApiArgumentException>(action);
			});
		}

		private static Service CreateDefaultService()
		{
			return new Service
			{
				Id = Guid.NewGuid(),
				Title = "Service.Default"
			};
		}
	}
}