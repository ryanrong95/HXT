using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.DappApi.Services.Models;

namespace Yahv.PsWms.DappApi.Services.Views
{
    public class RequiresView : UniqueView
        <Require, PsWmsRepository>
    {
        #region 构造函数

        public RequiresView()
        {
        }

        public RequiresView(PsWmsRepository reponsitory) : base(reponsitory)
        {
        }
        #endregion

        protected override IQueryable<Require> GetIQueryable()
        {
            var view = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Requires>()
                       select new Require
                       {
                           ID = entity.ID,
                           NoticeID = entity.NoticeID,
                           NoticeTransportID = entity.NoticeTransportID,
                           Name = entity.Name,
                           Contents = entity.Contents,
                           CreateDate = entity.CreateDate,
                       };

            return view;
        }
    }
}
