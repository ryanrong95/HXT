using NtErp.Wss.Oss.Services;
using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Views
{
    /// <summary>
    /// 用户支出视图
    /// </summary>
    public class UserOutputsView : UniqueView<Models.UserOutput, CvOssReponsitory>
    {
        public UserOutputsView()
        {

        }
        internal UserOutputsView(CvOssReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Models.UserOutput> GetIQueryable()
        {
            var clientsView = new ClientsTopView(this.Reponsitory);

            var linq = from entity in Reponsitory.ReadTable<Layer.Data.Sqls.CvOss.UserOutputs>()
                       select new Models.UserOutput
                       {

                           ID = entity.ID,
                           ClientID = entity.ClientID,
                           Type = (UserAccountType)entity.Type,
                           From = (OutputTo)entity.From,
                           OrderID = entity.OrderID,
                           Currency = (Needs.Underly.Currency)entity.Currency,
                           Amount = entity.Amount,
                           CreateDate = entity.CreateDate,
                           UserInputID = entity.UserInputID,
                           DateIndex = entity.DateIndex,
                            
                       };

            return linq;
        }
    }
}
