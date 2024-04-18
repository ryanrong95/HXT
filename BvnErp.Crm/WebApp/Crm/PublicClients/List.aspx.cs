using Needs.Erp.Generic;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using NtErp.Crm.Services.Models.Generic;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.PublicClients
{
    /// <summary>
    /// 公海客户展示页面
    /// </summary>
    public partial class List : Uc.PageBase
    {
        #region 页面加载
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                load();
            }
        }

        /// <summary>
        /// 页面数据初始化
        /// </summary>
        private void load()
        {
            var admin = new NtErp.Crm.Services.Views.AdminTopView()[Needs.Erp.ErpPlot.Current.ID];
            this.Model.AdminType = (int)admin.JobType;
            this.Model.ClientData = Needs.Erp.ErpPlot.Current.ClientSolutions.Clients.Select(item => new { item.ID, item.Name }).Json();
        }


        /// <summary>
        /// 查询数据
        /// </summary>
        protected void data()
        {
            string ClientID = Request.QueryString["ClientID"];
            var data = Needs.Erp.ErpPlot.Current.ClientSolutions.PublicClients;
            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Func<ClientDossier, object> linq = item => new
            {
                item.Client.ID,
                item.Client.Name,
                item.Client.Status,
                IsSafeName = item.Client.IsSafe.GetDescription(),
                StatusName = item.Client.Status.GetDescription(),
                item.Client.CreateDate,
                item.Client.UpdateDate,
                item.Client.Summary,
            };
            if (!string.IsNullOrWhiteSpace(ClientID))
            {
                Expression<Func<Client, bool>> lambda = item => item.ID.Contains(ClientID);
                lambdas.Add(lambda);
            }
            
            #region 拼凑页面需要数据
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            var myclient = data.GetPageList(page, rows, item => true, lambdas.ToArray());

            Response.Write(new
            {
                rows = myclient.Select(linq).ToArray(),
                total = myclient.Total,
            }.Json());
            #endregion
        }
        #endregion


        #region 页面操作
        /// <summary>
        /// 保护客户
        /// </summary>
        protected void Protect()
        {
            string id = Request.Form["ID"];
            var protect = Needs.Erp.ErpPlot.Current.ClientSolutions.Clients[id] ?? new Client();
            if (protect != null)
            {
                protect.EnterSuccess += Protect_EnterSuccess;
                protect.IsSafe = IsProtected.Yes;
                //绑定销售
                var saleAdmin = Needs.Underly.FkoFactory<AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID);
                Needs.Erp.ErpPlot.Current.ClientSolutions.Clients.Binding(protect.ID, saleAdmin, null, null);

                protect.SingleUpdateEnter();
            }
        }

        /// <summary>
        /// 保护客户成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Protect_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Alert("申请保护成功!");
        }

        /// <summary>
        /// 分配销售
        /// </summary>
        protected void Distribute()
        {
            string id = Request.Form["ID"];
            string salemanID = Request.Form["Sale"];
            var protect = Needs.Erp.ErpPlot.Current.ClientSolutions.PublicClients.GetTop(1, item => item.Client.ID == id).SingleOrDefault().Client ??
               new NtErp.Crm.Services.Models.Client();
            if (protect != null)
            {
                protect.EnterSuccess += Protect_EnterSuccess;
                protect.IsSafe = IsProtected.Yes;
                //绑定销售
                var saleAdmin = Needs.Underly.FkoFactory<AdminTop>.Create(salemanID);
                Needs.Erp.ErpPlot.Current.ClientSolutions.Clients.Binding(protect.ID, saleAdmin, null, null);

                protect.SingleUpdateEnter();
            }
        }

        /// <summary>
        /// 获取分配的销售
        /// </summary>
        protected void GetAdmin()
        {
            var admins = Needs.Erp.ErpPlot.Current.ClientSolutions.MyStaffs.Where(item => item.JobType == JobType.Sales || item.JobType == JobType.Sales_PME);
            Response.Write(admins.Json());
        }
        #endregion



        #region Excel导入
        /// <summary>
        /// 获取导入文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void InputFileUploadButton_Click(object sender, EventArgs e)
        {
            HttpFileCollection files = Request.Files;
            string ext = System.IO.Path.GetExtension(files[0].FileName);

            if (ext != ".xls" && ext != ".xlsx")
            {
                this.Alert("只能上传Excel格式文件");
            }
            else
            {
                var fileName = files[0].FileName.Replace(ext, DateTime.Now.ToString("-yyyyMMddHHmmssfff") + ext);
                string filePath = Server.MapPath("~/UploadFiles/");
                if (files.Count != 0)
                {
                    string path = Path.Combine(filePath, fileName);
                    files[0].SaveAs(path);
                    MyClients.List.UploadEntity entity = FileUpload(path);

                    if (string.IsNullOrWhiteSpace(entity.Message))
                    {
                        string message = entity.SuccessCount + "行保存成功," + entity.FailCount + "行保存失败";
                        if (!string.IsNullOrEmpty(entity.RepeatName))
                        {
                            message += "," + entity.RepeatName + "有重名;";
                        }
                        this.Alert(message);
                    }
                    else
                    {
                        this.Alert(entity.Message);
                    }
                }
            }

            load();
        }

        /// <summary>
        /// Excel数据转为DataTable
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        protected MyClients.List.UploadEntity FileUpload(string filepath)
        {
            DataTable dataTable = new DataTable(); FileStream fs = null; DataColumn column = null; IWorkbook workbook = null;
            ISheet sheet = null; IRow row = null; ICell cell = null; int startRow = 1;
            MyClients.List.UploadEntity entity = new MyClients.List.UploadEntity();
            try
            {
                #region 读取Excel
                using (fs = System.IO.File.OpenRead(filepath))
                {
                    if (filepath.IndexOf(".xlsx") > 0)
                        workbook = new XSSFWorkbook(fs);
                    else if (filepath.IndexOf(".xls") > 0)
                        workbook = new HSSFWorkbook(fs);

                    if (workbook != null)
                    {
                        sheet = workbook.GetSheetAt(0);
                        dataTable = new DataTable();
                        if (sheet != null)
                        {
                            int rowCount = sheet.LastRowNum;

                            if (rowCount > 0)
                            {
                                IRow firstRow = sheet.GetRow(0);//第一行
                                int cellCount = firstRow.LastCellNum;//列数

                                //构建datatable的列                   
                                for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                {
                                    column = new DataColumn("column" + (i + 1));
                                    dataTable.Columns.Add(column);
                                }


                                //填充行
                                for (int i = startRow; i <= rowCount; ++i)
                                {
                                    row = sheet.GetRow(i);
                                    if (row == null) continue;

                                    DataRow dataRow = dataTable.NewRow();
                                    for (int j = row.FirstCellNum; j < cellCount; ++j)
                                    {
                                        cell = row.GetCell(j);
                                        if (cell == null)
                                        {
                                            dataRow[j] = "";
                                        }
                                        else
                                        {
                                            switch (cell.CellType)
                                            {
                                                case CellType.Blank:
                                                    dataRow[j] = "";
                                                    break;
                                                case CellType.Numeric:
                                                    short format = cell.CellStyle.DataFormat;
                                                    //对时间格式（2015.12.5、2015/12/5、2015-12-5等）的处理
                                                    if (format == 14 || format == 31 || format == 57 || format == 58)
                                                        dataRow[j] = cell.DateCellValue;
                                                    else
                                                        dataRow[j] = cell.NumericCellValue;
                                                    break;
                                                case CellType.String:
                                                    dataRow[j] = cell.StringCellValue;
                                                    break;
                                                case CellType.Formula:
                                                    dataRow[j] = cell.NumericCellValue;
                                                    break;
                                            }
                                        }
                                    }
                                    dataTable.Rows.Add(dataRow);
                                }
                            }
                        }


                    }
                }
                #endregion

                entity = CreateClients(dataTable);
            }
            catch (Exception ex)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                entity.Message = ex.Message;
            }

            return entity;
        }


        #region 客户保存
        /// <summary>
        /// excel数据生成客户数据
        /// </summary>
        /// <param name="dt">excel数据</param>
        /// <returns></returns>
        private MyClients.List.UploadEntity CreateClients(DataTable dt)
        {
            string failnames = string.Empty; int failcount = 0;
            var Clients = Needs.Erp.ErpPlot.Current.ClientSolutions.Clients.Where(item => item.Status != ActionStatus.Delete &&
            item.Status != ActionStatus.Reject).Select(item => item.Name);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string name = dt.Rows[i]["column1"].ToString();

                //客户名重复校验
                if (Clients.Contains(name))
                {
                    failnames += name + ",";
                    failcount++;
                    continue;
                }

                if (!string.IsNullOrEmpty(name))
                {
                    var client = new Client();
                    client.Name = name;
                    client.IsSafe = IsProtected.No;
                    client.Status = ActionStatus.Complete;
                    client.Summary = string.Empty;
                    client.AdminCode = string.Empty;
                    client.IndustryInvolved = string.Empty;
                    client.CUSCC = string.Empty;
                    client["Currency"] = CurrencyType.CNY;
                    client["EnterpriseProperty"] = CustomerNature.Pending;
                    client["CustomerType"] = CustomerType.Pending;
                    client["BusinessType"] = BusinessType.Pending;
                    client["CustomerStatus"] = CustomerStatus.Pending;
                    client["Area"] = CustomerArea.Pending;
                    if (!string.IsNullOrEmpty(dt.Rows[i]["column2"].ToString()))
                    {
                        var industry = dt.Rows[i]["column2"].ToString().Replace("&", "&amp;");
                        client["ReIndustry"] = MyClients.List.GetIndustries(industry);
                    }
                    if (!string.IsNullOrEmpty(dt.Rows[i]["column3"].ToString()))
                    {
                        var manufactures = dt.Rows[i]["column3"].ToString().Replace("&", "&amp;");
                        client["AgentBrand"] = MyClients.List.GetManufacture(manufactures);
                    }
                    client.EnterSuccess += Client_EnterSuccess;
                    client.Enter();
                }
                else
                {
                    failcount++;
                }
            }

            return new MyClients.List.UploadEntity { SuccessCount = dt.Rows.Count - failcount, FailCount = failcount, RepeatName = failnames };
        }

        /// <summary>
        /// 客户保存成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            ClientBinding(sender as Client);
        }

        /// <summary>
        /// 客户绑定销售、行业和品牌
        /// </summary>
        /// <param name="client"></param>
        private void ClientBinding(Client client)
        {
            string[] Industries = client["ReIndustry"]?.Split(',') ?? new string[0];
            string[] Manufactures = client["AgentBrand"]?.Split(',') ?? new string[0];
            //品牌绑定
            foreach (var item in Manufactures)
            {
                var manu = Needs.Underly.FkoFactory<Company>.Create(item);
                Needs.Erp.ErpPlot.Current.ClientSolutions.Clients.Binding(client.ID, null, null, manu);
            }
            //行业绑定
            foreach (var item in Industries)
            {
                var industry = Needs.Underly.FkoFactory<Industry>.Create(item);
                Needs.Erp.ErpPlot.Current.ClientSolutions.Clients.Binding(client.ID, null, industry, null);
            }
        }
        #endregion

        #endregion
    }
}