using System.ComponentModel.DataAnnotations;

namespace Skuld.DataProviders.Sina
{
	public class SinaSetting
	{
		[Display(Name = "股票基本数据服务器")]
		public string OSS { get;  set; } = "218.244.146.57";

		[Display(Name = "所有股票基本数据")]
		public string ALL_STOCK_BASICS_FILE { get;  set; } = "http://{OSS}/static/all.csv";

		[Display(Name = "股票/指数代码扫描URL")]
		//public string SymbolScanUrl { get; private set; } = "http://vip.stock.finance.sina.com.cn/quotes_service/api/json_v2.php/Market_Center.getHQNodeData?page={PAGE}&num={COUNT}&sort=changepercent&asc=0&node={NODE}";
		public string StockAndIndexScanUrl { get;  set; } = "http://vip.stock.finance.sina.com.cn/quotes_service/api/json_v2.php/Market_Center.getHQNodeData?page={PAGE}&num={COUNT}&sort=symbol&asc=1&node={NODE}";


		[Display(Name = "基金代码扫描URL")]
		//public string FundSymbolScanUrl { get; set; } = "http://vip.stock.finance.sina.com.cn/fund_center/data/jsonp.php/Begin/NetValueReturn_Service.NetValueReturnOpen?page={PAGE}&num={COUNT}&sort=form_year&asc=0&ccode=&type2=0&type3=";
		public string FundSymbolScanUrl { get; set; } = "http://stock.finance.sina.com.cn/fundfilter/api/openapi.php/MoneyFinanceFundFilterService.getFundFilterAll?callback=makeFilterData&page={PAGE}&num={COUNT}&dpc=1";
		[Display(Name = "基金价格")]
		public string FundPriceUrl { get; set; } = "http://stock.finance.sina.com.cn/fundInfo/api/openapi.php/CaihuiFundInfoService.getNav?callback=abc&symbol={SYMBOL}&datefrom=&dateto=&page={PAGE}&num={COUNT}";

		//复权因子
		[Display(Name = "股票/指数后复权价格")]
		public string StockAndIndexAdjuestPriceUrl { get;  set; } = "http://finance.sina.com.cn/realstock/newcompany/{SYMBOL}/phfq.js";
		//行情
		[Display(Name = "股票/指数交易价格")]
		public string StockAndIndexTradePriceUrl { get;  set; } = "http://money.finance.sina.com.cn/quotes_service/api/jsonp_v2.php/a/CN_MarketData.getKLineData?symbol={SYMBOL}&scale={SCALE}&ma=no&datalen={COUNT}";


		//上市时间，基本信息
		//http://stocks.sina.cn/sh/company?vt=4&code=sh601766 PS
		//股本结构
		//http://stocks.sina.cn/sh/capital?vt=4&code=sh601766 Date PS
		//十大股东
		//http://stocks.sina.cn/sh/holder?vt=4&code=sh601766 Date PS[]
		//财务摘要
		//http://stocks.sina.cn/sh/abstr?vt=4&code=sh601766 [Date PS][]
		//分红信息
		//http://stocks.sina.cn/sh/finance?vt=4&code=sh601766 [Date Ps][]
		//资产负债
		//http://stocks.sina.cn/sh/liab?vt=4&code=sh601766 [Unit] [Date Ps][]
		//利润简表
		//http://stocks.sina.cn/sh/profit?vt=4&code=sh601766 [Unit] [Date Ps][]
		//现金流量
		//http://stocks.sina.cn/sh/cash?vt=4&code=sh601766 [Unit] [Date Ps][]

		[Display(Name = "属性")]
		public string PropertyUrl { get; set; } = "http://stocks.sina.cn/sh/{TYPE}?vt=4&code={SYMBOL}";

		//所属板块
		[Display(Name = "所属板块")]
		public string CategoryUrl { get; set; } = "http://stock1.sina.cn/dpool/stock_new/v2/stock_owner.php?vt=4&code={SYMBOL}";

		//当日行情
		//http://hq.sinajs.cn/rn=votr3&list=sz300519,sz002802,sz300515,sh603016,sh603909,sz002789,sz300522,sh603268,sz002801,sz300518,sh603618,sh603029,sz002248,sz002703,sz000835,sz002098,sz002799,sz300500,sz300450,sh601069,sz002759,sh603309,sh603919,sz002788,sz002392,sh601388,sh600503,sh603318,sz002800,sz002115,sz002795,sz300508,sh600758,sh600516,sz002124,sz002766,sz300126,sz000785,sh603958,sz300292,sz002716,sz300501
	}
}
