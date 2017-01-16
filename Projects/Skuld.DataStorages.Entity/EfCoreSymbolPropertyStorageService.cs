using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using SF;
using System.Linq;
using System.Data.Entity;

namespace Skuld.DataStorages.Entity
{
	public class EFCoreSymbolPropertyStorageService
	{
		string ConnectionString { get; }
		public EFCoreSymbolPropertyStorageService(string ConnectionString)
		{
			this.ConnectionString = ConnectionString;
		}
		public async Task<Dictionary<string, DateTime>> GetNextUpdateTimes(Symbol symbol)
		{
			using (var ctx = new AppContext(ConnectionString))
			{
				//{ ST: sh: 600000:浦发银行 公司概况  公司名称 = 上海浦东发展银行股份有限公司; 所属行业 = 上海 银行业; 上市日期 = 1999 - 11 - 10; 注册资本 = 2161830; 公司电话 = 021 - 63611226,021 - 61618888; 办公地址 = 上海市中山东一路12号; 经营范围 = 吸收公众存款; 发放短期、中期和长期贷款; 办理结算; 办理票据贴现; 发行金融债券; 代理发行、代理兑付、承销政府、买卖政府债券; 同业拆借; 提供信用证服务及担保; 代理收付款项及代理保险业务; 提供保管箱服务; 外汇存款、外汇贷款、外汇汇款、外币兑换; 国际结算; 同业外汇拆借; 外汇票据的承兑和贴现; 外汇借款、外汇担保; 结汇、售汇; 买卖和代理买卖股票以外的外币有价证券; 自营和代客外汇买卖; 从事银行卡业务; 资信调查、咨询、见证业务; 离岸银行业务; 经批准的其它业务。}
				//ctx.SymbolPropertyGroupHistory.Add(new Models.SymbolPropertyGroupHistory
				//{
				//	Group = "公司概况",
				//	Symbol = "ST:sh:600000",
				//	Time = DateTime.Now
				//});
				//await ctx.SaveChangesAsync();

				var id = symbol.GetIdent();
				return await ctx.SymbolPropertyGroups
					.Where(g => g.Symbol == id && g.NextUpdateTime.HasValue)
					.ToDictionaryAsync(g => g.Group, g => g.NextUpdateTime.Value);
			}

		}
		async Task EnsurePropertyGroup(string name)
		{
			await Tasks.Retry(async ct =>
			{
				using (var ctx = new AppContext(ConnectionString))
				{
					var ctype = await ctx.PropertyGroups.FindAsync(name);
					if (ctype == null)
					{
						ctx.PropertyGroups.Add(new Models.PropertyGroup
						{
							Name = name
						});
						await ctx.SaveChangesAsync();
					}
				}
				return 0;
			});
		}
		async Task EnsurePropertyItem(string group,string name)
		{
			await Tasks.Retry(async ct =>
			{
				using (var ctx = new AppContext(ConnectionString))
				{
					var cat = await ctx.PropertyItems.FindAsync(group, name);
					if (cat == null)
					{
						ctx.PropertyItems.Add(new Models.PropertyItem
						{
							Group = group,
							Name = name
						});
						await ctx.SaveChangesAsync();
					}
				}
				return 0;
			});
		}
		static double? TryParseNumber(string str)
		{
			if (str.Length > 20)
				return null;
			double v;
			if (double.TryParse(str.Replace(",", ""), out v))
				return v;
			return null;
		}
		async Task MergeGroup(AppContext ctx, string id, PropertyGroup group, Models.SymbolPropertyGroup existsGroup, DateTime Time)
		{
			var existsValue = existsGroup == null ? Array.Empty<Models.SymbolPropertyValue>() :
				await ctx.SymbolPropertyValues.Where(v => v.Symbol == id && v.Group == group.Name).ToArrayAsync();
			if (existsGroup == null)
				ctx.SymbolPropertyGroups.Add(new Models.SymbolPropertyGroup
				{
					Group = group.Name,
					Symbol = id,
					Time = Time,
					RowCount = group.Rows.Length,
					NextUpdateTime=group.NextUpdateTime
				});
			else if(existsGroup.NextUpdateTime != group.NextUpdateTime ||
				existsGroup.Time!=Time ||
				existsGroup.RowCount !=group.Rows.Length
				)
			{
				existsGroup.NextUpdateTime = group.NextUpdateTime;
				existsGroup.Time = Time;
				existsGroup.RowCount = group.Rows.Length;
				ctx.Entry(existsGroup).State = EntityState.Modified;
			}

			foreach (var ev in existsValue)
			{
				var nv = ev.Row>=group.Rows.Length?null:group.Rows[ev.Row].Get(ev.Property);
				if (nv == null)
					ctx.SymbolPropertyValues.Remove(ev);
				else if (nv != ev.Value)
				{
					ev.Value = nv.Limit(1000);
					ev.Number = TryParseNumber(nv);
					ctx.Entry(ev).State = EntityState.Modified;
				}
			}

			for(var row=0;row<group.Rows.Length;row++)
				foreach(var p in group.Rows[row])
					if (!existsValue.Any(ev => ev.Property == p.Key && ev.Row==row))
						ctx.SymbolPropertyValues.Add(new Models.SymbolPropertyValue
						{
							Group = group.Name,
							Property = p.Key,
							Symbol = id,
							Value = p.Value.Limit(1000),
							Row=row,
							Number = TryParseNumber(p.Value)
						});
		}
		async Task MergeGroupHistory(AppContext ctx,string id, PropertyGroup group,Models.SymbolPropertyGroupHistory existsGroupHistory,DateTime Time)
		{
			var existsValueHistory = existsGroupHistory == null ? Array.Empty<Models.SymbolPropertyValueHistory>() :
				await ctx.SymbolPropertyValueHistory.Where(v => v.Symbol == id && v.Group == group.Name && v.Time == Time).ToArrayAsync();
			if (existsGroupHistory == null)
				ctx.SymbolPropertyGroupHistory.Add(new Models.SymbolPropertyGroupHistory
				{
					Group = group.Name,
					Symbol = id,
					Time = Time,
					RowCount=group.Rows.Length
				});

			foreach (var ev in existsValueHistory)
			{
				var nv = ev.Row >= group.Rows.Length ? null : group.Rows[ev.Row].Get(ev.Property);
				if (nv == null)
					ctx.SymbolPropertyValueHistory.Remove(ev);
				else if (nv != ev.Value)
				{
					ev.Value = nv.Limit(1000);
					ev.Number = TryParseNumber(nv);
					ctx.Entry(ev).State = EntityState.Modified;
				}
			}

			for (var row = 0; row < group.Rows.Length; row++)
				foreach (var p in group.Rows[row])
					if (!existsValueHistory.Any(ev => ev.Property == p.Key && ev.Row == row))
						ctx.SymbolPropertyValueHistory.Add(new Models.SymbolPropertyValueHistory
						{
							Group = group.Name,
							Property = p.Key,
							Symbol = id,
							Time = Time,
							Value = p.Value.Limit(1000),
							Row=row,
							Number = TryParseNumber(p.Value)
						});
		}
		async Task UpdateGroup(Symbol symbol, PropertyGroup group)
		{
			await Tasks.Retry(async ct =>
			{
				var id = symbol.GetIdent();
				using (var ctx=new AppContext(ConnectionString))
				{
					var curGroup = await ctx.SymbolPropertyGroups.FindAsync(id, group.Name);
					//没有当前组，直接新建
					if (curGroup == null)
					{
						var time = group.Time ?? DateTime.Now;
						await MergeGroup(ctx, id, group, null, time);
						await MergeGroupHistory(ctx, id, group, null, time);
					}
					//没有时间，需要当前组比较，如果有变化则认为是新的一组数据
					else if (!group.Time.HasValue)
					{
						var ops = await ctx.SymbolPropertyValues
							.Where(v => v.Symbol == id && v.Group == group.Name)
							.OrderBy(v => v.Row).ThenBy(v => v.Property)
							.Select(v => new { row = v.Row, prop = v.Property, value = v.Value })
							.ToArrayAsync();
						var vps = group.Rows.SelectMany((r, i) => r.OrderBy(p => p.Key).Select(p => new { row = i, prop = p.Key, value = p.Value })).ToArray();
						if(ops.Length!=vps.Length || ops.Zip(vps,(o,v)=>o.row!=v.row || o.prop!=v.prop || o.value!=v.value).Any(p=>p))
						{
							var time = DateTime.Now;
							await MergeGroup(ctx, id, group, curGroup, time);
							await MergeGroupHistory(ctx, id, group, null, time);
						}
					}
					//如果和当前组时间相同，需要合并
					else if (group.Time.Value == curGroup.Time)
					{
						await MergeGroup(ctx, id, group, curGroup, group.Time.Value);
						var existsGroupHistory = await ctx.SymbolPropertyGroupHistory.FindAsync(id, group.Name, group.Time.Value);
						await MergeGroupHistory(ctx, id, group, existsGroupHistory, group.Time.Value);
					}
					//如果比当前组时间新，需要新建组
					else if (group.Time.Value > curGroup.Time)
					{
						await MergeGroup(ctx, id, group, null, group.Time.Value);
						await MergeGroupHistory(ctx, id, group, null, group.Time.Value);
					}
					//如果比当前组时间旧，需要合并或新增历史组
					else
					{
						var existsGroupHistory = await ctx.SymbolPropertyGroupHistory.FindAsync(id, group.Name, group.Time.Value);
						await MergeGroupHistory(ctx, id, group, existsGroupHistory, group.Time.Value);
					}
					await ctx.SaveChangesAsync();
				}
				return 0;
			});

		}
		async Task Update(Symbol symbol, PropertyGroup group)
		{
			await EnsurePropertyGroup(group.Name);
			foreach (var prop in group.Rows.SelectMany(r=>r.Keys).Distinct())
				await EnsurePropertyItem(group.Name, prop);

			await UpdateGroup(symbol,group);
		}
		
		public async Task Update(Symbol symbol,IObservable<PropertyGroup> groups)
		{
			var pgs = new List<PropertyGroup>();
			await groups.ForEachAsync(g => pgs.Add(g));

			foreach(var gs in pgs.GroupBy(g=>g.Name))
				foreach(var gi in gs.OrderByDescending(g=>g.Time))
					await Update(symbol,gi);
		}
	}
}
