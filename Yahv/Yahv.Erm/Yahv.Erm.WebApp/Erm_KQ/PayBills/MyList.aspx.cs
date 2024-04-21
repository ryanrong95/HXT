using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Models.Rolls;
using Yahv.Linq.Extends;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Erm.WebApp.Erm_KQ.PayBills
{
    public partial class MyList : ErpParticlePage
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
            string[] companys = new string[]
            {
                 ConfigurationManager.AppSettings["LabourEnterpriseName"],
                 ConfigurationManager.AppSettings["LabourEnterpriseName2"]
            };

            IQueryable<StaffPayItem> data;
            var staff = Alls.Current.Staffs.FirstOrDefault(item => item.ID == Erp.Current.StaffID);

            if (companys.Contains(staff.EnterpriseCompany))
            {
                data = Alls.Current.PayItems.Where(item => item.StaffID == Erp.Current.StaffID
               && item.Status == (int)PayBillStatus.Closed && companys.Contains(item.CompanyName));
            }
            else
            {
                data = Alls.Current.PayItems.Where(item => item.StaffID == Erp.Current.StaffID
               && item.Status == (int)PayBillStatus.Closed);
            }


            data = data.Where(predicate);
            List<string> dynColumns;
            if (!data.Any())
            {
                return null;
            }
            var dataList = data.OrderByDescending(item => item.DateIndex).ToList();
            return ExportWages.Current.DynamicLinq(dataList, GetFixedColumns(), "Name", "Value", out dynColumns);
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
        protected string GetColNames()
        {
            var list = new List<dynamic>()
            {
                new {field="DateIndex",title="工资日期",width=65},
                 new {field="StaffName",title="姓名",width=60},
                new {field="PostionName",title="考核岗位",width=80},
                //new {field="CompanyName",title="所属公司"},
                new {field="City",title="地区",width=50},
                //new {field="DyjCompanyCode",title="分公司"},
                //new {field="DyjDepartmentCode",title="部门"},
                //new {field="DyjCode",title="ID（工号）",width=80},
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

            return new
            {
                rows = result
            };
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
            var result = staffPayItems.Where(item => item.StaffID == Erp.Current.StaffID).GroupBy(item => item.Name).Select(item => item.Key).ToList();
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


            //工资日期
            if (!string.IsNullOrWhiteSpace(Request.QueryString["s_wageDate"]))
            {
                string date = Request.QueryString["s_wageDate"].Replace("-", "");

                predicate = predicate.And(item => item.DateIndex == date);
            }

            //姓名、工号
            if (!string.IsNullOrWhiteSpace(Request.QueryString["s_name"]))
            {
                predicate =
                    predicate.And(item => item.StaffName.Contains(Request.QueryString["s_name"]) || item.DyjCode == Request.QueryString["s_name"]);
            }


            return predicate;
        }
    }
}