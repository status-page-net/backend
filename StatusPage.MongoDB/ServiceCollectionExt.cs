﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StatusPage.Api;
using System;

namespace StatusPage.MongoDB
{
	public static class ServiceCollectionExt
	{
		private static void AddStatusPageMongoDB(this IServiceCollection services)
		{
			services.AddSingleton(sp =>
			{
				Options options = sp.GetService<IOptions<Options>>().Value;
				MongoClientSettings settings = MongoClientSettings.FromConnectionString(options.ConnectionString);
				var client = new MongoClient(settings);
				return client.GetDatabase(options.Name);
			});
			services.AddSingleton<IServiceDAL, ServiceDAL>();
		}

		public static void AddStatusPageMongoDB(this IServiceCollection services, Action<Options> options)
		{
			services.Configure(options);
			services.AddStatusPageMongoDB();
		}

		public static void AddStatusPageMongoDB(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<Options>(configuration);
			services.AddStatusPageMongoDB();
		}
	}
}