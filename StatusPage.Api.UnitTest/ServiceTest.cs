using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace StatusPage.Api.UnitTest
{
	[TestClass]
	public class ServiceTest
	{
		[TestMethod]
		public void Validate_Ok()
		{
			var service = new Service
			{
				Id = Guid.NewGuid(),
				Title = "Service.Default"
			};
			service.Validate();
		}

		[TestMethod]
		public void Validate_Fail_Null()
		{
			Service service = null;
			Assert.ThrowsException<ApiArgumentException>(() => service.Validate());
		}

		[TestMethod]
		public void Validate_Fail_Title()
		{
			var service = new Service
			{
				Id = Guid.NewGuid(),
			};
			Assert.ThrowsException<ApiArgumentException>(() => service.Validate());
		}
	}
}