using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class SwapedNoticeHandler
    {
        private string[] PayExchangeSwapedNoticeIDs { get; set; }

        public SwapedNoticeHandler(string[] payExchangeSwapedNoticeIDs)
        {
            this.PayExchangeSwapedNoticeIDs = payExchangeSwapedNoticeIDs;
        }

        public void Execute()
        {
            if (this.PayExchangeSwapedNoticeIDs != null && this.PayExchangeSwapedNoticeIDs.Any())
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    foreach (var payExchangeSwapedNoticeID in this.PayExchangeSwapedNoticeIDs)
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeSwapedNotices>(new
                        {
                            HandleStatus = (int)Enums.SwapedNoticeHandleStatus.Handled,
                            UpdateTime = DateTime.Now,
                        }, item => item.ID == payExchangeSwapedNoticeID);
                    }
                }
            }
        }


    }
}
