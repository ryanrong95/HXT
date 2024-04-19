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
    /// 用户收入视图
    /// </summary>
    public class UserInputsView : UniqueView<Models.UserInput, CvOssReponsitory>
    {
        internal UserInputsView()
        {

        }
        internal UserInputsView(CvOssReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.UserInput> GetIQueryable()
        {
            var clientsView = new ClientsTopView(this.Reponsitory);

            var linq = from entity in Reponsitory.ReadTable<Layer.Data.Sqls.CvOss.UserInputs>()
                       join client in clientsView on entity.ClientID equals client.ID
                       select new Models.UserInput
                       {
                           ID = entity.ID,
                           ClientID = client.ID,
                           Type = (UserAccountType)entity.Type,
                           From = (InputFrom)entity.From,
                           Currency = (Needs.Underly.Currency)entity.Currency,
                           Amount = entity.Amount,
                           Code = entity.Code,
                           CreateDate = entity.CreateDate,
                            
                           Client = client
                       };

            return linq;
        }

    }
}
