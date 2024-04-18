using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Web.Mvc;
using Yahv.Linq;
using Yahv.PsWms.SzMvc.Services.Views;
using Yahv.Web.Mvc;

namespace Yahv.PsWms.SzApi.Controllers
{
    public class VoucherController : Controller
    {
        /// <summary>
        /// 深圳芯达通供应链有限公司ID
        /// </summary>
        private string XdtEnterpriseID = "DBAEAB43B47EB4299DD1D62F764E6B6A";

        public ActionResult Index()
        {
            return Json(new { msg = "dfs" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Init()
        {
            try
            {
                var clients = new SzMvc.Services.Views.Origins.ClientsOrigin();
                var vouchers = new SzMvc.Services.Views.Origins.VouchersOrigin();

                foreach (var client in clients)
                {
                    var year = DateTime.Now.Year.ToString();
                    var month = DateTime.Now.Month;
                    for (int i = 1; i <= month; i++)
                    {
                        var CutDateIndex = year + i.ToString().PadLeft(2, '0');
                        var voucher = vouchers
                            .SingleOrDefault(t => t.PayerID == client.ID &&
                            t.Mode == SzMvc.Services.Enums.VoucherMode.Receivables &&
                            t.Type == SzMvc.Services.Enums.VoucherType.Monthly &&
                            t.CutDateIndex == int.Parse(CutDateIndex));
                        if (voucher == null)
                        {
                            voucher = new SzMvc.Services.Models.Origin.Voucher();
                            voucher.PayeeID = this.XdtEnterpriseID;
                            voucher.PayerID = client.ID;
                            voucher.Type = SzMvc.Services.Enums.VoucherType.Monthly;
                            voucher.Mode = SzMvc.Services.Enums.VoucherMode.Receivables;
                            voucher.CutDateIndex = int.Parse(CutDateIndex);
                            voucher.Enter();
                        }
                    }
                }

                Response.StatusCode = 200;
                return Json(new { success = true, msg = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { success = false, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取多个账单信息
        /// { "VoucherIDs": "Vourcher20210301000002, Vourcher20210310000001" }
        /// </summary>
        /// <returns></returns>
        public JsonResult GetVoucherInfo(JPost jpost)
        {
            try
            {
                string VoucherIDs = jpost["VoucherIDs"]?.Value<string>();
                string[] VoucherIDs_Array = VoucherIDs.Replace(" ", "").Split(',');

                var voucherInfos = new VoucherInfoView(VoucherIDs_Array).GetVoucherInfos();

                var theVoucherInfos = voucherInfos.Select(item => new
                {
                    VoucherID = item.VoucherID,
                    CutDateIndex = item.CutDateIndex,
                });

                Response.StatusCode = 200;
                return Json(new { success = true, msg = "", voucherinfos = theVoucherInfos }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 200;
                return Json(new { success = false, msg = "发生错误：" + ex.Message, }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}