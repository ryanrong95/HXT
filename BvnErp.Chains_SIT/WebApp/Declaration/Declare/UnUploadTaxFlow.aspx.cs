using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Declare
{
    public partial class UnUploadTaxFlow : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                load();
            }
        }

        private void load()
        {
            this.Model.Status = Needs.Ccs.Services.MultiEnumUtils.ToDictionary<Needs.Ccs.Services.Enums.CusDecStatus>()
                .Select(item => new { Value = item.Key, Text = item.Value }).Json();
        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string ContrNo = Request.QueryString["ContrNo"];
            string EntryID = Request.QueryString["EntryID"];
            string OrderID = Request.QueryString["OrderID"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            var predicate = PredicateBuilder.Create<Needs.Ccs.Services.Views.UnUploadTaxFlowListViewModel>();

            if (!string.IsNullOrEmpty(ContrNo))
            {
                ContrNo = ContrNo.Trim();
                predicate = predicate.And(item => item.ContrNo.Contains(ContrNo));
            }
            if (!string.IsNullOrEmpty(EntryID))
            {
                EntryID = EntryID.Trim();
                predicate = predicate.And(item => item.EntryID.Contains(EntryID));
            }

            if (!string.IsNullOrEmpty(OrderID))
            {
                OrderID = OrderID.Trim();
                predicate = predicate.And(item => item.OrderID.Contains(OrderID));
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                DateTime start = Convert.ToDateTime(StartDate);
                predicate = predicate.And(item => item.DDate >= start);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
                predicate = predicate.And(item => item.DDate < end);
            }

            Needs.Ccs.Services.Views.UnUploadTaxFlowListView view = new Needs.Ccs.Services.Views.UnUploadTaxFlowListView();
            view.AllowPaging = true;
            view.PageIndex = page;
            view.PageSize = rows;
            view.Predicate = predicate;

            int recordCount = view.RecordCount;
            var decs = view.ToList();

            Func<Needs.Ccs.Services.Views.UnUploadTaxFlowListViewModel, object> convert = dec => new
            {
                ID = dec.DecHeadID,
                ContrNo = dec.ContrNo,
                OrderID = dec.OrderID,
                EntryID = dec.EntryID,
                Currency = dec.Currency,
                SwapAmount = dec.DecAmount,
                DDate = dec.DDate?.ToString("yyyy-MM-dd"),
                Status = Needs.Ccs.Services.MultiEnumUtils.ToText<Needs.Ccs.Services.Enums.CusDecStatus>(dec.CusDecStatus),
                IsHandledTariff = (dec.HandledType & HandledType.Tariff.GetHashCode()) == HandledType.Tariff.GetHashCode(),  //是否处理了关税
                IsHandledExciseTax = (dec.HandledType & HandledType.ExciseTax.GetHashCode()) == HandledType.ExciseTax.GetHashCode(),  //是否处理了消费税
                IsHandledAddedValueTax = (dec.HandledType & HandledType.AddedValueTax.GetHashCode()) == HandledType.AddedValueTax.GetHashCode(),  //是否处理了增值税
            };

            Response.Write(new
            {
                rows = decs.Select(convert).ToArray(),
                total = recordCount,
            }.Json());










            //string ContrNo = Request.QueryString["ContrNo"];
            //string EntryID = Request.QueryString["EntryID"];
            //string OrderID = Request.QueryString["OrderID"];
            //string StartDate = Request.QueryString["StartDate"];
            //string EndDate = Request.QueryString["EndDate"];

            //var DecTax = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DecTax.OrderByDescending(t => t.DDate).Where(t => t.UploadStatus == UploadStatus.NotUpload); ;
            //if (!string.IsNullOrEmpty(ContrNo))
            //{
            //    ContrNo = ContrNo.Trim();
            //    DecTax = DecTax.Where(t => t.ContrNo.Contains(ContrNo));
            //}
            //if (!string.IsNullOrEmpty(EntryID))
            //{
            //    EntryID = EntryID.Trim();
            //    DecTax = DecTax.Where(t => t.EntryId.Contains(EntryID));
            //}

            //if (!string.IsNullOrEmpty(OrderID))
            //{
            //    OrderID = OrderID.Trim();
            //    DecTax = DecTax.Where(t => t.OrderID.Contains(OrderID));
            //}
            //if (!string.IsNullOrEmpty(StartDate))
            //{
            //    DateTime start = Convert.ToDateTime(StartDate);
            //    DecTax = DecTax.Where(t => t.DDate >= start);
            //}
            //if (!string.IsNullOrEmpty(EndDate))
            //{
            //    DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
            //    DecTax = DecTax.Where(t => t.DDate < end);
            //}

            //var DecTaxs = DecTax.ToList().Where(t => t.IsSuccess).ToList();
            ////DecTax = DecTax.Where(t => t.IsDeclareSuccess).ToList();

            //Func<Needs.Ccs.Services.Models.DecTax, object> convert = head => new
            //{
            //    ID = head.ID,
            //    ContrNo = head.ContrNo,
            //    OrderID = head.OrderID,
            //    EntryID = head.EntryId,
            //    Currency = head.Currency,
            //    SwapAmount = head.DecAmount,
            //    DDate = head.DDate?.ToString("yyyy-MM-dd"),
            //    //SwapStatus = head.SwapStatus.GetDescription(),
            //    Status = head.StatusName,
            //};
            //this.Paging(DecTaxs, convert);
        }

        /// <summary>
        /// 上传缴税流水
        /// </summary>
        protected void UploadExcel()
        {
            try
            {
                HttpPostedFile file = Request.Files["uploadExcel"];
                string ext = Path.GetExtension(file.FileName);
                if (ext != ".xls" && ext != ".xlsx")
                {
                    Response.Write((new { success = false, message = "文件格式错误，请上传.xls或.xlsx文件！" }).Json());
                    return;
                }
                //查询未上传
                //var DecTaxes = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DecTax.AsQueryable().Where(t => t.UploadStatus == UploadStatus.NotUpload);

                //避免枚举中增加了值而引发错误，所以不遍历枚举，直接用具体值或了
                int allHandledTypeEnumsValue = Needs.Ccs.Services.Enums.HandledType.Tariff.GetHashCode()
                                             | Needs.Ccs.Services.Enums.HandledType.AddedValueTax.GetHashCode()
                                             | Needs.Ccs.Services.Enums.HandledType.ExciseTax.GetHashCode();
                var DecTaxes = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DecTax.AsQueryable().Where(t => t.HandledType != allHandledTypeEnumsValue);
                StringBuilder str = new StringBuilder();
                //文件保存
                string fileName = file.FileName.ReName();
                //创建文件目录
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Import);
                fileDic.CreateDataDirectory();
                file.SaveAs(fileDic.FilePath);
                DataTable dt = Ccs.Utils.NPOIHelper.ExcelToDataTable(fileDic.FilePath, false);
                //dt = LinqSortDataTable(dt);
                var updateData = new Dictionary<string, List<DecTaxFlow>>();
                for (int i = 1; i < dt.Rows.Count; i++)
                {
                    string EntryId = dt.Rows[i][1].ToString();
                    var DecTax = DecTaxes.Where(t => t.EntryId == EntryId).FirstOrDefault();
                    if (DecTax != null)
                    {
                        //如果已上传的缴费流水中已经有对应的 税费编号了，就 continue ， 认为是重复传的
                        if (DecTax.flows != null && DecTax.flows.Any())
                        {
                            string[] existedTaxNumbers = DecTax.flows.Select(t => t.TaxNumber).ToArray();
                            if (existedTaxNumbers.Contains(dt.Rows[i][3].ToString()))
                            {
                                continue;
                            }
                        }


                        DecTaxFlow flow = new DecTaxFlow();
                        flow.DecheadID = DecTax.ID;
                        flow.TaxNumber = dt.Rows[i][3].ToString();
                        flow.OrderID = DecTax.OrderID;
                        flow.Amount = decimal.Parse(dt.Rows[i][9].ToString());
                        flow.EntryID = EntryId;
                        //关税类型
                        if (flow.TaxNumber.Contains("A"))
                        {
                            flow.TaxType = DecTaxType.Tariff;
                        }
                        else if (flow.TaxNumber.Contains("Y"))
                        {
                            flow.TaxType = DecTaxType.ExciseTax;
                        }
                        else if (flow.TaxNumber.Contains("L"))
                        {
                            flow.TaxType = DecTaxType.AddedValueTax;
                        }
                        else
                        {
                            continue;
                        }
                        flow.Status = DecTaxStatus.Unpaid;
                        if (!updateData.ContainsKey(EntryId))
                        {
                            updateData.Add(EntryId, new List<DecTaxFlow>());
                        }

                        flow.DecTaxHandledType = DecTax.HandledType;

                        updateData[EntryId].Add(flow);
                    }
                }

                foreach (var item in updateData)
                {
                    var tarifData = item.Value.FirstOrDefault(f => f.TaxType == DecTaxType.Tariff);
                    if (tarifData != null)
                    {
                        tarifData.Enter();
                    }
                    var exciseData = item.Value.FirstOrDefault(f => f.TaxType == DecTaxType.ExciseTax);
                    if (exciseData != null)
                    {
                        exciseData.Enter();
                    }
                    var addedData = item.Value.FirstOrDefault(f => f.TaxType == DecTaxType.AddedValueTax);
                    if (addedData != null)
                    {
                        addedData.Enter();
                    }
                }


                if (str.ToString().Length > 0)
                {
                    Response.Write((new { success = false, message = "上传失败的报关单: " + str.ToString() }).Json());
                }
                else
                {
                    Response.Write((new { success = true, message = "上传成功" }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "上传失败" + ex.Message }).Json());
            }
        }

        //public DataTable LinqSortDataTable(DataTable tmpDt)
        //{
        //    DataTable dtsort = tmpDt.Clone();
        //    dtsort = tmpDt.Rows.Cast<DataRow>().OrderBy(r => r[1]).OrderBy(r => r[3].ToString().Contains("L")).CopyToDataTable();
        //    return dtsort;
        //}

        /// <summary>
        /// 设置为已上传缴费流水（清零 OrderItemTaxes 的实际收款税率）
        /// </summary>
        protected void SetNoDecTaxFlow()
        {
            try
            {
                string DecHeadID = Request.Form["DecHeadID"];
                string OrderID = Request.Form["OrderID"];
                int targetHandledType = int.Parse(Request.Form["HandledType"]);

                DecTax decTax = new DecTax()
                {
                    ID = DecHeadID,
                    OrderID = OrderID,
                };

                decTax.SetNoDecTaxFlow(targetHandledType);

                Response.Write((new { success = true, message = "处理成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "处理失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 批量 设置为已上传缴费流水（清零 OrderItemTaxes 的实际收款税率）
        /// </summary>
        protected void SetNoTaxBatch()
        {
            try
            {
                var Items = Request.Form["Param"].Replace("&quot;", "\'").JsonTo<List<NoTaxBatchParam>>();

                foreach (var item in Items)
                {
                    DecTax decTax = new DecTax()
                    {
                        ID = item.ID,
                        OrderID = item.OrderID,
                    };

                    decTax.SetNoDecTaxFlow(item.HandledType);
                }
                Response.Write((new { success = true, message = "处理成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "处理失败：" + ex.Message }).Json());
            }
        }

        public class NoTaxBatchParam
        { 
            public string ID { get; set; }
            public string ContrNo { get; set; }
            public string EntryID { get; set; }
            public string OrderID { get; set; }
            public int HandledType { get; set; }
        }
    }
}