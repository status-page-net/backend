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
				Service source = CreateDefaultService();
				Service service = await serviceBLL.CreateAsync(source, ct);
				Assert.IsNotNull(service);
				Assert.AreEqual(source.Id, service.Id);
				Assert.AreNotEqual(source.ETag, service.ETag);
				Assert.AreEqual(source.Title, service.Title);
			});
		}

		[TestMethod]
		public async Task Create_Fail_AlreadyExists()
		{
			await RunAsync(async (serviceBLL, ct) =>
			{
				Service source = CreateDefaultService();
				Service service = await serviceBLL.CreateAsync(source, ct);

				async Task action() => await serviceBLL.CreateAsync(source, ct);

				await Assert.ThrowsExceptionAsync<ServiceAlreadyExistsException>(action);
			});
		}

		[TestMethod]
		public async Task Get_Ok()
		{
			await RunAsync(async (serviceBLL, ct) =>
			{
				Service source = CreateDefaultService();
				Service service1 = await serviceBLL.CreateAsync(source, ct);

				Service service2 = await serviceBLL.GetAsync(source.Id, ct);

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
				Service source = CreateDefaultService();
				Service service1 = await serviceBLL.CreateAsync(source, ct);
				service1.Title += ".Updated";

				Service service2 = await serviceBLL.UpdateAsync(service1, ct);
				Assert.IsNotNull(service2);
				Assert.AreEqual(service1.Id, service2.Id);
				Assert.AreNotEqual(service1.ETag, service2.ETag);
				Assert.AreEqual(service1.Title, service2.Title);

				Service service3 = await serviceBLL.GetAsync(service2.Id, ct);
				Assert.AreEqual(service2, service3);
			});
		}

		[TestMethod]
		public async Task Update_Ok_NotFound()
		{
			await RunAsync(async (serviceBLL, ct) =>
			{
				Service source = CreateDefaultService();
				Service service1 = await serviceBLL.UpdateAsync(source, ct);
				Assert.IsNull(service1);

				Service service2 = await serviceBLL.GetAsync(source.Id, ct);
				Assert.IsNull(service2);
			});
		}

		[TestMethod]
		public async Task Update_Fail_Outdated()
		{
			await RunAsync(async (serviceBLL, ct) =>
			{
				Service source = CreateDefaultService();
				Service service1 = await serviceBLL.CreateAsync(source, ct);

				service1.Title += ".Updated";
				Service service2 = await serviceBLL.UpdateAsync(service1, ct);

				async Task action() => await serviceBLL.UpdateAsync(service1, ct);

				OutdatedServiceException e = await Assert.ThrowsExceptionAsync<OutdatedServiceException>(action);
				Assert.AreEqual(e.Latest, service2);

				service1.ETag = e.Latest.ETag;
				Service service3 = await serviceBLL.UpdateAsync(service1, ct);
				Assert.IsNotNull(service3);

				Service service4 = await serviceBLL.GetAsync(service1.Id, ct);
				Assert.AreEqual(service3, service4);
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