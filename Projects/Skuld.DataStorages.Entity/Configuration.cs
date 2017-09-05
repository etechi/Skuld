using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using SF;
using Skuld.DataStorages;
using Skuld.DataStorages.Entity;
using SF.Entities;
using SF.Data;
using Skuld.DataStorages.Entity.Models;
namespace SF.Core.ServiceManagement
{
	public static class SkuldEntityDataStorageConfiguration
	{
		public static IServiceCollection UseEntityDataStorages(this IServiceCollection sc,string TablePrefix)
		{
			sc.AddDataModules<Symbol>(TablePrefix);
			sc.AddDataModules<Category,CategorySymbol,CategoryType>(TablePrefix);
			sc.AddDataModules<Price>(TablePrefix);
			sc.AddDataModules<PropertyGroup, PropertyItem>(TablePrefix);
			sc.AddDataModules<SymbolPropertyGroup, SymbolPropertyGroupHistory, SymbolPropertyValue, SymbolPropertyValueHistory>(TablePrefix);

			sc.AddScoped<ISymbolStorageService, EFCoreSymbolStorageService>();
			sc.AddScoped<IKLineFrameStorageService, EFCoreKLineFrameStorageService>();
			sc.AddScoped<ISymbolCategoryStorageService, EFCoreSymbolCategoryStorageService>();
			sc.AddScoped<ISymbolPropertyStorageService, EFCoreSymbolPropertyStorageService>();

			return sc;
		}
	}
}
