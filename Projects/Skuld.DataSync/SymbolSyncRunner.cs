using System.Threading.Tasks;
using Skuld.DataProviders;
using Skuld.DataStorages;

namespace Skuld.DataSync
{
	class SymbolSyncRunner
	{
		public ISymbolScanner Scanner { get; }
		public ISymbolStorageService StorageService { get; }
		public SymbolSyncRunner(ISymbolScanner Scanner, ISymbolStorageService StorageService)
		{
			this.Scanner = Scanner;
			this.StorageService = StorageService;
		}
		public async Task Execute()
		{
			var ss = Scanner.Scan();
			await StorageService.Update(ss);
		}
	}
}