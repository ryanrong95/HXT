using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.PsWms.SzMvc.Services.Enums;
using Yahv.PsWms.SzMvc.Services.Models;
using Yahv.PsWms.SzMvc.Services.Models.Origin;
using Yahv.PsWms.SzMvc.Services.Views.Roll;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PsWms.SzApp.Bills.Payments
{
    public partial class Details : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns></returns>
        protected void LoadData()
        {
            string ID = Request.QueryString["ID"];
            var Index = Request.QueryString["Index"];
            var voucher = new SzMvc.Services.Views.Origins.VouchersOrigin().SingleOrDefault(t => t.PayerID == ID &&
                t.CutDateIndex == int.Parse(Index) &&
                t.Mode == VoucherMode.Payables &&
                t.Type == SzMvc.Services.Enums.VoucherType.Monthly);
            if (voucher != null)
            {
                var file = new SzMvc.Services.Views.Origins.PcFilesOrigin().Where(t => t.MainID == voucher.ID)
                .OrderByDescending(t => t.CreateDate).ToArray();

                this.Model.voucherData = new
                {
                    fileName = file.Count() == 0 ? "" : file[0].CustomName,
                    fileUrl = file.Count() == 0 ? "" : file[0].HttpUrl,
                };
            }
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            var ID = Request.QueryString["ID"];
            var Index = Request.QueryString["Index"];

            var orders = new SzMvc.Services.Views.Origins.OrdersOrigin().ToArray();
            var payerLefts = new SzMvc.Services.Views.Origins.PayerLeftsTopView().Where(t => t.PayerID == ID && t.CutDateIndex == int.Parse(Index)).ToArray();

            var linq = from entity in payerLefts
                       join order in orders on entity.FormID equals order.ID
                       select new
                       {
                           CreateDate = entity.CreateDate.ToString("yyyy-MM-dd"),
                           OrderID = order.ID,
                           Status = order.Status.GetDescription(),
                           OrderDate = order.CreateDate.ToString("yyyy-MM-dd"),
                           Conduct = entity.Conduct.GetDescription(),
                           entity.Subject,
                           UnitPrice = entity.UnitPrice.ToString("f2"),
                           entity.Quantity,
                           Total = entity.Total.ToString("f2"),
                       };

            return this.Paging(linq);
        }

        /// <summary>
        /// 账单导出
        /// </summary>
        protected void btnExport_Click(object sender, EventArgs e)
        {
            var ID = Request.QueryString["ID"];
            var Index = Request.QueryString["Index"];

            var voucher = new SzMvc.Services.Views.Origins.VouchersOrigin().SingleOrDefault(t => t.PayerID == ID &&
                t.CutDateIndex == int.Parse(Index) &&
                t.Mode == VoucherMode.Payables &&
                t.Type == SzMvc.Services.Enums.VoucherType.Monthly);
            if (voucher == null)
            {
                voucher = new Voucher();
                voucher.PayeeID = "";
                voucher.PayerID = ID;
                voucher.Type = SzMvc.Services.Enums.VoucherType.Monthly;
                voucher.Mode = VoucherMode.Payables;
                voucher.CutDateIndex = int.Parse(Index);
                voucher.Enter();
            }
            var filePath = voucher.ToPdf();
            //下载文件
            DownLoadFile(filePath);
        }

        /// <summary>
        /// 上传账单
        /// </summary>
        protected void UploadFile()
        {
            var ID = Request.Form["ID"];
            var Index = Request.Form["Index"];
            try
            {
                List<PcFile> fileList = new List<PcFile>();
                IList<HttpPostedFile> files = System.Web.HttpContext.Current.Request.Files.GetMultiple("uploadFile");
                if (files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        //处理附件
                        HttpPostedFile file = files[i];
                        if (file.ContentLength != 0)
                        {
                            NewFile newFile = new NewFile(file.FileName, PsOrderFileType.Bill);
                            file.SaveAs(newFile.FullName);

                            var voucher = new SzMvc.Services.Views.Origins.VouchersOrigin().SingleOrDefault(t => t.PayerID == ID &&
                                t.CutDateIndex == int.Parse(Index) &&
                                t.Mode == VoucherMode.Payables &&
                                t.Type == SzMvc.Services.Enums.VoucherType.Monthly);
                            if (voucher == null)
                            {
                                throw new Exception("未生成账单，请导出账单后再上传！");
                            }
                            var pcFile = new PcFile();
                            pcFile.MainID = voucher.ID;
                            pcFile.AdminID = Erp.Current.ID;
                            pcFile.CustomName = file.FileName;
                            pcFile.Url = newFile.URL;
                            pcFile.CreateDate = DateTime.Now;
                            pcFile.Type = PsOrderFileType.Bill;
                            pcFile.Enter();

                            fileList.Add(pcFile);
                        }
                    }
                }
                Response.Write((new { success = true, message = "上传成功", Url = fileList[0].HttpUrl, Name = fileList[0].CustomName }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "上传失败：" + ex.Message }).Json());
            }
        }
    }
}