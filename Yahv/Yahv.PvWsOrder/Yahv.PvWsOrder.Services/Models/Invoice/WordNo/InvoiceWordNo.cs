using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.Models
{
    public class InvoiceWordNo : IWordNo
    {
        public void GetWordNo()
        {
            var query = new Yahv.PvWsOrder.Services.Views.InvoicedListView().
               Where(item => item.InvoiceCreSta == true
                     );
            List<string> SwapIDs = query.Where(t => t.InCreWord == null || t.InCreWord == "").Select(t => t.InvoiceNoticeID).ToList();

            if (SwapIDs.Count() == 0)
            {
                return;
            }

            WordNoRequest wordNoRequest = new WordNoRequest();
            wordNoRequest.归属方案编号 = MakeAccountSetting.ServiceInvoImport_归属方案编号;
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
                        using (var responsitory = new Layers.Data.Sqls.PvWsOrderReponsitory())
                        {
                            responsitory.Update<Layers.Data.Sqls.PvWsOrder.InvoiceNotices>(
                                       new
                                       {
                                           InCreWord = item.FNumber,
                                           InCreNo = item.FSerialNum,
                                       }, t => t.ID == item.标识);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string swapIDs = string.Join(",", SwapIDs.ToArray());
                // ex.Message("获取服务费发票结果失败：" + swapIDs);
            }
        }
    }
}
