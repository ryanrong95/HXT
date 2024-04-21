using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Finance.Services;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Models.Rolls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Underly.Enums.PvFinance;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.BasicInfo.GoldStores
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Dictionary<string, string> dic_status = new Dictionary<string, string>();
                dic_status.Add(Underly.GeneralStatus.Normal.GetHashCode().ToString(), "正常");
                dic_status.Add(Underly.GeneralStatus.Closed.GetHashCode().ToString(), "停用");
                this.Model.Statuses = dic_status.Select(item => new { value = item.Key, text = item.Value });
            }
        }

        protected object data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string name = Request.QueryString["s_name"];
            string status = Request.QueryString["s_status"];

            using (var query1 = Erp.Current.Finance.GoldStores)
            {
                var view = query1;

                view = view.SearchByIsSpecial(false);

                if (!string.IsNullOrWhiteSpace(name))
                {
                    view = view.SearchByName(name);
                }

                if (!string.IsNullOrWhiteSpace(status))
                {
                    view = view.SearchByStatus((Underly.GeneralStatus)(Convert.ToInt32(status)));
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
            var goldStores = Erp.Current.Finance.GoldStores;
            goldStores.EnableSuccess += GoldStores_EnableSuccess;
            goldStores.Enable(array);
            Services.Oplogs.Oplog(Erp.Current.ID, LogModular.金库管理, Services.Oplogs.GetMethodInfo(), "启用", Request.Form["items"], url: Request.Url.ToString());
            Response.Write((new { success = true, message = "启用成功", }).Json());
        }

        /// <summary>
        /// 停用
        /// </summary>
        protected void disable()
        {
            var array = Request.Form["items"].Split(',');
            var goldStores = Erp.Current.Finance.GoldStores;
            goldStores.DisableSuccess += GoldStores_DisableSuccess;
            goldStores.Disable(array);
            Services.Oplogs.Oplog(Erp.Current.ID, LogModular.金库管理, Services.Oplogs.GetMethodInfo(), "停用", Request.Form["items"], url: Request.Url.ToString());
            Response.Write((new { success = true, message = "停用成功", }).Json());
        }

        private void GoldStores_DisableSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var ids = sender as string[];
            if (ids.Length > 0)
            {
                var url = XdtApi.GoldStoreEnter.GetApiAddress();
                var array = Erp.Current.Finance.GoldStores.Where(item => ids.Contains(item.ID)).ToArray();

                var data = new InputParam<GoldStoreInputDto>()
                {
                    Sender = SystemSender.Xindatong.GetFixedID(),
                    Option = OptionConsts.delete,
                };

                string result = string.Empty;
                foreach (var goldStore in array)
                {
                    data.Model = new GoldStoreInputDto() { OriginName = goldStore.Name, CreatorID = Erp.Current.ID };
                    result = Yahv.Utils.Http.ApiHelper.Current.JPost(url, data);
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.金库管理, Services.Oplogs.GetMethodInfo(), "Api 停用", result + data.Json(), url: url);
                }
            }
        }

        private void GoldStores_EnableSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var ids = sender as string[];
            if (ids.Length > 0)
            {
                var url = XdtApi.GoldStoreEnter.GetApiAddress();
                var array = Erp.Current.Finance.GoldStores.Where(item => ids.Contains(item.ID)).ToArray();

                var data = new InputParam<GoldStoreInputDto>()
                {
                    Sender = SystemSender.Xindatong.GetFixedID(),
                    Option = OptionConsts.insert,
                };

                string result = string.Empty;
                foreach (var goldStore in array)
                {
                    data.Model = new GoldStoreInputDto() { Name = goldStore.Name, CreatorID = Erp.Current.ID, Summary = goldStore.Summary, OwnerID = goldStore.OwnerID };
                    result = Yahv.Utils.Http.ApiHelper.Current.JPost(url, data);
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.金库管理, Services.Oplogs.GetMethodInfo(), "Api 启用", result + data.Json(), url: url);
                }
            }
        }
    }
}