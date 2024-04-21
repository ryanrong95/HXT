using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Models.Rolls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Controls.Easyui;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm.PayBills
{
    public partial class AllList : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 获取handsontable数据源
        /// </summary>
        /// <returns></returns>
        public List<dynamic> GetData(Expression<Func<StaffPayItem, bool>> predicate)
        {
            var data = Alls.Current.PayItems.Where(predicate);
            List<string> dynColumns;
            if (!data.Any())
            {
                return null;
            }
            return ExportWages.Current.DynamicLinq(data.OrderBy(item => item.DateIndex).ThenBy(item => item.StaffName).ToList(), GetFixedColumns(), "Name", "Value", out dynColumns);
        }

        /// <summary>
        /// 获取固定列
        /// </summary>
        /// <returns></returns>
        private List<string> GetFixedColumns()
        {
            return new List<string>()
                    {
                        "PayID",
                        "StaffID",
                        "StaffName",
                        "DyjCode",
                        "DateIndex",
                        "Status",
                        "City",
                        "CompanyName",
                        "DyjCompanyCode",
                        "DyjDepartmentCode",
                        "PostionName",
                        "IDCard",
                        "StaffSelCode",
                    };
        }

        /// <summary>
        /// 列名
        /// </summary>
        /// <returns></returns>
        protected string getColNames()
        {
            var list = new List<dynamic>()
            {
                new {field="DateIndex",title="工资日期",width=65},
                 new {field="StaffName",title="姓名",width=60},
                new {field="PostionName",title="考核岗位",width=80},
                new {field="CompanyName",title="所属公司",width=100},
                new {field="City",title="地区",width=50},
                new {field="DyjCompanyCode",title="分公司",width=50},
                new {field="DyjDepartmentCode",title="部门",width=50},
                new {field="DyjCode",title="ID（工号）",width=80},
                //new {field="IDCard",title="身份证号码"},
            };

            var dynColumns = GetDynamicColumns();
            if (dynColumns != null)
            {
                var result = dynColumns.JsonTo<dynamic>();
                foreach (var d in result)
                {
                    list.Add(new { field = d.name, title = d.name, width = 16 * d.name.ToString().Length < 60 ? 60 : 16 * d.name.ToString().Length });
                }
            }

            return list.Json();
        }

        protected object data()
        {
            var result = GetData(GetExpression());
            if (result == null)
            {
                return new
                {
                    rows = ""
                };
            }
            return this.Paging(result, item => item);
        }

        /// <summary>
        /// 导出excel
        /// </summary>
        protected void btnExport_Click(object sender, EventArgs e)
        {
            var data = GetData(GetExpression());
            if (data == null || !data.Any())
            {
                Easyui.Alert("操作提示", "未找到数据!", Sign.Error);
                return;
            }
            var dataTable = ExportWages.Current.JsonToDataTable(data.Json());
            if (dataTable == null || dataTable.Rows.Count <= 0)
            {
                Easyui.Alert("操作提示", "未找到数据!", Sign.Error);
                return;
            }

            //根据模板文件，过滤一下标头
            var wageItems = Alls.Current.WageItems.OrderBy(item => item.OrderIndex).Select(item => item.Name).ToList();
            var tempHeads = ExportWages.Current.GetExcelHead(GetTemplate(), "1,2");
            wageItems = wageItems.Where(item => tempHeads.Contains(item)).ToList();

            string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            string files = ExportWages.Current.MakeExportExcel(fileName, dataTable.Select(), dataTable.Columns, wageItems, fixedColumns: "工资日期,姓名,考核岗位,所属公司,地区,分公司,部门,ID（工号）");

            Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "员工工资列表",
                $"Excel导出!", string.Empty);
            //下载文件
            DownLoadFile(files);
        }

        /// <summary>
        /// 获取动态列
        /// </summary>
        /// <returns></returns>
        private string GetDynamicColumns()
        {
            string dynColumns = string.Empty;
            var staffPayItems = Alls.Current.PayItems.Where(GetExpression());       //导入工资数据
            if (!staffPayItems.Any())
            {
                return null;
            }
            //获取我的工资项
            var result = staffPayItems.GroupBy(item => item.Name).Select(item => item.Key).ToList();
            //根据模板去掉其他工资项
            var heads = ExportWages.Current.GetExcelHead(Server.MapPath("~/Upload/Template/template.xls"), "1,2");
            result = result.Where(item => heads.Contains(item)).ToList();
            //获取工资项
            var wageitem = Alls.Current.WageItems.ToList();
            //排序
            var resultOrder = from r in result
                              join w in wageitem on r equals w.Name
                              orderby w.OrderIndex
                              select new { name = r, Type = w.Type };
            dynColumns = resultOrder.Json();
            return dynColumns;
        }

        /// <summary>
        /// 查询条件
        /// </summary>
        /// <returns></returns>
        public Expression<Func<StaffPayItem, bool>> GetExpression()
        {
            Expression<Func<StaffPayItem, bool>> predicate = t => true;

            //工资开始日期
            string begin = Request.Form["s_begin"] ?? Request.QueryString["s_begin"] ?? s_begin.Value;
            if (!string.IsNullOrWhiteSpace(begin))
            {
                begin = begin.Replace("-", "");
                predicate = predicate.And(item => String.Compare(item.DateIndex, begin, StringComparison.Ordinal) >= 0);
            }

            //工资开始日期
            string end = Request.Form["s_end"] ?? Request.QueryString["s_end"] ?? s_end.Value;
            if (!string.IsNullOrWhiteSpace(end))
            {
                end = end.Replace("-", "");
                predicate = predicate.And(item => String.Compare(item.DateIndex, end, StringComparison.Ordinal) <= 0);
            }

            //姓名、工号
            string name = Request.Form["s_name"] ?? Request.QueryString["s_name"] ?? s_name.Value;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate =
                    predicate.And(item => item.StaffName.Contains(name) || item.DyjCode == name);
            }


            return predicate;
        }



        private string GetTemplate()
        {
            return Server.MapPath("~/Upload/Template/template.xls");
        }
    }
}