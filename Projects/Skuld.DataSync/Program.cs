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
using Microsoft.Extensions.DependencyInjection;
using SF.Data.Entity.EntityFrameworkCore;
using SF.DI;
using Skuld.DataProviders;
using Skuld.DataStorages;
using SF.Serialization;

namespace Skuld.DataSync
{
	public class AppContext : DbContext
	{
		public AppContext(DbContextOptions<AppContext> options)
			: base(options)
		{ }

	}
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			//if (System.Diagnostics.Debugger.IsAttached == false)
			//	System.Diagnostics.Debugger.Launch();
			var connection = @"data source=localhost\SQLEXPRESS;initial catalog=skuld2;user id=sa;pwd=system;MultipleActiveResultSets=True;App=EntityFramework";
			services.AddDbContext<AppContext>(
				(isp, options) =>
				options.LoadDataModels(isp).UseSqlServer(connection)
				);

			// Add framework services.
			var sc = services.GetDIServiceCollection();

			sc.UseNewtonsoftJson();

			////sc.AddTransient<IAdd, Add>();
			//sc.UseMemoryManagedServiceSource();

			//var msc = new ManagedServiceCollection(sc);
			//msc.SetupServices();
			//msc.UseEFCoreIdentGenerator("App");
			//msc.UseEFCoreUser("App");

			//sc.UseServiceMetadata();

			sc.UseSinaDataProviders();
			sc.UseEntityDataStorages("Skuld");
			sc.UseEFCoreDataEntity<AppContext>();

			services.AddTransient<SyncRunner>();

			services.AddScoped<SymbolSyncRunner>();

			services.AddScoped<PriceSyncRunner>();
			services.AddScoped<CategorySyncRunner>();
			services.AddScoped<PropertySyncRunner>();

			var sp = services.BuildServiceProvider();
			return sp;
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
			var sc = new ServiceCollection();
			var sp=new Startup().ConfigureServices(sc);
			var runner = sp.GetRequiredService<SyncRunner>();
			runner.Execute().Wait();
		}
	}
}