using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service;
using Yahv.CrmPlus.Service.Extends;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.Company
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            Dictionary<string, string> list = new Dictionary<string, string>() { { "0", "全部" } };
            //状态
            this.Model.Status = list.Concat(ExtendsEnum.ToDictionary<DataStatus>()).Select(item => new
            {
                value = int.Parse(item.Key),
                text = item.Value.ToString()
            });

        }

        protected object data()
        {
            var query = new CompaniesRoll().Where(Predicate());
            var result = this.Paging(query.OrderByDescending(item => item.CompanyCreateDate).ToArray().Select(item => new
            {
                item.ID,
                item.Name,
                Corperation = item.EnterpriseRegister.Corperation,
                item.EnterpriseRegister.Uscc,
                item.EnterpriseRegister.RegAddress,
                Status=item.CompanyStatus,
                StatusDes = item.CompanyStatus.GetDescription(),
                Creator = item.Creator?.RealName,
                CreteDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            }));

            return result;
        }


        Expression<Func<Yahv.CrmPlus.Service.Models.Origins.Company, bool>> Predicate()
        {
            Expression<Func<Yahv.CrmPlus.Service.Models.Origins.Company, bool>> predicate = item => true;
            var name = Request["s_name"];
            var status = Request["selStatus"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Name.Contains(name));
            }

            DataStatus dataStatus;
            if (Enum.TryParse(status, out dataStatus) && dataStatus != 0)
            {
                predicate = predicate.And(item => item.CompanyStatus == dataStatus);
            }
            return predicate;
        }



        /// <summary>
        /// 停用
        /// </summary>
        protected void Closed()
        {
            var arry = Request.Form["items"].Split(',');
            try
            {
                var entity = new CompaniesRoll().Where(t => arry.Contains(t.ID));
                entity.Close();
                foreach (var it in entity)
                {
                    LogsOperating.LogOperating(Erp.Current, it.ID, $"停用内部公司:{ it.Name}");
                }

            }
            catch (Exception ex)
            {

                LogsOperating.LogOperating(Erp.Current, arry.ToString(), $"停用内部公司 操作失败"+ex);
            }
        }


        /// <summary>
        /// 启用
        /// </summary>
        protected void Enable()
        {
            var arry = Request.Form["items"].Split(',');
            try
            {
                var entity = new CompaniesRoll().Where(t => arry.Contains(t.ID));
                entity.Enable();
                foreach (var it in entity)
                {
                    LogsOperating.LogOperating(Erp.Current, it.ID, $"启用了内部公司:{ it.Name}");
                }
            }
            catch (Exception ex)
            {

                LogsOperating.LogOperating(Erp.Current, arry.ToString(), $"启用内部公司失败" + ex);
            }
        }

    }
}