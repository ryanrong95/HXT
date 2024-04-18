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

namespace WebApp.Crm.MyClients
{
    /// <summary>
    /// 客户展示页面
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
            var admintopview = new NtErp.Crm.Services.Views.AdminTopView();
            this.Model.CurrentAdmin = admintopview[Needs.Erp.ErpPlot.Current.ID].Json();
            this.Model.ReIndustry = Needs.Erp.ErpPlot.Current.ClientSolutions.MyIndustries.Where(item => item.FatherID == null).
             Select(item => new { item.ID, item.Name }).Json();
            var ImportantLevelData = EnumUtils.ToDictionary<ImportantLevel>();
            ImportantLevelData.Add("", "全部");
            this.Model.ImportantLevelData = ImportantLevelData.Select(item => new { value = item.Key, text = item.Value }).Json();
            this.Model.Manufacture = Needs.Erp.ErpPlot.Current.ClientSolutions.MyManufactures.Select(item => new { item.ID, item.Name }).Json();
            this.Model.DrpCategory = new NtErp.Crm.Services.Views.IndustryTree().tree;
            this.Model.ClientOwner = Needs.Erp.ErpPlot.Current.ClientSolutions.MyClientsBase.GetMyClientAdmin().
                Select(item => new { item.ID, item.RealName }).Distinct().Json();
            this.Model.ClientData = Needs.Erp.ErpPlot.Current.ClientSolutions.Clients.Select(item => new { item.ID, item.Name }).Json();
            this.Model.StatusData = EnumUtils.ToDictionary<ActionStatus>().Where(item => item.Key != ActionStatus.Normal.ToString()).
                Select(item => new { value = item.Key, text = item.Value }).Json();
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        protected void data()
        {
            string adminid = Needs.Erp.ErpPlot.Current.ID;
            string ClientID = Request.QueryString["ClientID"];
            string ClientOwner = Request.QueryString["ClientOwner"];
            string ClientBrand = Request.QueryString["ClientBrand"];
            string ClientIndustry = Request.QueryString["ClientIndustry"];
            string AdminCode = Request.QueryString["AdminCode"];
            string Status = Request.QueryString["Status"];
            string ClientReIndustry = Request.QueryString["ClientReIndustry"];
            string IsWarning = Request.QueryString["IsWarning"];
            string importantlevel = Request.QueryString["ImportantLevel"];
            var data = Needs.Erp.ErpPlot.Current.ClientSolutions.MyClients;
            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Expression<Func<ClientDossier, bool>> expression = item => true;
            string[] clientids = null;

            #region 页面查询条件
            //查询一周内进入公海的客户
            var warningclient = Needs.Erp.ErpPlot.Current.ClientSolutions.WarningClients.Select(item => item.ID).ToArray();
            if (!string.IsNullOrWhiteSpace(IsWarning))
            {
                Expression<Func<Client, bool>> lambda = item => warningclient.Contains(item.ID);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrWhiteSpace(ClientID))
            {
                Expression<Func<Client, bool>> lambda = item => item.ID.Contains(ClientID);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrWhiteSpace(Status))
            {
                Expression<Func<Client, bool>> lambda = item => item.Status == (ActionStatus)int.Parse(Status);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrWhiteSpace(AdminCode))
            {
                Expression<Func<Client, bool>> lambda = item => item.AdminCode.Contains(AdminCode);
                lambdas.Add(lambda);
            }
            if(!string.IsNullOrWhiteSpace(importantlevel))
            {
                var a = @"""ImportantLevel"":""" + importantlevel;
                Expression<Func<Client, bool>> lambda = item => item.NTextString.Contains(@"""ImportantLevel"":""" + importantlevel);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(ClientIndustry))
            {
                var ClientIndustryArray = ClientIndustry.Split(',').ToArray();
                foreach (var industry in ClientIndustryArray)
                {
                    Expression<Func<Client, bool>> lambda = item => item.IndustryInvolved.Contains(industry);
                    lambdas.Add(lambda);
                }
            }
            if (!string.IsNullOrEmpty(ClientBrand))
            {
                var ClientBrandArray = ClientBrand.Split(',').ToArray();
                clientids = data.MapCondition(item => ClientBrandArray.Contains(item.IndustryID), clientids);
            }
            if (!string.IsNullOrEmpty(ClientReIndustry))
            {
                var ClientReIndustryArray = ClientReIndustry.Split(',').ToArray();
                clientids = data.MapCondition(item => ClientReIndustryArray.Contains(item.IndustryID), clientids);
            }
            if (!string.IsNullOrEmpty(ClientOwner))
            {
                clientids = data.MapCondition(item => item.AdminID == ClientOwner, clientids);
            }
            if (clientids != null)
            {
                expression = item => clientids.Contains(item.Client.ID);
            }
            #endregion


            #region 拼凑页面需要数据
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);
            var myclient = data.GetPageList(page, rows, expression, lambdas.ToArray());

            //获取我的员工
            var mystaff = Needs.Erp.ErpPlot.Current.ClientSolutions.MyStaffs.Select(item => item.ID).ToArray();
            //对象转化
            Func<ClientDossier, object> linq = item => new
            {
                item.Client.ID,
                item.Client.Name,
                item.Client.Status,
                IsSafeName = item.Client.IsSafe.GetDescription(),
                StatusName = item.Client.Status.GetDescription(),
                item.Client.CreateDate,
                item.Client.UpdateDate,
                AdminName = string.Join(",", item.Admins.Select(a => a.RealName).ToArray()),
                AdminID = string.Join(",", item.Admins.Select(a => a.ID).ToArray()),
                IsOwn = item.Admins.Count(a => mystaff.Contains(a.ID)) > 0,
                IsWarning = warningclient.Contains(item.Client.ID),
            };

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
        /// 数据删除
        /// </summary>
        protected void Delete()
        {
            string id = Request.Form["ID"];
            var del = Needs.Erp.ErpPlot.Current.ClientSolutions.MyClients.GetTop(1, item => item.Client.ID == id).SingleOrDefault().Client;
            if (del != null)
            {
                del.AbandonSuccess += Del_AbandonSuccess;
                del.Abandon();
            }
        }

        /// <summary>
        /// 删除成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Del_AbandonSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Alert("删除成功!");
        }

        /// <summary>
        /// 申请
        /// </summary>
        protected void Apply()
        {
            string id = Request.Form["ID"];
            var apply = new Apply();
            apply.MainID = id;
            apply.Type = ApplyType.CreatedClient;
            apply.Admin = Needs.Underly.FkoFactory<AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID);
            apply.Summary = "客户申请";
            apply.Status = ApplyStatus.Audting;
            apply.Enter();

            //更新客户状态
            var plan = Needs.Erp.ErpPlot.Current.ClientSolutions.MyClients.GetTop(1, item => item.Client.ID == id).SingleOrDefault().Client ?? new Client();
            plan.Status = ActionStatus.Auditing;
            plan.SingleUpdateEnter();
        }

        /// <summary>
        /// 加入公海
        /// </summary>
        protected void Allote()
        {
            string id = Request.Form["ID"];
            var Client = Needs.Erp.ErpPlot.Current.ClientSolutions.MyClients.GetTop(1, item => item.Client.ID == id).SingleOrDefault().Client ?? new Client();
            Client.IsSafe = IsProtected.No;
            Client.SingleUpdateEnter();
            //删除管理员绑定
            Needs.Erp.ErpPlot.Current.ClientSolutions.Clients.DeleteBindingAdmin(id);
        }

        /// <summary>
        /// 获取销售
        /// </summary>
        protected void GetAdmin()
        {
            var admins = Needs.Erp.ErpPlot.Current.ClientSolutions.MyStaffs.Where(item => item.JobType == JobType.Sales || item.JobType == JobType.Sales_PME).
                Where(a => a.ID != Needs.Erp.ErpPlot.Current.ID);
            Response.Write(admins.Json());
        }

        /// <summary>
        /// 分配客户保护人
        /// </summary>
        protected void Distribute()
        {
            string id = Request.Form["ID"];
            string[] salemanIDs = Request.Form["Sale"].Split(',');
            if(!string.IsNullOrWhiteSpace(Request.Form["BindingID"]))
            {
                string[] bindingids = Request.Form["BindingID"].Split(',');
                var deleteids = bindingids.Except(salemanIDs);
                //解除指定销售绑定
                Needs.Erp.ErpPlot.Current.ClientSolutions.Clients.DeleteBindingSales(id, deleteids.ToArray());
            }

            //绑定选择的销售
            Needs.Erp.ErpPlot.Current.ClientSolutions.Clients.BindingSales(id, salemanIDs);

            //绑定选择的销售
            //var saleAdmin = Needs.Underly.FkoFactory<AdminTop>.Create(salemanID);
            //删除绑定销售  暂时需求
            //Needs.Erp.ErpPlot.Current.ClientSolutions.Clients.DeleteBindingAdmin(id);

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
                    UploadEntity entity = FileUpload(path);

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
        protected UploadEntity FileUpload(string filepath)
        {
            DataTable dataTable = new DataTable(); FileStream fs = null; DataColumn column = null; IWorkbook workbook = null;
            ISheet sheet = null; IRow row = null; ICell cell = null; int startRow = 1;
            UploadEntity entity = new UploadEntity();
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
        private UploadEntity CreateClients(DataTable dt)
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
                    client.IsSafe = IsProtected.Yes;
                    client.Summary = string.Empty;
                    client.AdminCode = string.Empty;
                    client["Currency"] = CurrencyType.CNY;
                    client["EnterpriseProperty"] = CustomerNature.Pending;
                    client["CustomerType"] = CustomerType.Pending;
                    client["BusinessType"] = BusinessType.Pending;
                    client["CustomerStatus"] = CustomerStatus.Pending;
                    client["Area"] = CustomerArea.Pending;
                    client.IndustryInvolved = string.Empty;
                    client.CUSCC = string.Empty;
                    if (!string.IsNullOrEmpty(dt.Rows[i]["column2"].ToString()))
                    {
                        var industry = dt.Rows[i]["column2"].ToString().Replace("&", "&amp;");
                        client["ReIndustry"] = GetIndustries(industry);
                    }
                    if (!string.IsNullOrEmpty(dt.Rows[i]["column3"].ToString()))
                    {
                        var manufactures = dt.Rows[i]["column3"].ToString().Replace("&", "&amp;");
                        client["AgentBrand"] = GetManufacture(manufactures);
                    }
                    client.EnterSuccess += Client_EnterSuccess;
                    client.Enter();
                }
                else
                {
                    failcount++;
                }
            }

            return new UploadEntity { SuccessCount = dt.Rows.Count - failcount, FailCount = failcount, RepeatName = failnames };
        }

        /// <summary>
        /// 客户保存成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            ClientBinding(sender as Client);
            Apply(e.Object);
        }

        /// <summary>
        /// 客户绑定销售、行业和品牌
        /// </summary>
        /// <param name="client"></param>
        private void ClientBinding(Client client)
        {
            //绑定销售
            AdminTop Admin = Needs.Underly.FkoFactory<AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID);
            Needs.Erp.ErpPlot.Current.ClientSolutions.Clients.Binding(client.ID, Admin, null, null);

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

        /// <summary>
        /// 导入后自动申请
        /// </summary>
        protected void Apply(string id)
        {
            var apply = new Apply();
            apply.MainID = id;
            apply.Type = ApplyType.CreatedClient;
            apply.Admin = Needs.Underly.FkoFactory<AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID);
            apply.Summary = "客户申请";
            apply.Status = ApplyStatus.Audting;
            apply.Enter();

        }
        #endregion


        /// <summary>
        /// 所属行业校验
        /// </summary>
        /// <param name="industry">excel中行业</param>
        /// <returns></returns>
        public static string GetIndustries(string industry)
        {
            string[] array = industry.Split('，');
            var ReIndustry = Needs.Erp.ErpPlot.Current.ClientSolutions.MyIndustries.Where(item => item.FatherID == null);

            var industries = ReIndustry.Where(item => array.Contains(item.Name)).Select(item => item.ID).ToArray();

            var data = industries.Count() == 0 ? null : string.Join(",", industries);

            return data;
        }

        /// <summary>
        /// 品牌校验
        /// </summary>
        /// <param name="industry">excel中行业</param>
        /// <returns></returns>
        public static string GetManufacture(string Manufactrue)
        {
            string[] array = Manufactrue.Split('，');
            var Manufactrues = Needs.Erp.ErpPlot.Current.ClientSolutions.Companys.Where(item => item.Type == CompanyType.Manufacture);

            var Manus = Manufactrues.Where(item => array.Contains(item.Name)).Select(item => item.ID).ToArray();

            var data = Manus.Count() == 0 ? null : string.Join(",", Manus);

            return data;
        }


        /// <summary>
        /// 导入完成信息对象
        /// </summary>
        public class UploadEntity
        {
            //成功数量
            public int SuccessCount { get; set; }

            //错误数量
            public int FailCount { get; set; }

            public string RepeatName { get; set; }

            //报错信息
            public string Message { get; set; }
        }
        #endregion
    }
}