using System;
using System.Linq;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Usually;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm.Postions
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        /// <summary>
        /// 数据查询
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            //获取当前所有工资项
            var wageitems = Alls.Current.WageItems.Where(item => true);

            return new
            {
                rows = wageitems.OrderBy(t => t.CreateDate).ToArray().Select(item => new
                {
                    item.ID,
                    item.Name,
                })
            };
        }

        /// <summary>
        /// 保存事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Postion model;

            if (!string.IsNullOrWhiteSpace(Request.QueryString["id"]))
            {
                model = new Postion()
                {
                    Name = Request.Form["Name"].Trim(),
                    AdminID = Erp.Current.ID,
                };

                model.EnterError += EnterError;
                model.EnterSuccess += EnterSuccess;
                model.Enter();
            }
        }

        private void EnterError(object sender, ErrorEventArgs e)
        {
            Easyui.Alert("操作提示", e.Message, Web.Controls.Easyui.Sign.Error);
        }

        private void EnterSuccess(object sender, SuccessEventArgs e)
        {
            BindingWages(e.Object as Postion);

            //Easyui.Reload("操作提示", "操作成功!", Web.Controls.Easyui.Sign.None);
            Easyui.Dialog.Close("操作成功!", Web.Controls.Easyui.AutoSign.Success);

        }

        /// <summary>
        /// 分配工资项
        /// </summary>
        /// <param name="PositionID"></param>
        protected void BindingWages(Postion postion)
        {
            if (string.IsNullOrWhiteSpace(this.hCheckId.Value))
            {
                return;
            }
            var wageids = this.hCheckId.Value.Split(',');
            var wages = Alls.Current.WageItems.Where(item => wageids.Contains(item.ID)).ToArray();
            Alls.Current.Postions.BindingWages(postion.ID, wages);

            //更新岗位下员工的工资项
            Alls.Current.Postions.RefreshStaffWageItem(postion.ID);



            Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                nameof(Yahv.Systematic.Erm),
                "岗位管理",
                $"添加岗位：{postion.Name}",
                wages.Json());
        }
    }
}