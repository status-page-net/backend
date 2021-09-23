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
		public async Task Create_Ok()
		{
			await RunAsync(async (serviceBLL, mock, ct) =>
			{
				Service service = CreateDefaultService();
				await serviceBLL.CreateAsync(service, ct);
			});
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
		public async Task Create_Fail_AlreadyExists()
		{
			await RunAsync(async (serviceBLL, mock, ct) =>
			{
				Service service = CreateDefaultService();
				await serviceBLL.CreateAsync(service, ct);

				async Task action() => await serviceBLL.CreateAsync(service, ct);

				await Assert.ThrowsExceptionAsync<ServiceAlreadyExistsException>(action);
			});
		}

		[TestMethod]
		public async Task Get_Ok()
		{
			await RunAsync(async (serviceBLL, mock, ct) =>
			{
				Service service1 = CreateDefaultService();
				await serviceBLL.CreateAsync(service1, ct);

				Service service2 = await serviceBLL.GetAsync(service1.Id, ct);

				Assert.AreEqual(service1, service2);
			});
		}

		[TestMethod]
		public async Task Get_Ok_NotFound()
		{
			await RunAsync(async (serviceBLL, mock, ct) =>
			{
				Service service = await serviceBLL.GetAsync(Guid.NewGuid(), ct);
				Assert.IsNull(service);
			});
		}

		[TestMethod]
		public async Task Update_Ok()
		{
			await RunAsync(async (serviceBLL, mock, ct) =>
			{
				Service service1 = CreateDefaultService();
				await serviceBLL.CreateAsync(service1, ct);
				service1.Title += ".Updated";

				bool exists = await serviceBLL.UpdateAsync(service1, ct);
				Assert.IsTrue(exists);

				Service service2 = await serviceBLL.GetAsync(service1.Id, ct);
				Assert.AreEqual(service1, service2);
			});
		}

		[TestMethod]
		public async Task Update_Ok_NotFound()
		{
			await RunAsync(async (serviceBLL, mock, ct) =>
			{
				Service service1 = CreateDefaultService();
				bool exists = await serviceBLL.UpdateAsync(service1, ct);
				Assert.IsFalse(exists);

				Service service2 = await serviceBLL.GetAsync(service1.Id, ct);
				Assert.IsNull(service2);
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

		[TestMethod]
		public async Task Delete_Ok()
		{
			await RunAsync(async (serviceBLL, mock, ct) =>
			{
				Service service1 = CreateDefaultService();
				await serviceBLL.CreateAsync(service1, ct);

				bool exists = await serviceBLL.DeleteAsync(service1.Id, ct);
				Assert.IsTrue(exists);

				Service service2 = await serviceBLL.GetAsync(service1.Id, ct);
				Assert.IsNull(service2);
			});
		}

		[TestMethod]
		public async Task Delete_Ok_NotFound()
		{
			await RunAsync(async (serviceBLL, mock, ct) =>
			{
				Service service = CreateDefaultService();
				bool exists = await serviceBLL.DeleteAsync(service.Id, ct);
				Assert.IsFalse(exists);
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
