using System;
using System.Threading.Tasks;

namespace Skuld.DataStorages
{
	public interface IKLineFrameStorageService
	{
		KLineFrameRange GetKLineFrameRequired(Symbol symbol, int Interval);
		Task UpdateAsync(Symbol Symbol, int Interval, IObservable<KLineFrame> Frames);
	}
}
