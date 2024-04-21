using System;
using System.Linq;
using Yahv.Underly;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm.WageItems
{
    public partial class Order : ErpParticlePage
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
            var wageitems = Alls.Current.WageItems.Where(item => true);

            return new
            {
                rows = wageitems.OrderBy(t => t.OrderIndex).ToArray().Select(item => new
                {
                    item.ID,
                    item.Name,
                    item.OrderIndex,
                    item.Description,
                    item.Status,
                    StatusName = item.Status.GetDescription(),
                    CreateDate = item.CreateDate.ToString("yyyy-MM-dd hh:mm:ss"),
                    item.AdminID,
                    item.AdminName,
                }),
            };
        }


        /// <summary>
        /// 保存修改后的顺序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                var ids = this.hidids.Value.Split(',');
                var orderindexs = this.hidIndexs.Value.Split(',');
                if (ids.Count() == 0)
                {
                    return;
                }
                Alls.Current.WageItems.Order(ids, orderindexs);

                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "修改工资项排序",
                    $"修改工资项排序", string.Empty);
            }
            catch (Exception ex)
            {
                Easyui.Alert("操作提示", ex.Message, Web.Controls.Easyui.Sign.Error);
            }
            //Easyui.Reload("操作提示", "操作成功!", Web.Controls.Easyui.Sign.None);
            Easyui.Dialog.Close("操作成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}