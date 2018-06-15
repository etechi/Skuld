using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using SF;
using SF.Sys.Services;
using Skuld.DataProviders;
using Skuld.DataProviders.Sina;
namespace SF.Sys.Services
{
	public static class SinaConfiguration
	{
		public static IServiceCollection AddSinaDataProviders(this IServiceCollection sc,SinaSetting Setting=null)
		{
			sc.AddSingleton(Setting ?? new SinaSetting());
			sc.AddScoped<ISymbolScanner, SinaSymbolScanner>();
			sc.AddScoped<IKLineFrameDigger, SinaKLineFrameDigger>();
			sc.AddScoped<ISymbolCategoryDigger, SinaSymbolCategoryDigger>();
			sc.AddScoped<ISymbolPropertyDigger, SinaSymbolPropertyDigger>();
			return sc;
		}
	}
}
