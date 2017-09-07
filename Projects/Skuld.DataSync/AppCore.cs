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
using SF.Core.Hosting;
using SF.Core.ServiceManagement.Management;
using SF.Core.Logging;

namespace Skuld.DataSync
{
	public static class AppCore
	{
		public static void AddAppCoreServices(IServiceCollection Services, EnvironmentType EnvType)
		{
			AddSysServices(Services, EnvType);
			AddCommonServices(Services, EnvType);
			AddBizServices(Services, EnvType);
		}
		static void AddSysServices(IServiceCollection Services, EnvironmentType EnvType)
		{
			if (EnvType == EnvironmentType.Utils)
				Services.AddConsoleDefaultFilePathStructure();

			Services.AddNewtonsoftJson();
			Services.AddSystemTimeService();
			Services.AddTaskServiceManager();
			//Services.AddDataContext();
			Services.AddDataEntityProviders();
			Services.AddServiceFeatureControl();
			Services.AddFilePathResolver();
			Services.AddLocalFileCache();
			Services.AddDefaultSecurityServices();
			Services.AddEventServices();
			Services.AddCallPlans();
			Services.AddDefaultCallPlanStorage();
		}
		static void AddCommonServices(IServiceCollection Services, EnvironmentType EnvType)
		{
			Services.AddDefaultKBServices();

			Services.AddManagedService();
			Services.AddManagedServiceAdminServices();

			//Services.AddMediaService(EnvType);

			//Services.InitService("媒体服务", (sp, sim) =>
			//	sim.NewMediaService()
			//	);


			Services.AddIdentGenerator();

		}
		static void AddBizServices(IServiceCollection Services, EnvironmentType EnvType)
		{
		}

		static Task InitCommonServices(IServiceProvider sp, IServiceInstanceManager sim, long? ParentId)
		{
			return Task.CompletedTask;
		}
	}

	public static class Net46App
	{
		public static ILogService LogService()
		{
			var ls = new LogService(new SF.Core.Logging.MicrosoftExtensions.MSLogMessageFactory());
			ls.AddDebug();
			return ls;
		}
		public static IAppInstanceBuilder Setup(EnvironmentType EnvType, ILogService logService = null,IServiceCollection Services=null)
		{
			var ls = logService ?? LogService();
			var builder = new SF.Core.Hosting.AppInstanceBuilder(
				null,
				EnvType,
				Services??new SF.Core.ServiceManagement.ServiceCollection(),
				ls
				)
				.With(sc => sc.AddLogService(ls))
				.With((sc, envType) => AppCore.AddAppCoreServices(sc, envType))
				.With((sc, envType) => ConfigServices(sc, envType))
				//.OnEnvType(e => e != EnvironmentType.Utils, sp =>
				//{
				//	var configuration = new DbMigrationsConfiguration();
				//	var migrator = new DbMigrator(configuration);
				//	migrator.Update();
				//	return null;
				//})
				;
			return builder;
		}


		static void ConfigServices(IServiceCollection Services, EnvironmentType EnvType)
		{
			//Services.AddSystemMemoryCache();
			//Services.AddSystemDrawing();
			Services.AddEFCoreDataEntity((sp,conn) => sp.Resolve<AppContext>());
			Services.AddConsoleDefaultFilePathStructure();

			//Services.AddTransient(tsp => new AppContext(tsp));
			//Services.AddEF6DataEntity<AppContext>();
			Services.AddDataContext(new SF.Data.DataSourceConfig
			{
				ConnectionString = AppContext.ConnectionString
			});

			Services.AddTransient<SyncRunner>();

			Services.AddScoped<SymbolSyncRunner>();

			Services.AddScoped<PriceSyncRunner>();
			Services.AddScoped<CategorySyncRunner>();
			Services.AddScoped<PropertySyncRunner>();

			Services.UseSinaDataProviders();
			Services.UseEntityDataStorages("");
		}

	}
}