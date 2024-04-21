using System;
using System.Linq;
using Yahv.Erm.Services;
using Yahv.Web.Erp;
using Yahv.Underly;
using Yahv.Linq.Extends;
using Yahv.Utils.Serializers;

namespace Yahv.Erm.WebApp.Erm.WageItems
{
    /// <summary>
    /// 列表展示页面
    /// </summary>
    public partial class List : ErpParticlePage
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
            var name = Request.QueryString["Name"];
            var type = Request.QueryString["ItemType"];

            var wageitems = Alls.Current.WageItems.Where(item => true);

            if (!string.IsNullOrWhiteSpace(name))
            {
                wageitems = wageitems.Where(item => item.Name.Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(type) && type != "0")
            {
                var t = (WageItemType)(int.Parse(type));
                wageitems = wageitems.Where(item => item.Type == t);
            }

            return new
            {
                rows = wageitems.OrderBy(t => t.OrderIndex).ToArray().Select(item => new
                {
                    item.ID,
                    item.Name,
                    item.OrderIndex,
                    //IsCalc = (item.IsCalc == true) ? "是" : "否",
                    Type = item.Type.GetDescription(),
                    IsImport = (item.IsImport == true) ? "是" : "否",
                    item.Status,
                    StatusName = item.Status.GetDescription(),
                    CreateDate = item.CreateDate.ToString("yyyy-MM-dd hh:mm:ss"),
                    item.AdminID,
                    item.AdminName,
                    item.InputerName,
                })
            };
        }

        protected object getTypes()
        {
            var result = ExtendsEnum.ToDictionary<WageItemType>().Select(item => new { value = item.Key, text = item.Value }).ToList();
            result.Insert(0, new { value = "", text = "全部" });

            return result;
        }


        /// <summary>
        /// 删除工资项
        /// </summary>
        protected void delete()
        {
            var array = Request.Form["ids"].Split(',');
            Alls.Current.WageItems.Delete(array);

            Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "工资项管理",
                    $"删除工资项", array.Json());
        }

        protected void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                if (fileUpload.HasFile)
                {
                    string filePath = Server.MapPath("~/Upload/Template/");
                    string fileFullName = filePath + "template.xls";
                    fileUpload.SaveAs(fileFullName);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}