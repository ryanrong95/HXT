using Needs.Ccs.Services.ApiSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class BuyWordNo : IWordNo
    {
        public void GetWordNo()
        {
            var query = new Needs.Ccs.Services.Views.AccImportView().
                Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal && item.ExchangeDate != null && item.BuyCreSta == true);
            List<string> SwapIDs = query.Where(t => t.BuyCreWord == null || t.BuyCreWord == "").Select(t => t.Code).ToList();

            if (SwapIDs.Count() == 0)
            {
                return;
            }

            WordNoRequest wordNoRequest = new WordNoRequest();
            wordNoRequest.归属方案编号 = MakeAccountSetting.BuyImport_归属方案编号;
            wordNoRequest.标识集合 = SwapIDs;

            try
            {
                var mk = new WordNoHandler();
                var jResult = mk.PostToK3(wordNoRequest);

                if (jResult.success == false)
                {
                    return;
                }

                foreach (var item in jResult.data.标识集合)
                {
                    if (item.生成凭证 == 1)
                    {
                        using (Layer.Data.Sqls.ScCustomsReponsitory responsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                        {
                            responsitory.Update<Layer.Data.Sqls.ScCustoms.MoneyOrders>(
                                       new
                                       {
                                           BuyCreWord = item.FNumber,
                                           BuyCreNo = item.FSerialNum
                                       }, t => t.Code == item.标识);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string swapIDs = string.Join(",", SwapIDs.ToArray());
                ex.CcsLog("获取承兑贴现结果失败：" + swapIDs);
            }
        }
    }
}
