using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatusPage.Function;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace StatusPage.IntegrationTest
{
	[TestClass]
	public class FunctionHostTest
	{
		[TestMethod]
		public async Task Ping_Ok()
		{
			using var host = FunctionHostFactory.Create<ServiceController>();
			HttpClient client = host.CreateClient();
			using HttpResponseMessage response = await client.GetAsync("admin/host/ping");
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			string content = await response.Content.ReadAsStringAsync();
			Assert.AreEqual(String.Empty, content);
		}
	}
}