using Layers.Data.Sqls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.PdaApi.Services.Models;

namespace Yahv.PsWms.PdaApi.Services.Views
{
    /// <summary>
    /// 运单费用视图
    /// </summary>
    public class WaybillsView : UniqueView<Waybill, PsWmsRepository>
    {
        protected override IQueryable<Waybill> GetIQueryable()
        {
            var view = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Waybills>()
                       select new Waybill
                       {
                           ID = entity.ID,
                           Code = entity.Code,
                           Currency = (Underly.Currency)entity.Currency,
                           Freight = entity.Freight,
                           Weight = entity.Weight,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate
                       };

            return view;
        }

        /// <summary>
        /// 录入费用
        /// </summary>
        /// <param name="jobject"></param>
        public void EnterFee(JObject jobject)
        {
            var args = new
            {
                WaybillCode = jobject["WaybillCode"]?.Value<string>(),
                Freight = jobject["Freight"]?.Value<decimal>(),
                Weight = jobject["Weight"]?.Value<decimal>(),
            };

            if (string.IsNullOrEmpty(args.WaybillCode))
                throw new ArgumentNullException("运单号不能为空");
            if (args.Freight == null)
                throw new ArgumentNullException("运费不能为空");
            if (args.Weight == null)
                throw new ArgumentNullException("重量不能为空");

            if (!Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Waybills>().Any(item => item.Code == args.WaybillCode.Trim()))
            {
                Reponsitory.Insert(new Layers.Data.Sqls.PsWms.Waybills()
                {
                    ID = args.WaybillCode.Trim(),
                    Code = args.WaybillCode.Trim(),
                    Freight = args.Freight.Value,
                    Weight = args.Weight.Value,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now
                });
            }
            else
            {
                Reponsitory.Update<Layers.Data.Sqls.PsWms.Waybills>(new
                {
                    Freight = args.Freight.Value,
                    Weight = args.Weight.Value,
                    ModifyDate = DateTime.Now
                }, item => item.Code == args.WaybillCode.Trim());
            }
        }
    }
}
