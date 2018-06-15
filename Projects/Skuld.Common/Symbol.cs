using SF;
using SF.Sys;
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
		public string ScopeCode { get; set; }
		public string GetIdent() => SymbolTypeToCode(Type) + ":" + ScopeCode;
		public override string ToString()
		{
			return GetIdent();
		}
		public static string SymbolTypeToCode(SymbolType Type)
		{
			switch (Type)
			{
				case SymbolType.Index:return "IN";  //指数
				case SymbolType.Bond: return "BO";   //债券
				case SymbolType.Stock: return "ST";  //股票
				case SymbolType.Fund: return "FD";   //基金
				case SymbolType.Warrant: return "WA";//权证
				case SymbolType.Futures: return "FU"; //期货
				case SymbolType.ForeignExchange: return "FE"; //外汇
				case SymbolType.Others: return "OT";  //其他
				default:
					throw new NotSupportedException();
			}
		}
		public static SymbolType ParseSymbolTypeCode(string Type)
		{
			if (Type.Length != 2)
				throw new ArgumentException();
			switch (Type)
			{
				case "IN":return SymbolType.Index;
				case "BO": return SymbolType.Bond;
				case "ST": return SymbolType.Stock;
				case "FD": return SymbolType.Fund;
				case "WA": return SymbolType.Warrant;
				case "FU": return SymbolType.Futures;
				case "FE": return SymbolType.ForeignExchange;
				case "OT": return SymbolType.Others;
				default:throw new ArgumentException();
			}
		}
		public static SymbolScope Parse(string Scope)
		{
			var p = Scope.Split2('-');
			if (p.Item2.Length == 0)
				throw new ArgumentException();
			return new SymbolScope
			{
				Type = ParseSymbolTypeCode(p.Item1),
				ScopeCode = p.Item2
			};
		}
	}
	public class Symbol
	{
		public SymbolScope Scope { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public string GetIdent() => $"{Scope}:{Code}";
		public override string ToString()
		{
			return string.Concat(GetIdent(),":",Name);
		}
		public static Symbol Parse(string Symbol)
		{
			var i = Symbol.LastIndexOf(':');
			if (i == -1)
				throw new ArgumentException();
			return new Skuld.Symbol
			{
				Scope = SymbolScope.Parse(Symbol.Substring(0, i)),
				Code = Symbol.Substring(i + 1)
			};
		}
	}
}
