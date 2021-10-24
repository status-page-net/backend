using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Azure.WebJobs.Script;
using Microsoft.Azure.WebJobs.Script.WebHost;
using System;

namespace StatusPage.IntegrationTest
{
	public class FunctionHostFactory : WebApplicationFactory<Startup>
	{
		private FunctionHostFactory()
		{
		}

		public static FunctionHostFactory Create(string root)
		{
			Environment.SetEnvironmentVariable(
				EnvironmentSettingNames.AzureWebJobsScriptRoot,
				root);
			return new FunctionHostFactory();
		}
	}
}