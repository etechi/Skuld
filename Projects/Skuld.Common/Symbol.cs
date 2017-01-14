using System;

namespace Skuld
{
	public enum SymbolType
	{
		Index,  //指数
		Bond,   //债券
		Stock,  //股票
		Fund,   //基金
		Warrant,//权证
		Futures, //期货
		ForeignExchange, //外汇
		Others,  //其他
	}
	public class SymbolScope
	{
		public SymbolType Type { get; set; }
		public string Name { get; set; }
		public override string ToString()
		{
			return Type + "-" + Name;
		}
	}
	public class Symbol
	{
		public SymbolScope Scope { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public override string ToString()
		{
			return $"{Scope}-{Code}:{Name}";
		}
	}
}
