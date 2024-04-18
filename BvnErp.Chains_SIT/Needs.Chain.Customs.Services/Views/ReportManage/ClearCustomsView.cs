using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ClearCustomsView : UniqueView<Models.CustomsClearance, ScCustomsReponsitory>
    {
        public ClearCustomsView()
        { }

        protected ClearCustomsView(ScCustomsReponsitory reponsitory, IQueryable<Models.CustomsClearance> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Models.CustomsClearance> GetIQueryable()
        {

            var result = from dechead in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                         join declist in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>() on dechead.ID equals declist.DeclarationID
                         join org in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseCountries>() on declist.OriginCountry equals org.Code into orgs
                         from baseOrg in orgs.DefaultIfEmpty()
                         join unit in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseUnits>() on declist.FirstUnit equals unit.Code into units
                         from baseUnit in units.DefaultIfEmpty()
                         select new Models.CustomsClearance
                         {
                            ID = dechead.ID,
                            VoyNo = dechead.VoyNo,
                            CreateDate = dechead.CreateTime,
                            OriginCountry = declist.OriginCountry,
                            Origin = baseOrg.EditionOne,
                            HSCode = declist.CodeTS,
                            Qty = declist.GQty,
                            FirstQty = declist.FirstQty.Value,
                            UnitName = baseUnit.Name,
                            Unit = declist.FirstUnit,
                            DecTotal = declist.DeclTotal,
                            Currency = declist.TradeCurr,
                            NetWt = declist.NetWt.Value,
                            GName = declist.GName
                         };

            return result;
        }

        /// <summary>
        ///  根据客户编号查询
        /// </summary>
        /// <param name="clientCode"></param>
        /// <returns></returns>
        public ClearCustomsView SearchByVoyNo(string voyNo)
        {
            var linq = from query in this.IQueryable
                       where query.VoyNo == voyNo
                       select query;

            var view = new ClearCustomsView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Models.CustomsClearance> iquery = this.IQueryable.Cast<Models.CustomsClearance>().OrderByDescending(item => item.CreateDate);
            
            //获取数据
            var ienum_clearanceData = iquery.ToArray();

            var group_clear = from c in ienum_clearanceData
                              group c by new { c.HSCode8, c.GName, c.Currency, c.UnitName } into g
                              orderby g.Key.HSCode8
                              select new Models.CustomsClearance
                              {
                                HSCode =  g.Key.HSCode8,
                                //Origin =  g.Key.Origin,
                                UnitName =  g.Key.UnitName,
                                GName = g.Key.GName,
                                Currency =  g.Key.Currency,
                                Qty = g.Sum(b => b.Qty),
                                FirstQty = g.Sum(b => b.FirstQty),
                                DecTotal = g.Sum(b => b.DecTotal),
                                NetWt = g.Sum(b => b.NetWt)
                              };

            var results = group_clear;
            int total = results.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                results = results.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            Func<Models.CustomsClearance, object> convert = item => new
            {
                //item.Origin,
                item.GName,
                item.HSCode,
                item.UnitName,
                item.UnitShow,
                item.Currency,
                item.Qty,
                item.FirstQty,
                item.DecTotal,
                item.NetWt
            };

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,             
                rows = results.Select(convert).ToArray(),
            };
        }
    }
}
