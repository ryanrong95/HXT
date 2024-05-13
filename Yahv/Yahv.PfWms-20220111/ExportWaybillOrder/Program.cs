using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls.PvWms;
using Layers.Linq;
using Wms.Services.chonggous.Views;
using Wms.Services.chonggous.Models;
using Yahv.Linq;
using Layers.Data.Sqls;

namespace ExportWaybillOrder
{
    class Program
    {
        static void Main(string[] args)
        {
            
            #region 主逻辑
            using (var pvcenterReponsitory = new PvCenterReponsitory())
            using (var reponsitory = new PvWmsRepository())
            {
                var data = from notice in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                           join input in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>()
                           on notice.InputID equals input.ID
                           where notice.WareHouseID.Contains("SZ") && notice.Source == 35 && notice.Type == 200 && notice.WaybillID != ""
                           select new
                           {
                               notice.WaybillID,
                               input.OrderID,
                           };
                var ienum_data = data.ToArray().Distinct().OrderByDescending(item => item.WaybillID);

                Dictionary<string, List<string>> wayDits = new Dictionary<string, List<string>>();
                foreach (var item in ienum_data)
                {
                    if (wayDits.ContainsKey(item.WaybillID))
                    {
                        if (!wayDits[item.WaybillID].Contains(item.OrderID))
                        {
                            wayDits[item.WaybillID].Add(item.OrderID);
                        }
                    }
                    else
                    {
                        wayDits.Add(item.WaybillID, new List<string>() { item.OrderID });
                    }
                }

                foreach (var item in wayDits)
                {
                    var joinStr = string.Join(",", item.Value);
                    pvcenterReponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                    {
                        OrderID = joinStr
                    }, waybill => waybill.ID == item.Key);
                }

            }
            #endregion
            

            #region Test
            //Dictionary<string, List<string>> dicts = new Dictionary<string, List<string>>();
            //dicts.Add("1", new List<string>() { "2" });
            //dicts.Add("2", new List<string>() { "3", "4" });

            //foreach (var item in dicts)
            //{
            //    var joinStr = string.Join(",", item.Value);

            //}
            #endregion
        }


    }
}
