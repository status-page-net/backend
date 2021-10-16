using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatusPage.Api;
using StatusPage.BLL;
using StatusPage.Mock;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StatusPage.UnitTest
{
	[TestClass]
	public class ServiceTest
	{
		[TestMethod]
		public void Validate_Ok()
		{
			Service service = CreateDefaultService();
			Service.Validate(service);
		}

		[TestMethod]
		public void Validate_Fail_ServiceIsNull()
		{
			Assert.ThrowsException<InvalidServiceException>(() => Service.Validate(null));
		}

		[TestMethod]
		public void Validate_Fail_TitleIsNull()
		{
			Service service = CreateDefaultService();
			service.Title = null;
			Assert.ThrowsException<InvalidServiceException>(() => Service.Validate(service));
		}

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
		public async Task Create_Fail_ServiceIsNull()
		{
			await RunAsync(async (serviceBLL, mock, ct) =>
			{
				async Task action() => await serviceBLL.CreateAsync(null, ct);

				await Assert.ThrowsExceptionAsync<InvalidServiceException>(action);
			});
		}

		[TestMethod]
		public async Task Update_Fail_ServiceIsNull()
		{
			await RunAsync(async (serviceBLL, mock, ct) =>
			{
				async Task action() => await serviceBLL.UpdateAsync(null, ct);

				await Assert.ThrowsExceptionAsync<InvalidServiceException>(action);
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
