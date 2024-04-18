using iTextSharp.text;
using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.ClientViews;
using Yahv.PvWsOrder.Services.XDTModels;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.XDTClientView
{
    public class NoticeBoardView : UniqueView<NoticeBoardModel, ScCustomReponsitory>
    {
        public NoticeBoardView()
        {

        }

        protected override IQueryable<NoticeBoardModel> GetIQueryable()
        {
            var iQuery = from noticeBoard in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.NoticeBoard>()
                         where noticeBoard.Status == (int)GeneralStatus.Normal
                         select new NoticeBoardModel
                         {
                             ID = noticeBoard.ID,
                             Title = noticeBoard.NoticeTitle,
                             CreateDate = noticeBoard.CreateDate,
                             Content = noticeBoard.NoticeContent,
                         };
            return iQuery;
        }

        public Tuple<NoticeBoardModel[], int> ToPage(int page = 1, int rows = 10)
        {
            IQueryable<NoticeBoardModel> iquery = this.IQueryable.Cast<NoticeBoardModel>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();
            iquery = iquery.Skip(rows * (page - 1)).Take(rows);

            //获取数据
            var ienum_noticeboard = iquery.ToArray();

            var result = ienum_noticeboard;

            return new Tuple<NoticeBoardModel[], int>(result.ToArray(), total);
        }

    }
}
