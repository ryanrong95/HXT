using Needs.Ccs.Services.ApiSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class FundTransWordNo : IWordNo
    {
        public void GetWordNo()
        {
            var query = new Needs.Ccs.Services.Views.FundTransferAppliesView()
                 .Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal && item.FundTranCreSta == true).
                 Where(item => item.OutAccount.AccountName != "华芯通-兴业银行快捷支付平台").
                 Where(item => item.ApplyStatus == Needs.Ccs.Services.Enums.FundTransferApplyStatus.Done).
                 OrderByDescending(t => t.CreateDate).AsQueryable();
            List<string> SwapIDs = query.Where(t => t.FundTranWrod == null || t.FundTranWrod == "").Select(t => t.ID).ToList();

            if (SwapIDs.Count() == 0)
            {
                return;
            }

            WordNoRequest wordNoRequest = new WordNoRequest();
            wordNoRequest.归属方案编号 = MakeAccountSetting.FundTransImport_归属方案编号;
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
                            responsitory.Update<Layer.Data.Sqls.ScCustoms.FundTransferApplies>(
                                       new
                                       {
                                           FundTranWrod = item.FNumber,
                                           FundTranNo = item.FSerialNum
                                       }, t => t.ID == item.标识);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string swapIDs = string.Join(",", SwapIDs.ToArray());
                ex.CcsLog("获取银行往来：" + swapIDs);
            }
        }
    }
}
