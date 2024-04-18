using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Invoice
{
    public partial class Confirm : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// </summary>
        protected void LoadData()
        {
            string ID = Request.QueryString["ID"];
            var notice = new Needs.Ccs.Services.Views.InvoiceNoticeViewRJ(ID).GetInvoice();
            //开票信息
            this.Model.InvoiceData = new
            {
                InvoiceType = notice.InvoiceType.GetDescription(),
                DeliveryType = notice.DeliveryType.GetDescription(),
                CompanyName = notice.Client.Company.Name,
                TaxCode = notice.ClientInvoice.TaxCode,
                BankInfo = notice.BankName + " " + notice.BankAccount,
                AddressTel = notice.Address + " " + notice.Tel
            }.Json();
            //邮寄信息
            this.Model.MaileDate = new
            {
                ReceipCompany = notice.Client.Company.Name,
                //ReceiterAndTel = notice.MailName + " " + notice.MailMobile,
                ReceiterName = notice.MailName,
                ReceiterTel = notice.MailMobile,
                DetailAddres = notice.MailAddress,
                WaybillCode = notice.WaybillCode,
            }.Json();

            //其它信息
            this.Model.OtherData = new
            {
                Amount = notice.Amount.ToRound(2),
                Difference = notice.Difference,
                Summary = notice.Summary,
            }.Json();
        }

        /// <summary>
        /// 加载产品信息
        /// </summary>
        protected void ProductData()
        {
            string id = Request["ID"];
            //1.根据ID获取通知项内容
            //var notice = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.InvoiceNotice[id];
            var noticeitem = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.InvoiceNoticeItem.Where(x => x.InvoiceNoticeID == id);


            List<InvoiceNoticeItem> listInvoiceNoticeItem = noticeitem.ToList();
            decimal totalQuantity = 0;
            foreach (var item in listInvoiceNoticeItem)
            {
                totalQuantity += item.OrderItem == null ? 1 : item.OrderItem.Quantity;
            }

            var totaldata = new
            {
                Amount = listInvoiceNoticeItem.Sum(t => t.Amount).ToRound(2), //含税金额
                Difference = listInvoiceNoticeItem.Sum(t => t.Difference), //开票差额
                Quantity = totalQuantity, //数量
                SalesTotalPrice = listInvoiceNoticeItem.Sum(t => t.SalesTotalPrice).ToRound(2), //金额
            };


            //前台显示
            Func<InvoiceNoticeItem, object> convert = item => new
            {
                ID = item.ID,
                item.OrderID,
                ProductName = item.OrderItem == null ? "*物流辅助服务*服务费" : item.OrderItem?.Category.Name,  //产品名称
                //ProductModel = item.OrderItem?.Product.Model,//型号
                ProductModel = item.OrderItem?.Model,
                item.OrderItem?.Unit,//单位
                Quantity = item.OrderItem == null ? 1 : item.OrderItem.Quantity,//数量
                item.SalesUnitPrice, //单价
                SalesTotalPrice = item.SalesTotalPrice.ToRound(2), //金额
                item.UnitPrice, //含税单价
                item.InvoiceTaxRate, //税率
                Amount = item.Amount.ToRound(2),//含税总额
                //为了与开票软件一致，这里先算出不含税金额，再算出含税金额
                //Amount = (((item.Amount + item.Difference) / (1 + item.InvoiceTaxRate)).ToRound(2)* (1 + item.InvoiceTaxRate)).ToRound(2),
                TaxName = item.OrderItem == null ? "*物流辅助服务*服务费" : item.TaxName,//税务名称
                TaxCode = item.OrderItem == null ? "3040407040000000000" : item.TaxCode,
                item.Difference,
                item.InvoiceCode,
                item.InvoiceNo,
                InvoiceDate = item.InvoiceDate?.ToString("yyyy-MM-dd"),
                item.UnitName,

            };

            Response.Write(new { rows = listInvoiceNoticeItem.Select(convert).ToArray(), totaldata = totaldata }.Json());
        }

        /// <summary>
        /// 计算发票号
        /// </summary>
        protected void CalcInvoiceNumber()
        {
            try
            {
                string id = Request.Form["InvoiceNoticeID"];
                var changeProductData = Request.Form["Data"].Replace("&quot;", "'");
                var InvoiceModelList = changeProductData.JsonTo<List<InvoiceSubmitModel>>();

                List<InvoiceContext.InvoiceInfoForCalc> invoiceForCalc = new List<InvoiceContext.InvoiceInfoForCalc>();
                foreach (var invoiceModel in InvoiceModelList)
                {
                    foreach (var invoiceNo in invoiceModel.InvoiceNosReal)
                    {
                        invoiceForCalc.Add(new InvoiceContext.InvoiceInfoForCalc
                        {
                            InvoiceCode = invoiceModel.InvoiceCode,
                            InvoiceNo = invoiceNo,
                            InvoiceDate = invoiceModel.InvoiceDate,
                        });
                    }
                }

                invoiceForCalc = invoiceForCalc.Distinct(new InvoiceContext.DistinctInvoiceInfoComparer()).ToList();

                var invoiceInfos = invoiceForCalc.Select(item => new
                {
                    item.InvoiceCode,
                    item.InvoiceNo,
                    item.InvoiceDate,
                }).ToArray();
                int invoicecount = invoiceForCalc.Count();

                Response.Write((new { success = true, message = "计算发票号成功", invoiceinfos = invoiceInfos, invoicecount = invoicecount, }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "发生错误：" + ex.Message }).Json());
            }
        }

        ///// <summary>
        ///// 确认开票
        ///// </summary>
        protected void ConfirmInvoice()
        {
            try
            {
                string id = Request.Form["InvoiceNoticeID"];
                var changeProductData = Request.Form["Data"].Replace("&quot;", "'");
                var InvoiceModelList = changeProductData.JsonTo<List<InvoiceSubmitModel>>();
                //var count = InvoiceModelList.Where(x => x.InvoiceNo == null || x.InvoiceNo == "").Count();
                //if (count > 0)
                //{
                //    Response.Write((new { success = false, message = "发票号码不允许为空！" }).Json());
                //    return;
                //}
                //var count2 = InvoiceModelList.Where(x => x.InvoiceCode == null || x.InvoiceCode == "").Count();
                //if (count2 > 0)
                //{
                //    Response.Write((new { success = false, message = "发票代码不允许为空！" }).Json());
                //    return;
                //}
                //var count3 = InvoiceModelList.Where(x => x.InvoiceDate == null || x.InvoiceDate == "").Count();
                //if (count3 > 0)
                //{
                //    Response.Write((new { success = false, message = "开票日期不允许为空！" }).Json());
                //    return;
                //}

                var InvoiceContext = new InvoiceContext();
                InvoiceContext.InvoiceModelList = InvoiceModelList;
                InvoiceContext.Admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);//开票人
                InvoiceContext.InvoiceNoticeID = id;
                InvoiceContext.ConfirmInvoice();

                //NoticeLog noticeLog = new NoticeLog();
                //noticeLog.MainID = id;
                //noticeLog.NoticeType = SendNoticeType.InvoicePending;
                //noticeLog.Readed = true;
                //noticeLog.SendNotice();

                Response.Write((new { success = true, message = "开票成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "申请失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 更新发票代码
        /// </summary>
        protected void UpdateInvoiceCode()
        {
            try
            {
                string id = Request.Form["InvoiceNoticeID"];
                var noticeItems = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.InvoiceNoticeItem.Where(x => x.InvoiceNoticeID == id);

                var changeProductData = Request.Form["Data"].Replace("&quot;", "'");
                var InvoiceModelList = changeProductData.JsonTo<List<InvoiceSubmitModel>>();
                foreach (var item in InvoiceModelList)
                {
                    var noticeItem = noticeItems.Where(x => x.ID == item.ID).FirstOrDefault();
                    if (noticeItem != null)
                    {
                        noticeItem.InvoiceCode = item.InvoiceCode;
                        noticeItem.Enter();
                    }
                }

                Response.Write((new { success = true, message = "更新成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "更新失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 更新发票号码
        /// </summary>
        protected void UpdateInvoiceNo()
        {
            try
            {
                string id = Request.Form["InvoiceNoticeID"];
                var noticeItems = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.InvoiceNoticeItem.Where(x => x.InvoiceNoticeID == id);

                var changeProductData = Request.Form["Data"].Replace("&quot;", "'");
                var InvoiceModelList = changeProductData.JsonTo<List<InvoiceSubmitModel>>();
                foreach (var item in InvoiceModelList)
                {
                    var noticeItem = noticeItems.Where(x => x.ID == item.ID).FirstOrDefault();
                    if (noticeItem != null)
                    {
                        noticeItem.InvoiceNo = item.InvoiceNo;
                        noticeItem.Enter();
                    }
                }

                Response.Write((new { success = true, message = "更新成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "更新失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 更新开票日期
        /// </summary>
        protected void UpdateInvoiceDate()
        {
            try
            {
                string id = Request.Form["InvoiceNoticeID"];
                var noticeItems = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.InvoiceNoticeItem.Where(x => x.InvoiceNoticeID == id);

                var changeProductData = Request.Form["Data"].Replace("&quot;", "'");
                var InvoiceModelList = changeProductData.JsonTo<List<InvoiceSubmitModel>>();
                foreach (var item in InvoiceModelList)
                {
                    var noticeItem = noticeItems.Where(x => x.ID == item.ID).FirstOrDefault();
                    if (noticeItem != null)
                    {
                        noticeItem.InvoiceDate = item.InvoiceDateDt;
                        noticeItem.Enter();
                    }
                }

                Response.Write((new { success = true, message = "更新成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "更新失败：" + ex.Message }).Json());
            }
        }

        protected void LoadInvoiceLogs()
        {
            string id = Request.Form["ID"];
            var noticeLog = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.InvoiceNoticeLog.Where(x => x.InvoiceNoticeID == id);
            noticeLog = noticeLog.OrderByDescending(x => x.CreateDate);
            Func<InvoiceNoticeLog, object> convert = item => new
            {
                ID = item.ID,
                Summary = item.Summary,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            };
            Response.Write(new { rows = noticeLog.Select(convert).ToArray(), }.Json());
        }

        /// <summary>
        /// 获取微信发票页面二维码
        /// </summary>
        protected void GetWxInvoicePageQrCode()
        {
            string InvoiceNoticeID = Request.QueryString["InvoiceNoticeID"];
            string AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;

            // 生成二维码的内容
            string strCode = "http://wx.for-ic.net/FinanceManage/InvoiceScan?InvoiceNoticeID=" + InvoiceNoticeID + "&AdminID=" + AdminID;
            QRCodeGenerator qrGenerator = new QRCoder.QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(strCode, QRCodeGenerator.ECCLevel.Q);
            QRCode qrcode = new QRCode(qrCodeData);

            // qrcode.GetGraphic 方法可参考最下发“补充说明”
            Bitmap qrCodeImage = qrcode.GetGraphic(10, Color.Black, Color.White, null, 15, 6, false);
            MemoryStream ms = new MemoryStream();
            qrCodeImage.Save(ms, ImageFormat.Jpeg);

            // 如果想保存图片 可使用  qrCodeImage.Save(filePath);

            // 响应类型
            Response.ContentType = "image/Jpeg";
            //输出字符流
            Response.BinaryWrite(ms.ToArray());
        }

        /// <summary>
        /// 发票信息
        /// </summary>
        protected void InvoiceData()
        {
            try
            {
                string InvoiceNoticeID = Request["ID"];

                using (var query = new Needs.Ccs.Services.Views.TaxManageForNoticeView(InvoiceNoticeID))
                {
                    var view = query;

                    Response.Write(new { success = true, data = view.ToMyPage() }.Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write(new { success = false, err = ex.Message, }.Json());
            }
        }
    }
}