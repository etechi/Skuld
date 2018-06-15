using System.Threading.Tasks;
using Skuld.DataProviders;
using Skuld.DataStorages;

namespace Skuld.DataSync
{

	class PropertySyncRunner
	{
		public ISymbolPropertyDigger Digger { get; }
		public ISymbolPropertyStorageService StorageService { get; }
		public PropertySyncRunner(ISymbolPropertyDigger Digger, ISymbolPropertyStorageService StorageService)
		{
			this.Digger = Digger;
			this.StorageService = StorageService;
		}
		public async Task Execute(Symbol symbol)
		{
			if (symbol.Scope.Type != SymbolType.Stock)
				return;
			var updateTimes = await StorageService.GetNextUpdateTimes(symbol);
			var re = Digger.Dig(symbol, updateTimes);
			await StorageService.Update(symbol, re);
		}
	}
}