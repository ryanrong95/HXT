using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.BasicInfo.Persons
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Dictionary<string, string> dicDistricts = new Dictionary<string, string>();
                foreach (Origin item in Enum.GetValues(typeof(Origin)))
                {
                    dicDistricts.Add(item.ToString(), item.GetDescription());
                }
                this.Model.Districts = dicDistricts.Select(item => new { value = item.Key, text = item.Value });

                Dictionary<string, string> dic_status = new Dictionary<string, string>();
                dic_status.Add("", "全部");
                dic_status.Add(Underly.GeneralStatus.Normal.GetHashCode().ToString(), "正常");
                dic_status.Add(Underly.GeneralStatus.Closed.GetHashCode().ToString(), "停用");
                this.Model.Statuses = dic_status.Select(item => new { value = item.Key, text = item.Value });
            }
        }

        protected object data()
        {
            var query = Erp.Current.Finance.Persons.Where(GetExpression()).ToArray();
            return this.Paging(query.OrderBy(item => item.Status).ThenByDescending(item => item.CreateDate), item => new
            {
                item.ID,
                item.RealName,
                Gender = item.Gender?.GetDescription(),
                item.IDCard,
                item.Mobile,
                item.CreatorName,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                StatusName = item.Status.GetDescription(),
            });
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected void enable()
        {
            var array = Request.Form["items"].Split(',');
            Erp.Current.Finance.Persons.Enable(array);
            Services.Oplogs.Oplog(Erp.Current.ID, LogModular.人员管理, Services.Oplogs.GetMethodInfo(), "启用", Request.Form["items"]);
            Response.Write((new { success = true, message = "启用成功", }).Json());
        }

        /// <summary>
        /// 停用
        /// </summary>
        protected void disable()
        {
            var array = Request.Form["items"].Split(',');
            Erp.Current.Finance.Persons.Disable(array);
            Services.Oplogs.Oplog(Erp.Current.ID, LogModular.人员管理, Services.Oplogs.GetMethodInfo(), "停用", Request.Form["items"]);
        }

        #region 私有函数
        private Expression<Func<Person, bool>> GetExpression()
        {
            Expression<Func<Person, bool>> predicate = item => true;

            string name = Request.QueryString["s_name"];
            string status = Request.QueryString["s_status"];

            //名称或手机号
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.RealName.Contains(name) || item.Mobile.Contains(name));
            }
            //状态
            if (!string.IsNullOrWhiteSpace(status))
            {
                predicate = predicate.And(item => item.Status == (GeneralStatus)int.Parse(status));
            }

            return predicate;
        }
        #endregion
    }
}