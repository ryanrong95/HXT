using Needs.Ccs.Services;
using Needs.Utils.Npoi;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.WL.ConsoleToolApp
{
    public class ClearingData
    {
        public void GenerateHistory()
        {
            var workbook = ExcelFactory.ReadFile(@"C:\Users\user1\Desktop\clearing.xls");
            var npoi = new NPOIHelper(workbook);
            DataTable dt = npoi.ExcelToDataTable(true);

            List<ClearingDataVo> clearingDataVos = new List<ClearingDataVo>();
            for(int i = 0; i < dt.Rows.Count; i++)
            {
                ClearingDataVo dataVo = new ClearingDataVo();
                dataVo.WaybillCode = dt.Rows[i]["WaybillCode"].ToString();
                dataVo.CarrierID = dt.Rows[i]["CarrierID"].ToString();             
                dataVo.ArriveDate = DateTime.FromOADate(double.Parse(dt.Rows[i]["ArriveDate"].ToString()));
                dataVo.OrderID = dt.Rows[i]["OrderID"].ToString();
                dataVo.OrderItemID = dt.Rows[i]["OrderItemID"].ToString();
                clearingDataVos.Add(dataVo);
            }

            List<string> waybills = clearingDataVos.Select(t => t.WaybillCode).Distinct().ToList();

            using (var reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                for (int i = 0; i < waybills.Count(); i++)
                {
                    string waybillID = waybills[i];
                    try
                    {
                        Console.WriteLine(waybills[i] + "开始");
                        var items = clearingDataVos.Where(t => t.WaybillCode == waybills[i]).ToList();
                        string orderWayBillID = ChainsGuid.NewGuidUp();
                        reponsitory.Insert<Layer.Data.Sqls.ScCustoms.OrderWaybills>(new Layer.Data.Sqls.ScCustoms.OrderWaybills
                        {
                            ID = orderWayBillID,
                            OrderID = items.FirstOrDefault().OrderID,
                            WaybillCode = items.FirstOrDefault().WaybillCode,
                            CarrierID = items.FirstOrDefault().CarrierID,
                            AdminID = "",
                            HKDeclareStatus = 1,
                            Status = 200,
                            CreateDate = items.FirstOrDefault().ArriveDate
                        });

                        for (int j = 0; j < items.Count(); j++)
                        {
                            var sortingInfo = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>().Where(t => t.OrderItemID == items[j].OrderItemID && t.Status == 200).FirstOrDefault();
                            if (sortingInfo != null)
                            {
                                string sortingID = sortingInfo.ID;
                                reponsitory.Insert<Layer.Data.Sqls.ScCustoms.OrderWaybillItems>(new Layer.Data.Sqls.ScCustoms.OrderWaybillItems
                                {
                                    ID = ChainsGuid.NewGuidUp(),
                                    OrderWaybillID = orderWayBillID,
                                    SortingID = sortingID
                                });
                            }
                        }
                        Console.WriteLine(waybills[i] + "结束");
                    }
                    catch(Exception ex)
                    {
                        ex.CcsLog(waybills[i]);
                        continue;
                    }
                    
                }
            }
           
        }
    }
}
