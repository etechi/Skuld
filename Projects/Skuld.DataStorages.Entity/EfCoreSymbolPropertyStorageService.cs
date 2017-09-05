using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using SF;
using System.Linq;
using SF.Entities;
using System.Collections.Concurrent;
using SF.Data;

namespace Skuld.DataStorages.Entity
{
	public class EFCoreSymbolPropertyStorageService : ISymbolPropertyStorageService
	{
		IDataContext Context { get; }
		public EFCoreSymbolPropertyStorageService(IDataContext Context)
		{
			this.Context = Context;
		}
		public async Task<Dictionary<string, DateTime>> GetNextUpdateTimes(Symbol symbol)
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
				return await Context.Set<Models.SymbolPropertyGroup>().AsQueryable(true)
					.Where(g => g.Symbol == id && g.NextUpdateTime.HasValue)
					.ToDictionaryAsync(g => g.Group, g => g.NextUpdateTime.Value);

		}
		static ConcurrentDictionary<string, string> PropGroupNames = new ConcurrentDictionary<string, string>();
		async Task EnsurePropertyGroup(string name)
		{
			if (PropGroupNames.ContainsKey(name))
				return;
			await Context.Retry(async ct =>
				await Context.Set<Models.PropertyGroup>().EnsureAsync(
					new Models.PropertyGroup
					{
						Name = name
					})
			);
			PropGroupNames.TryAdd(name, name);
		}
		static ConcurrentDictionary<string, string> PropItemNames = new ConcurrentDictionary<string, string>();
		async Task EnsurePropertyItem(string group,string name)
		{
			var key = group + ":" + name;
			if (PropItemNames.ContainsKey(key))
				return;

			await Context.Retry(async ct =>
				await Context.Set<Models.PropertyItem>().EnsureAsync(
					new Models.PropertyItem
					{
						Group=group,
						Name = name
					})
				);

			PropItemNames.TryAdd(key, key);
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
		async Task<Models.SymbolPropertyGroup> MergeGroup(string id, PropertyGroup group, Models.SymbolPropertyGroup existsGroup, DateTime Time)
		{
			var existsValue = existsGroup == null ? Array.Empty<Models.SymbolPropertyValue>() :
				await Context.Set<Models.SymbolPropertyValue>().AsQueryable(false).Where(v => v.Symbol == id && v.Group == group.Name).ToArrayAsync();

			if (existsGroup == null)
				existsGroup=Context.Set<Models.SymbolPropertyGroup>().Add(
					new Models.SymbolPropertyGroup
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
				Context.Set< Models.SymbolPropertyGroup>().Update(existsGroup);
			}


			var set = Context.Set<Models.SymbolPropertyValue>();
			foreach (var ev in existsValue)
			{
				var nv = ev.Row>=group.Rows.Length?null:group.Rows[ev.Row].Get(ev.Property);
				if (nv == null)
					set.Remove(ev);
				else if (nv != ev.Value)
				{
					ev.Value = nv.Limit(1000);
					ev.Number = TryParseNumber(nv);
					Context.Set<Models.SymbolPropertyValue>().Update(ev);
				}
			}

			for(var row=0;row<group.Rows.Length;row++)
				foreach(var p in group.Rows[row])
					if (!existsValue.Any(ev => ev.Property == p.Key && ev.Row==row))
						set.Add(new Models.SymbolPropertyValue
						{
							Group = group.Name,
							Property = p.Key,
							Symbol = id,
							Value = p.Value.Limit(1000),
							Row=row,
							Number = TryParseNumber(p.Value)
						});
			return existsGroup;
		}
		async Task MergeGroupHistory(string id, PropertyGroup group,Models.SymbolPropertyGroupHistory existsGroupHistory,DateTime Time)
		{
			var existsValueHistory = existsGroupHistory == null ? Array.Empty<Models.SymbolPropertyValueHistory>() :
				await Context.Set<Models.SymbolPropertyValueHistory>().AsQueryable(false).Where(v => v.Symbol == id && v.Group == group.Name && v.Time == Time).ToArrayAsync();
			if (existsGroupHistory == null)
				Context.Set<Models.SymbolPropertyGroupHistory>().Add(new Models.SymbolPropertyGroupHistory
				{
					Group = group.Name,
					Symbol = id,
					Time = Time,
					RowCount=group.Rows.Length
				});
			var set = Context.Set<Models.SymbolPropertyValueHistory>();

			foreach (var ev in existsValueHistory)
			{
				var nv = ev.Row >= group.Rows.Length ? null : group.Rows[ev.Row].Get(ev.Property);
				if (nv == null)
					set.Remove(ev);
				else if (nv != ev.Value)
				{
					ev.Value = nv.Limit(1000);
					ev.Number = TryParseNumber(nv);
					set.Update(ev);
				}
			}

			for (var row = 0; row < group.Rows.Length; row++)
				foreach (var p in group.Rows[row])
					if (!existsValueHistory.Any(ev => ev.Property == p.Key && ev.Row == row))
						set.Add(new Models.SymbolPropertyValueHistory
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
		async Task UpdateGroup(Symbol symbol, PropertyGroup group, Dictionary<string, Models.SymbolPropertyGroup> curGroups)
		{
				var id = symbol.GetIdent();
				var curGroup = curGroups.Get(group.Name);
				//没有当前组，直接新建
				if (curGroup == null)
				{
					var time = group.Time ?? DateTime.Now;
					curGroups[group.Name]=await MergeGroup(id, group, null, time);
					await MergeGroupHistory(id, group, null, time);
				}
				//没有时间，需要当前组比较，如果有变化则认为是新的一组数据
				else if (!group.Time.HasValue)
				{
					var ops = await Context.Set<Models.SymbolPropertyValue>().AsQueryable()
						.Where(v => v.Symbol == id && v.Group == group.Name)
						.OrderBy(v => v.Row).ThenBy(v => v.Property)
						.Select(v => new { row = v.Row, prop = v.Property, value = v.Value })
						.ToArrayAsync();
					var vps = group.Rows.SelectMany((r, i) => r.OrderBy(p => p.Key).Select(p => new { row = i, prop = p.Key, value = p.Value })).ToArray();
					if(ops.Length!=vps.Length || ops.Zip(vps,(o,v)=>o.row!=v.row || o.prop!=v.prop || o.value!=v.value).Any(p=>p))
					{
						var time = DateTime.Now;
						curGroups[group.Name] = await MergeGroup(id, group, curGroup, time);
						await MergeGroupHistory(id, group, null, time);
					}
				}
				//如果和当前组时间相同，需要合并
				else if (group.Time.Value == curGroup.Time)
				{
					curGroups[group.Name] = await MergeGroup(id, group, curGroup, group.Time.Value);
					var existsGroupHistory = await Context.Set<Models.SymbolPropertyGroupHistory>().FindAsync(id, group.Name, group.Time.Value);
					await MergeGroupHistory(id, group, existsGroupHistory, group.Time.Value);
				}
				//如果比当前组时间新，需要新建组
				else if (group.Time.Value > curGroup.Time)
				{
					curGroups[group.Name] = await MergeGroup(id, group, curGroup, group.Time.Value);
					await MergeGroupHistory(id, group, null, group.Time.Value);
				}
				//如果比当前组时间旧，需要合并或新增历史组
				else
				{
					var existsGroupHistory = await Context.Set<Models.SymbolPropertyGroupHistory>().FindAsync(id, group.Name, group.Time.Value);
					await MergeGroupHistory(id, group, existsGroupHistory, group.Time.Value);
				}
				
		}
	
		
		public async Task Update(Symbol symbol,IObservable<PropertyGroup> groups)
		{
			var pgs = new List<PropertyGroup>();
			await groups.ForEachAsync(g => pgs.Add(g));

			foreach (var gs in pgs.GroupBy(g => g.Name))
			{
				await EnsurePropertyGroup(gs.Key);
				foreach (var gi in (from g in gs
								   from r in g.Rows
								   from k in r.Keys
								   select k).Distinct()
								   )
					await EnsurePropertyItem(gs.Key, gi);
			}

			await Context.Retry(async ct =>
			{
				var id = symbol.GetIdent();
				var curGroups = await Context
					.Set<Models.SymbolPropertyGroup>()
					.AsQueryable(false)
					.Where(g=>g.Symbol==id)
					.ToDictionaryAsync(g=>g.Group);

				foreach (var gs in pgs.GroupBy(g => g.Name))
					foreach (var gi in gs.OrderByDescending(g => g.Time))
						await UpdateGroup(symbol, gi, curGroups);

				await Context.SaveChangesAsync();
				return 0;
			});
		}
	}
}
