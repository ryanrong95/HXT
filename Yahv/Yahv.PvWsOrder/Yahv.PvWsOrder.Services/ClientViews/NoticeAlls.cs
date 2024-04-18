using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.PvWsOrder.Services.ClientModels;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    public class NoticeAlls : Yahv.Linq.UniqueView<Notice, PvWsOrderReponsitory>
    {
        public NoticeAlls()
        {
        }

        protected override IQueryable<Notice> GetIQueryable()
        {
            return from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.Notices>()
                   orderby notice.CreateDate descending
                   select new Notice
                   {
                       ID = notice.ID,
                     //  /Name = notice.Name,
                       Context = notice.Context,
                    //   URL = notice.URL,
                       CreateDate = notice.CreateDate,
                     //  Creator = notice.Creator,
                   };
        }
    }
}
