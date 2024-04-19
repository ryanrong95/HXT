using Needs.Erp.Generic;
using Needs.Linq;
using NtErp.Wss.Sales.Services.Model;
using NtErp.Wss.Sales.Services.Models.SsoUsers;
using NtErp.Wss.Sales.Services.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Views
{
    public class UserInputsView : QueryView<UserInput, Layer.Data.Sqls.BvOrdersReponsitory>
    {

        string userid;
        UserAccountType type;
        public UserInputsView()
        {
        }
        public UserInputsView(string userid)
        {
            this.userid = userid;
        }
        public UserInputsView(string userid, UserAccountType type)
        {
            this.userid = userid;
            this.type = type;
        }

        protected override IQueryable<UserInput> GetIQueryable()
        {
            var linqs = from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvOrders.UserInputs>()
                        select new UserInput
                        {
                            ID = entity.ID,
                            UserID = entity.UserID,
                            Amount = entity.Amount,
                            Currency = (Currency)entity.Currency,
                            Code = entity.Code,
                            Source = (Models.SsoUsers.InputSource)entity.Source,
                            Type = (Models.SsoUsers.UserAccountType)entity.Type,
                            CreateDate = entity.CreateDate
                        };
            if (string.IsNullOrEmpty(this.userid))
            {
                return linqs;
            }
            else
            {
                linqs = linqs.Where(t => t.UserID == this.userid);
                if (this.type != UserAccountType.Unknown)
                {
                    linqs = linqs.Where(t => t.Type == this.type);
                }
                return linqs;
            }
        }
    }
}
