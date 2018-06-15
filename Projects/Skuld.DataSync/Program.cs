using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Design;
using SF.Sys.Hosting;
using SF.Sys.Services;
using SF.Sys;
using SF.Core.ServiceManagement;

namespace Skuld.DataSync
{
	public class AppContext : DbContext
	{
		public static string ConnectionString { get; }= @"data source=localhost\SQLEXPRESS;initial catalog=skuld3;user id=sa;pwd=system;MultipleActiveResultSets=True;App=EntityFramework";
		public AppContext(DbContextOptions<AppContext> options)
			: base(options)
		{ }

	}
	public class AppContextFactory : IDesignTimeDbContextFactory<AppContext>
	{
		IAppInstance Instance { get; } = AppBuilder
					.Init(EnvironmentType.Utils)
					.With((sc, env) =>
					{
						sc.AddMSConfiguration();
					}).Build();

		public AppContext CreateDbContext(string[] args)
		{
			return Instance.ServiceProvider.Resolve<AppContext>();
		}
	}
	public static class AppBuilder
	{
		public static IAppInstanceBuilder Init(
			EnvironmentType EnvType
			)
		{
			var builder = new AppInstanceBuilder(null, EnvType)
				.With((sc, envType) =>
					sc
					.AddSystemServices(EnvType)
					.AddMSConfiguration()
					.AddEFCoreDbContext<AppContext>(true)

					.AddTransient<SyncRunner>()
					.AddSinaDataProviders()
					.AddEntityDataStorages()
					.AddScoped<SymbolSyncRunner>()
					.AddScoped<PriceSyncRunner>()
					.AddScoped<CategorySyncRunner>()
					.AddScoped<PropertySyncRunner>()					
				)
				;

			return builder;
		}
	}
	public class Startup
	{
		public IServiceProvider ConfigureServices(Microsoft.Extensions.DependencyInjection.IServiceCollection services=null)
		{
			var ins = AppBuilder.Init( EnvironmentType.Utils)
			.OnEnvType(
				t => t != EnvironmentType.Utils,
				sc =>
				{
					sc.AddMSServices(services);
				}
				)
			.Build();
			return ins.ServiceProvider;
		}
	}



	class Program
	{
		static void Main(string[] args)
		{
			System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

			var sp = new Startup().ConfigureServices();
			var runner = sp.Resolve<SyncRunner>();
			runner.Execute().Wait();
		}
	}
}