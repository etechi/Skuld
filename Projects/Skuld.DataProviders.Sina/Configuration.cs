using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using SF;
using Skuld.DataProviders;
using Skuld.DataProviders.Sina;
namespace SF.Core.ServiceManagement
{
	public static class SinaConfiguration
	{
		public static IServiceCollection UseSinaDataProviders(this IServiceCollection sc,SinaSetting Setting=null)
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
