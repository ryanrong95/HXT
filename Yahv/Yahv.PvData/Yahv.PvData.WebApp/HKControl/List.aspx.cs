using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Yahv.Linq.Extends;
using Yahv.Utils.Npoi;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvData.WebApp.HKControl
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        protected object data()
        {
            var view = Yahv.Erp.Current.PvData.HKControls.Where(query());

            return this.Paging(view, item => new
            {
                item.ID,
                item.Brand,
                item.Model,
                item.Type,
                isControl = item.isControl ? "是" : "否",
                item.Description,
                item.Status,
                CreateDate = item.CreateDate.ToShortDateString(),
                UpdateDate = item.UpdateDate.ToShortDateString()

            });
        }

        /// <summary>
        /// 获取搜索查询条件
        /// </summary>
        /// <returns></returns>
        private Expression<Func<YaHv.PvData.Services.Models.HKControl, bool>> query()
        {
            #region 搜索条件
            var predicate = PredicateExtends.True<YaHv.PvData.Services.Models.HKControl>(); // 条件拼接

            string brand = Request["Brand"];
            string model = Request["Model"];
            string isControl = Request["IsControl"];
            if (!string.IsNullOrEmpty(brand))
            {
                predicate = predicate.And(item => item.Brand.StartsWith(brand.Trim()));
            }
            if (!string.IsNullOrEmpty(model))
            {
                predicate = predicate.And(item => item.Model.StartsWith(model.Trim()));
            }

            if (!string.IsNullOrEmpty(isControl) && isControl != "-100")
            {
                var result = isControl == "1";
                predicate = predicate.And(item => item.isControl == result);
            }

            return predicate;
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected void ImportHK()
        {
            HttpPostedFile file = Request.Files["uploadExcel"];
            string ext = Path.GetExtension(file.FileName);
            if (ext != ".xls" && ext != ".xlsx")
            {
                Response.Write((new { success = false, message = "文件格式错误，请上传.xls或.xlsx文件！" }).Json());
                return;
            }
            string filePath = Server.MapPath("~/Upload/");
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            //读取Excel
            // string file = Path.Combine(this.MonitoringPath, this.MonitoringFile);
            IWorkbook workbook = ExcelFactory.CreateByTemplate(filePath);
            foreach (var item in workbook)
            {

            }

            var npoi = new NPOIHelper(workbook);
            var dt = npoi.ExcelToDataTable(false);
            var rows = dt.Rows;

        }

    }
}
