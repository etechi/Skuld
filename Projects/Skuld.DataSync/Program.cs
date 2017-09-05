using System;
using Skuld.DataProviders.Sina;
using Skuld.DataStorages.Entity;
using System.Threading.Tasks;
using Skuld;
using System.Reactive.Linq;
using System.IO;
using System.Reactive.Joins;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SF.Core.ServiceManagement;
using Skuld.DataProviders;
using Skuld.DataStorages;
using SF.Core.Serialization;
using SF.Data.EntityFrameworkCore;
using System.Data.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Design;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Skuld.DataSync
{
	public class AppContext : DbContext
	{
		public static string ConnectionString { get; }= @"data source=localhost\SQLEXPRESS;initial catalog=skuld2;user id=sa;pwd=system;MultipleActiveResultSets=True;App=EntityFramework";
		public AppContext(DbContextOptions<AppContext> options)
			: base(options)
		{ }

	}
	public class AppContextFactory : IDesignTimeDbContextFactory<AppContext>
	{

		public AppContext CreateDbContext(string[] args)
		{
			Debugger.Launch();
			var sc = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
			sc.AddLogging(lb => lb.AddDebug().AddFilter(l=>true));
			return new Startup().ConfigureServices(sc).Resolve<AppContext>();
		}
	}
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		public IServiceProvider ConfigureServices(Microsoft.Extensions.DependencyInjection.IServiceCollection services=null)
		{
			if (services == null)
				services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
			services.AddMemoryCache();
			services.AddDbContext<AppContext>(
				(isp, options) =>
					options.LoadDataModels(isp).UseSqlServer(isp.Resolve<DbConnection>()),
				ServiceLifetime.Transient
				);

			var sc=new SF.Core.ServiceManagement.ServiceCollection();
			sc.AddServices(services);
			

			return Net46App.Setup(SF.Core.Hosting.EnvironmentType.Production, null, sc).Build().ServiceProvider;
			////if (System.Diagnostics.Debugger.IsAttached == false)
			////	System.Diagnostics.Debugger.Launch();
			//services.AddDataContext(new SF.Data.DataSourceConfig
			//{
			//	ConnectionString = connection
			//});
			//services.AddDataEntityProviders();
			//services.add
			//services.AddDbContext<AppContext>(
			//	(isp, options) =>
			//	options.LoadDataModels(isp).UseSqlServer(connection)
			//	);

			//// Add framework services.
			//var sc = services.GetDIServiceCollection();

			//sc.UseNewtonsoftJson();

			//////sc.AddTransient<IAdd, Add>();
			////sc.UseMemoryManagedServiceSource();

			////var msc = new ManagedServiceCollection(sc);
			////msc.SetupServices();
			////msc.UseEFCoreIdentGenerator("App");
			////msc.UseEFCoreUser("App");

			////sc.UseServiceMetadata();

			//sc.UseSinaDataProviders();
			//sc.UseEntityDataStorages("Skuld");
			//sc.UseDataContext();
			//sc.UseEFCoreDataEntity<AppContext>();

			//services.AddTransient<SyncRunner>();

			//services.AddScoped<SymbolSyncRunner>();

			//services.AddScoped<PriceSyncRunner>();
			//services.AddScoped<CategorySyncRunner>();
			//services.AddScoped<PropertySyncRunner>();

			//var sp = services.BuildServiceProvider();
			//return sp;
		}
	}



	class Program
	{
		
		//static async Task Test()
		//{
		//	Console.WriteLine(DateTime.Now);
		//	var fc = new SF.FlowController(1);
		//	await Enumerable.Range(0, 100).ToObservable()
		//		.Delay(r => Observable.FromAsync(fc.Wait).IgnoreElements())
		//		.SelectMany(async r =>
		//		{
		//			await Task.Delay(1000);
		//			Console.WriteLine($"{r} {DateTime.Now}");
		//			fc.Complete();
		//			return r;
		//		})
		//		.ForEachAsync(o => { });

		//}
		static void Main(string[] args)
		{
			var sp = new Startup().ConfigureServices();
			var runner = sp.Resolve<SyncRunner>();
			runner.Execute().Wait();
		}
	}
}