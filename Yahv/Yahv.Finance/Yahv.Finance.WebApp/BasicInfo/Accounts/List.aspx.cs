using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Finance.Services.Enums;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.BasicInfo.Accounts
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //金库名称
                var goldStores = Yahv.Erp.Current.Finance.GoldStores.Where(t => t.Status == GeneralStatus.Normal).ToArray();
                this.Model.GoldStores = goldStores.Select(item => new { value = item.ID, text = item.Name });

                //所属公司
                var enterprises = Yahv.Erp.Current.Finance.Enterprises.Where(t => t.Status == GeneralStatus.Normal).ToArray();
                this.Model.Enterprises = enterprises.Select(item => new { value = item.ID, text = item.Name });

                //状态
                Dictionary<string, string> dic_status = new Dictionary<string, string>();
                dic_status.Add("0", "全部");
                dic_status.Add(Underly.GeneralStatus.Normal.GetHashCode().ToString(), "正常");
                dic_status.Add(Underly.GeneralStatus.Closed.GetHashCode().ToString(), "停用");
                this.Model.Statuses = dic_status.Select(item => new { value = item.Key, text = item.Value });

                //账户性质
                Dictionary<string, string> dic_naturetype = new Dictionary<string, string>();
                dic_naturetype.Add("0", "全部");
                dic_naturetype.Add(NatureType.Public.GetHashCode().ToString(), NatureType.Public.GetDescription());
                dic_naturetype.Add(NatureType.Private.GetHashCode().ToString(), NatureType.Private.GetDescription());
                this.Model.NatureType = dic_naturetype.Select(item => new { value = item.Key, text = item.Value });

                //币种
                this.Model.Currency = ExtendsEnum.ToDictionary<Currency>().Select(item => new { value = item.Key, text = item.Value });
                //类型
                this.Model.Types = ExtendsEnum.ToDictionary<EnterpriseAccountType>().Select(item => new { value = item.Key, text = item.Value });
            }
        }

        protected object data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string goldstore_name = Request.QueryString["s_goldstore_name"];
            string code = Request.QueryString["s_code"];
            string enterprise_name = Request.QueryString["s_enterprise_name"];
            string status = Request.QueryString["s_status"];
            string name = Request.QueryString["s_name"];
            string naturetype = Request.QueryString["s_naturetype"];
            string currency = Request.QueryString["s_currency"];
            string accountType = Request.QueryString["s_ep_accountType"];

            using (var query1 = Erp.Current.Finance.Accounts)
            {
                var view = query1;

                //如果不是超级管理员，只能看到自己管理的账户，或者管理员为空的账户
                if (!Erp.Current.IsSuper)
                {
                    view = view.SearchByOwnerID(Erp.Current.ID);
                }

                if (!string.IsNullOrWhiteSpace(goldstore_name))
                {
                    view = view.SearchByGoldStoreName(goldstore_name);
                }
                if (!string.IsNullOrWhiteSpace(code))
                {
                    view = view.SearchByCode(code);
                }
                if (!string.IsNullOrWhiteSpace(enterprise_name))
                {
                    view = view.SearchByEnterpriseName(enterprise_name);
                }
                if (!string.IsNullOrWhiteSpace(status))
                {
                    if (status != "0")
                    {
                        view = view.SearchByStatus((GeneralStatus)(int.Parse(status)));
                    }
                }
                if (!string.IsNullOrWhiteSpace(name))
                {
                    view = view.SearchByName(name);
                }
                if (!string.IsNullOrWhiteSpace(naturetype))
                {
                    if (naturetype != "0")
                    {
                        view = view.SearchByNatureType((NatureType)(int.Parse(naturetype)));
                    }
                }
                if (!string.IsNullOrWhiteSpace(currency))
                {
                    view = view.SearchByCurrency((Currency)(int.Parse(currency)));
                }

                if (!string.IsNullOrWhiteSpace(accountType))
                {
                    view = view.SearchByEpAccountType(GetEpAccountType(accountType));
                }

                return view.ToMyPage(page, rows).Json();
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected void enable()
        {
            var array = Request.Form["items"].Split(',');
            Erp.Current.Finance.Accounts.Enable(array);
            Services.Oplogs.Oplog(Erp.Current.ID, LogModular.账户管理, Services.Oplogs.GetMethodInfo(), "启用", Request.Form["items"]);
            Response.Write((new { success = true, message = "启用成功", }).Json());
        }

        /// <summary>
        /// 停用
        /// </summary>
        protected void disable()
        {
            var array = Request.Form["items"].Split(',');
            Erp.Current.Finance.Accounts.Disable(array);
            Services.Oplogs.Oplog(Erp.Current.ID, LogModular.账户管理, Services.Oplogs.GetMethodInfo(), "停用", Request.Form["items"]);
            Response.Write((new { success = true, message = "停用成功", }).Json());
        }

        /// <summary>
        /// 根据字符串返回 组合枚举
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        EnterpriseAccountType GetEpAccountType(string values)
        {
            EnterpriseAccountType epType = default(EnterpriseAccountType);
            foreach (var type in values.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                epType |= (EnterpriseAccountType)int.Parse(type);
            }

            return epType;
        }
    }
}