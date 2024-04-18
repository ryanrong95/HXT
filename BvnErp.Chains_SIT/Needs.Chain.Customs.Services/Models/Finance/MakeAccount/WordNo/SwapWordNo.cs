using Needs.Ccs.Services.ApiSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class SwapWordNo : IWordNo
    {
        public void GetWordNo()
        {
            var query = new Needs.Ccs.Services.Views.SwapedListView().SearchBySwapCreSta(true);
            List<string> SwapIDs =  query.Where(t => t.SwapCreWord == null||t.SwapCreWord=="").Select(t=>t.SwapNoticeID).ToList();

            if (SwapIDs.Count() == 0)
            {
                return;
            }

            WordNoRequest wordNoRequest = new WordNoRequest();
            wordNoRequest.归属方案编号 = MakeAccountSetting.SwapImport_归属方案编号;
            wordNoRequest.标识集合 = SwapIDs;

            try
            {
                var mk = new WordNoHandler();
                var jResult = mk.PostToK3(wordNoRequest);

                if (jResult.success == false)
                {
                    return;
                }

                foreach(var item in jResult.data.标识集合)
                {
                    if (item.生成凭证 == 1)
                    {
                        using (Layer.Data.Sqls.ScCustomsReponsitory responsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                        {
                            responsitory.Update<Layer.Data.Sqls.ScCustoms.SwapNotices>(
                                       new
                                       {
                                           SwapCreWord = item.FNumber,
                                           SwapCreNo = item.FSerialNum
                                       }, t => t.ID == item.标识);
                        }
                    }                   
                }
            }
            catch(Exception ex)
            {
                string swapIDs = string.Join(",", SwapIDs.ToArray());
                ex.CcsLog("获取换汇结果失败：" + swapIDs);
            }
        }
    }

}
