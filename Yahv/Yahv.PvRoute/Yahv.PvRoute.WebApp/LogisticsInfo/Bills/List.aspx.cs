using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Erm.Services;
using Yahv.PvRoute.Services;
using Yahv.PvRoute.Services.Enums;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Controls.Easyui;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.PvRoute.WebApp.LogisticsInfo.Bills
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //状态
                Dictionary<string, string> dic_status = new Dictionary<string, string>();
                dic_status.Add("0", "全部");
                dic_status.Add(PrintSource.SF.GetHashCode().ToString(), PrintSource.SF.GetDescription());
                dic_status.Add(PrintSource.KYSY.GetHashCode().ToString(), PrintSource.KYSY.GetDescription());
                this.Model.Sources = dic_status.Select(item => new { value = item.Key, text = item.Value });
            }
        }

        protected object data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string faceOrderID = Request.QueryString["faceOrderID"];
            string source = Request.QueryString["source"];
            string dateIndex = Request.QueryString["dateIndex"];
            using (var query = Erp.Current.PvRoute.Bills)
            {
                var view = query;

                if (!string.IsNullOrWhiteSpace(faceOrderID))
                {
                    view = view.SearchByFaceOrderID(faceOrderID);
                }
                if (!string.IsNullOrWhiteSpace(source))
                {
                    if (source != "0")
                    {
                        view = view.SearchByCarrier((PrintSource)(int.Parse(source)));
                    }
                }
                if (!string.IsNullOrWhiteSpace(dateIndex))
                {
                    view = view.SearchByDateIndex(int.Parse(dateIndex));
                }
                return view.ToMyPage(page, rows).Json();
            }
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnImport_Click(object sender, EventArgs e)
        {
            string fileFullName = UploadFile();     //上传文件

            if (string.IsNullOrWhiteSpace(fileFullName))
            {
                Easyui.Alert("操作提示", "上传文件失败!", Sign.Error);
                return;
            }

            var dt = UploadStaffs.Current.ExcelToTable(fileFullName);
            //判断是否包含关键列
            if (!dt.Columns.Contains("快递单号") ||
                !dt.Columns.Contains("件数") ||
                !dt.Columns.Contains("重量") ||
                !dt.Columns.Contains("金额") ||
                !dt.Columns.Contains("币种") ||
                !dt.Columns.Contains("承运商") ||
                !dt.Columns.Contains("期号"))
            {
                Easyui.Alert("操作提示", "导入文件格式不正确!", Sign.Error);
                return;
            }

            foreach (DataRow dr in dt.Rows)
            {
                PrintSource carrier = PrintSource.UKnown;
                if (dr["承运商"].ToString().Contains("顺丰"))
                {
                    carrier = PrintSource.SF;
                }

                if (dr["承运商"].ToString().Contains("跨越速运"))
                {
                    carrier = PrintSource.KYSY;
                }

                try
                {
                    DateTime? checkTime=null;//核对时间
                    DateTime _checkTime;

                    if(DateTime.TryParse(dr["核对时间"].ToString(), out _checkTime))
                    {
                        checkTime = DateTime.Parse(dr["核对时间"].ToString());
                    }

                    DateTime? reviewTime = null;//审核时间
                    DateTime _reviewTime;

                    if (DateTime.TryParse(dr["审核时间"].ToString(), out _reviewTime))
                    {
                        reviewTime = DateTime.Parse(dr["审核时间"].ToString());
                    }

                    DateTime? cashierTime = null;//出纳时间
                    DateTime _cashierTime;

                    if (DateTime.TryParse(dr["出纳时间"].ToString(), out _cashierTime))
                    {
                        cashierTime = DateTime.Parse(dr["出纳时间"].ToString());
                    }
                    new Services.Models.Bill
                    {
                        FaceOrderID = dr["快递单号"].ToString(),
                        Quantity = Convert.ToInt32(dr["件数"].ToString()),
                        Weight = Convert.ToDecimal(dr["重量"].ToString()),
                        Price = Convert.ToDecimal(dr["金额"].ToString()),
                        Currency = (Currency)(int.Parse(dr["币种"].ToString())),
                        Carrier = carrier,
                        Checker = dr["核对人"]?.ToString(),
                       
                        CheckTime = checkTime,
                        Reviewer = dr["审核人"]?.ToString(),
                        ReviewTime = reviewTime,
                        Cashier = dr["出纳人"]?.ToString(),
                        CashierTime = cashierTime,
                        DateIndex = int.Parse(dr["期号"].ToString()),
                        Source = RecordSource.Ours
                    }.Enter();
                }
                catch (Exception ex)
                {

                    throw;
                }
            }

        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExport_Click(object sender, EventArgs e)
        {
            string _faceOrderID = faceOrderID?.Value?? Request.QueryString["faceOrderID"];
            string _source = s_carrier?.Value?? Request.QueryString["source"];
            string _dateIndex = dateIndex?.Value?? Request.QueryString["dateIndex"];
            //string faceOrderID =  Request.QueryString["faceOrderID"];
            //string source = Request.QueryString["source"];
            //string dateIndex = Request.QueryString["dateIndex"];

            using (var query = Erp.Current.PvRoute.Bills)
            {
                var view = query;

                if (!string.IsNullOrWhiteSpace(_faceOrderID))
                {
                    view = view.SearchByFaceOrderID(_faceOrderID);
                }
                if (!string.IsNullOrWhiteSpace(_source))
                {
                    if (_source != "0")
                    {
                        view = view.SearchByCarrier((PrintSource)(int.Parse(_source)));
                    }
                }
                if (!string.IsNullOrWhiteSpace(_dateIndex))
                {
                    view = view.SearchByDateIndex(int.Parse(_dateIndex));
                }
                if (view == null || !view.Any())
                {
                    Easyui.Alert("操作提示", "未找到数据!", Sign.Error);
                    return;
                }

                var dataTable = ExportBills.Current.JsonToDataTable(view.Json());
                if (dataTable == null || dataTable.Rows.Count <= 0)
                {
                    Easyui.Alert("操作提示", "未找到数据!", Sign.Error);
                    return;
                }

                var bills = new List<string>()
            {
               "快递单号","件数","重量", "金额","币种","承运商","核对人","核对时间","审核人","审核时间","出纳人","出纳时间","期号"
            };

                string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                string files = ExportBills.Current.MakeExportExcel(fileName, dataTable.Select(), dataTable.Columns, bills);

                DownLoadFile(files);

            }

        }

        /// <summary>
        /// 模板下载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDownload_Click(object sender, EventArgs e)
        {
            List<string> myItems = new List<string>
            {
                "快递单号",
                "件数",
                "重量",
                "金额",
                "币种",
                "承运商",
                "核对人",
                "核对时间",
                "审核人",
                "审核时间",
                 "出纳人",
                "出纳时间",
                "期号"
            };
            var fileName = ExportBills.Current.MakeTemplateExcel(myItems.ToArray());
            DownLoadFile(fileName);
        }


        #region 私有方法
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        private string UploadFile()
        {
            string fileFullName = string.Empty; //上传文件地址

            try
            {
                if (fileUpload.HasFile)
                {
                    string filePath = Server.MapPath("~/Upload/");
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string extension = Path.GetExtension(fileUpload.PostedFile.FileName); //获取扩展名
                    fileFullName = filePath + fileName + extension;

                    fileUpload.SaveAs(fileFullName);
                }
            }
            catch (Exception)
            {
                fileFullName = string.Empty;
            }

            return fileFullName;
        }

        private List<Yahv.PvRoute.Services.Models.Bill> GetList(DataTable dt)
        {
            List<Yahv.PvRoute.Services.Models.Bill> list = new List<Services.Models.Bill>();
            foreach (DataRow dr in dt.Rows)
            {
                PrintSource carrier = PrintSource.UKnown;
                if (dr["承运商"].ToString().Contains("顺丰"))
                {
                    carrier = PrintSource.SF;
                }

                if (dr["承运商"].ToString().Contains("跨越速运"))
                {
                    carrier = PrintSource.KYSY;
                }
                list.Add(new Services.Models.Bill
                {
                    FaceOrderID = dr["快递单号"].ToString(),
                    Quantity = int.Parse(dr["件数"].ToString()),
                    Weight = Convert.ToDecimal(dr["重量"].ToString()),
                    Price = Convert.ToDecimal(dr["金额"].ToString()),
                    Currency = (Currency)(int.Parse(dr["币种"].ToString())),
                    Carrier = carrier,
                    Checker = dr["核对人"].ToString(),
                    CheckTime = DateTime.Parse(dr["核对时间"].ToString()),
                    Reviewer = dr["审核人"].ToString(),
                    ReviewTime = DateTime.Parse(dr["审核时间"].ToString()),
                    Cashier = dr["出纳人"].ToString(),
                    CashierTime = DateTime.Parse(dr["出纳时间"].ToString()),
                    DateIndex = int.Parse(dr["期号"].ToString()),
                    Source = RecordSource.Ours
                });
            }

            return null;
        }

        
        #endregion
    }
}