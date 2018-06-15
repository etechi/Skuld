using SF.Sys.Services;
using Skuld.DataStorages;
using Skuld.DataStorages.Entity;
using Skuld.DataStorages.Entity.Models;

namespace SF.Sys.Services
{
	public static class SkuldEntityDataStorageConfiguration
	{
		public static IServiceCollection AddEntityDataStorages(this IServiceCollection sc,string TablePrefix="Skuld")
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
