using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Yahv.CrmPlus.Service;

using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.Company.BookAccounts
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)

        {
            if (!IsPostBack)
            {
                this.Model.ID = Request.QueryString["id"];
                Dictionary<string, string> list = new Dictionary<string, string>() { { "0", "全部" } };

                this.Model.DataStatus = list.Concat(ExtendsEnum.ToDictionary<DataStatus>()).Select(item => new
                {
                    value = int.Parse(item.Key),
                    text = item.Value.ToString()
                });


                Dictionary<string, string> dic = new Dictionary<string, string>() { {"0" ,"全部"} };
                this.Model.BookAccountType = dic.Concat(ExtendsEnum.ToDictionary<BookAccountType>()).Select(item => new
                {
                    value = int.Parse(item.Key),
                    text = item.Value.ToString()

                });
            }
        }




        protected object data()
        {
            string id = Request.QueryString["id"];
            var query = Erp.Current.CrmPlus.BookAccounts[id,RelationType.Own].Where(Predicate());

            return this.Paging(query, item => new
            {
                item.ID,
                item.Bank,
                item.Account,
                item.BankAddress,
                nature = item.IsPersonal == false ? Nature.Company.GetDescription() : Nature.Person.GetDescription(),
                item.BankCode,
                BookAccountType = item.BookAccountType.GetDescription(),
                BookAccountMethord = item.BookAccountMethord.GetDescription(),
                item.Status,
                StatusName = item.Status.GetDescription(),
                Creator = item.Admin.RealName,
            });

        }


        Expression<Func<Service.Models.Origins.BookAccount, bool>> Predicate()
        {
            Expression<Func<Yahv.CrmPlus.Service.Models.Origins.BookAccount, bool>> predicate = item => true;
            var name = Request["s_name"];
            var status = Request["selStatus"];
            var accountType = Request["bookAccountType"];
            var mobile = Request["Mobile"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Account.Contains(name));
            }
            ////账号类型
            BookAccountType bookAccountType;
            if (Enum.TryParse(accountType, out bookAccountType) && bookAccountType != 0)
            {
                predicate = predicate.And(item => item.BookAccountType == bookAccountType);
            }

            DataStatus dataStatus;
            if (Enum.TryParse(status, out dataStatus) && dataStatus != 0)
            {
                predicate = predicate.And(item => item.Status == dataStatus);
            }
            return predicate;
        }



        /// <summary>
        /// 停用
        /// </summary>
        protected void Closed()
        {
            var id = Request.Form["ID"];
            var companyID = Request.Form["CompanyID"];
            try
            {
                var entity = Erp.Current.CrmPlus.BookAccounts[id];
                entity.Closed();
                LogsOperating.LogOperating(Erp.Current, entity.Enterprise.ID, $"停用收付款账号:{ entity.ID}");
            }
            catch (Exception ex)
            {
                LogsOperating.LogOperating(Erp.Current, companyID, $"停用收付款账号 操作失败" + ex);
            }
        }


        /// <summary>
        /// 启用
        /// </summary>
        protected void Enable()
        {
            var id = Request.Form["ID"];
            var companyID = Request.Form["CompanyID"];
            try
            {
                var entity = Erp.Current.CrmPlus.BookAccounts[id];
                entity.Enable();
                LogsOperating.LogOperating(Erp.Current, entity.Enterprise.ID, $"启用收付款账号:{ entity.ID}");
            }
            catch (Exception ex)
            {
                LogsOperating.LogOperating(Erp.Current, companyID, $"启用收付款账号 操作失败" + ex);
            }
        }
    }
}