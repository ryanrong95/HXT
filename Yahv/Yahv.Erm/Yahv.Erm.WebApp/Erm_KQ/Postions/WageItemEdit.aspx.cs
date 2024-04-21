using System;
using System.Linq;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm_KQ.Postions
{
    public partial class WageItemEdit : ErpParticlePage
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
            var positionid = Request.QueryString["id"];

            //获取当前所有工资项
            var wageitems = Alls.Current.WageItems.Where(item => true);
            var PositionItems = Alls.Current.Postions.PositionWages(positionid);

            return new
            {
                rows = wageitems.OrderBy(t => t.CreateDate).ToArray().Select(item => new
                {
                    item.ID,
                    item.Name,
                    IsChecked = PositionItems.Contains(item.ID),
                }),
            };
        }

        /// <summary>
        /// 保存事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var id = Request.QueryString["id"];
                var wageids = this.hCheckId.Value.Split(',');
                var wages = Alls.Current.WageItems.Where(item => wageids.Contains(item.ID)).ToArray();
                Alls.Current.Postions.BindingWages(id, wages);

                //更新岗位下员工的工资项
                Alls.Current.Postions.RefreshStaffWageItem(id);

                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                    nameof(Yahv.Systematic.Erm),
                    "岗位管理",
                    $"修改岗位：{id}",
                    wages.Json());
            }
            catch (Exception ex)
            {
                Easyui.Alert("操作提示", ex.Message, Web.Controls.Easyui.Sign.Error);
            }
            Easyui.Reload("操作提示", "操作成功!", Web.Controls.Easyui.Sign.None);
        }

    }
}