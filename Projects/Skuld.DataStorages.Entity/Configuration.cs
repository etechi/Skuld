using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using SF;
using Skuld.DataStorages;
using Skuld.DataStorages.Entity;
using SF.Data.Entity;
using Skuld.DataStorages.Entity.Models;
namespace SF.DI
{
	public static class SkuldEntityDataStorageConfiguration
	{
		public static IDIServiceCollection UseEntityDataStorages(this IDIServiceCollection sc,string TablePrefix)
		{
			sc.UseDataModules<Symbol>(TablePrefix);
			sc.UseDataModules<Category,CategorySymbol,CategoryType>(TablePrefix);
			sc.UseDataModules<Price>(TablePrefix);
			sc.UseDataModules<PropertyGroup, PropertyItem>(TablePrefix);
			sc.UseDataModules<SymbolPropertyGroup, SymbolPropertyGroupHistory, SymbolPropertyValue, SymbolPropertyValueHistory>(TablePrefix);

			sc.AddScoped<ISymbolStorageService, EFCoreSymbolStorageService>();
			sc.AddScoped<IKLineFrameStorageService, EFCoreKLineFrameStorageService>();
			sc.AddScoped<ISymbolCategoryStorageService, EFCoreSymbolCategoryStorageService>();
			sc.AddScoped<ISymbolPropertyStorageService, EFCoreSymbolPropertyStorageService>();

			return sc;
		}
	}
}
