using Needs.Ccs.Services.ApiSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class FinancePWordNo : IWordNo
    {
        public void GetWordNo()
        {
            var query = new Needs.Ccs.Services.Views.FinancePaymentViewRJ().
                Where(item =>  item.FinPCreSta == true && item.PayeeName== "暂收款");
            List<string> SwapIDs = query.Where(t => t.FinPCreWord == null || t.FinPCreWord == "").Select(t => t.ID).ToList();

            if (SwapIDs.Count() == 0)
            {
                return;
            }

            WordNoRequest wordNoRequest = new WordNoRequest();
            wordNoRequest.归属方案编号 = MakeAccountSetting.FinancePImport_归属方案编号;
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
                            responsitory.Update<Layer.Data.Sqls.ScCustoms.FinancePayments>(
                                       new
                                       {
                                           FinPCreWord = item.FNumber,
                                           FinPCreNo = item.FSerialNum
                                       }, t => t.ID == item.标识);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string swapIDs = string.Join(",", SwapIDs.ToArray());
                ex.CcsLog("获取缴纳报关进口关税增值税：" + swapIDs);
            }
        }
    }
}
