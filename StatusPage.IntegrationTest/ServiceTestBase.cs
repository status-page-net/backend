using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatusPage.Api;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StatusPage.IntegrationTest
{
	public abstract class ServiceTestBase
	{
		protected abstract IServiceProvider ServiceProvider { get; }

		private delegate Task ActionAsync(
			IServiceBLL serviceBLL,
			CancellationToken ct);

		private async Task RunAsync(ActionAsync action)
		{
			var serviceBLL = ServiceProvider.GetService<IServiceBLL>();
			await action(serviceBLL, CancellationToken.None);
		}

		[TestMethod]
		public async Task Create_Ok()
		{
			await RunAsync(async (serviceBLL, ct) =>
			{
				Service service = CreateDefaultService();
				await serviceBLL.CreateAsync(service, ct);
			});
		}

		[TestMethod]
		public async Task Create_Fail_AlreadyExists()
		{
			await RunAsync(async (serviceBLL, ct) =>
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
			await RunAsync(async (serviceBLL, ct) =>
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
			await RunAsync(async (serviceBLL, ct) =>
			{
				Service service = await serviceBLL.GetAsync(Guid.NewGuid(), ct);
				Assert.IsNull(service);
			});
		}

		[TestMethod]
		public async Task Update_Ok()
		{
			await RunAsync(async (serviceBLL, ct) =>
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
			await RunAsync(async (serviceBLL, ct) =>
			{
				Service service1 = CreateDefaultService();
				bool exists = await serviceBLL.UpdateAsync(service1, ct);
				Assert.IsFalse(exists);

				Service service2 = await serviceBLL.GetAsync(service1.Id, ct);
				Assert.IsNull(service2);
			});
		}

		[TestMethod]
		public async Task Delete_Ok()
		{
			await RunAsync(async (serviceBLL, ct) =>
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
			await RunAsync(async (serviceBLL, ct) =>
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