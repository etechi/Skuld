using System.Threading.Tasks;
using Skuld.DataProviders;
using Skuld.DataStorages;

namespace Skuld.DataSync
{
	class CategorySyncRunner
	{
		public ISymbolCategoryDigger Digger { get; }
		public ISymbolCategoryStorageService StorageService { get; }
		public CategorySyncRunner(ISymbolCategoryDigger Digger,ISymbolCategoryStorageService StorageService)
		{
			this.Digger = Digger;
			this.StorageService = StorageService;

		}
		public async Task Execute(Symbol symbol)
		{
			if (symbol.Scope.Type != SymbolType.Stock)
				return;
			var re = await Digger.Dig(symbol);
			if (re == null)
				return;
			await StorageService.Update(symbol, re);
		}
	}
}