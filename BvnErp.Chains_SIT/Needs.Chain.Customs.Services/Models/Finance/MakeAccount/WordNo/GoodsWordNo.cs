using Needs.Ccs.Services.ApiSettings;
using Needs.Ccs.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class GoodsWordNo : IWordNo
    {
        public void GetWordNo()
        {
            var query = new Needs.Ccs.Services.Views.GoodsImportView()
                .Where(item => item.Status == Status.Normal && item.FeeType == FinanceFeeType.DepositReceived && item.GoodsCreStatus == true)
                .OrderByDescending(item => item.ReceiptDate)
                .AsQueryable();
            List<string> SwapIDs = query.Where(t => t.GoodsCreWord == null || t.GoodsCreWord == "").Select(t => t.ID).ToList();

            if (SwapIDs.Count() == 0)
            {
                return;
            }

            WordNoRequest wordNoRequest = new WordNoRequest();
            wordNoRequest.归属方案编号 = MakeAccountSetting.GoodsImport_归属方案编号;
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
                            responsitory.Update<Layer.Data.Sqls.ScCustoms.FinanceReceipts>(
                                       new
                                       {
                                           GoodsCreWord = item.FNumber,
                                           GoodsCreNo = item.FSerialNum
                                       }, t => t.ID == item.标识);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string swapIDs = string.Join(",", SwapIDs.ToArray());
                ex.CcsLog("获取货款失败：" + swapIDs);
            }
        }
    }
}
