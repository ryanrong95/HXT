using Needs.Ccs.Services;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Utils.Converters;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Declare
{
    public partial class DecCheck : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadFiles();
            }
        }

        protected void Print()
        {
            string DeclarationID = Request.Form["ID"];
            var DecHead = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHead[DeclarationID];
            var vendor = new VendorContext(VendorContextInitParam.Pointed, DecHead.OwnerName).Current1;
            //var InPort = "";//入境口岸
            //var CusPort = "";//关区代码
            //var CusPortName = "";//关区代码
            //var CIQPort = "";//检验检疫机关代码

            //申报地海关 CustomMaster  CustomMasterName
            //进境关别 IEPort   IEPortName
            //入境口岸  EntyPortCode  EntyPortCodeName


            var customMasterView = new Needs.Ccs.Services.Views.BaseCustomMasterView();
            var entryPortView = new Needs.Ccs.Services.Views.BaseEntryPortsView();

            var CustomMaster = DecHead.CustomMaster;
            var CustomMasterName = customMasterView.FirstOrDefault(t => t.Code == DecHead.CustomMaster)?.Name;
            var IEPort = DecHead.IEPort;
            var IEPortName = customMasterView.FirstOrDefault(t => t.Code == DecHead.IEPort)?.Name;
            var EntyPortCode = DecHead.EntyPortCode;
            var EntyPortCodeName = entryPortView.FirstOrDefault(t => t.Code == DecHead.EntyPortCode)?.Name;
            var Containers = DecHead.Containers.ToArray();
            var Container = "";
            if (Containers.Count() > 0)
            {
                var SumContainer = 0;
                var dd = new string[] { "11", "12", "13", "32" };
                foreach (var con in Containers)
                {
                    Container += con.ContainerID + ";";
                    SumContainer += dd.Contains(con.ContainerMd) ? 2 : 1;
                }

                Container = SumContainer + ";" + Container;
            }

            //switch (DecHead.EntyPortCode)
            //{
            //    //文锦渡
            //    case "470401":
            //        InPort = "470401";
            //        CusPort = "5320";
            //        CIQPort = "475400";
            //        CusPortName = "文锦渡";
            //        break;
            //    //皇岗
            //    case "470201":
            //        InPort = "470201";
            //        CusPort = "5301";
            //        CIQPort = "475200";
            //        CusPortName = "皇岗";
            //        break;
            //    //沙头角
            //    case "470501":
            //        InPort = "470501";
            //        CusPort = "5303";
            //        CIQPort = "475500";
            //        CusPortName = "沙头角";
            //        break;
            //    //默认皇岗
            //    default:
            //        InPort = "470201";
            //        CusPort = "5301";
            //        CIQPort = "475200";
            //        CusPortName = "皇岗";
            //        break;
            //}

            var isshowclient = Needs.Wl.Admin.Plat.AdminPlat.Current.ID != "Admin00002";

            string jsonResult = new
            {
                //InPort = InPort,
                //CusPort = CusPort,
                //CIQPort = CIQPort,
                //CusPortName = CusPortName,

                CustomMaster,
                CustomMasterName,
                IEPort,
                IEPortName,
                EntyPortCode,
                EntyPortCodeName,

                ContrNo = DecHead.ContrNo,

                CustomsCode = PurchaserContext.Current.CustomsCode,//消费使用单位代码-10位
                Code = PurchaserContext.Current.Code,//消费单位 统一社会信用代码
                CiqCode = PurchaserContext.Current.CiqCode,//消费单位 检验检疫代码
                CompanyName = PurchaserContext.Current.CompanyName,//消费使用单位-客户名称

                OwnerName = isshowclient ? DecHead.OwnerName : "",//消费使用单位-客户名称
                OwnerCusCode = isshowclient ? DecHead.OwnerCusCode : "",//消费使用单位代码-10位
                OwnerScc = isshowclient ? DecHead.OwnerScc : "",//消费单位 同意社会信用代码               

                VendorCompanyName = vendor.CompanyName,

                VoyNo = DecHead.VoyNo,//航次号
                BillNo = DecHead.BillNo,//提运单号
                PackNo = DecHead.PackNo,//件数
                GrossWt = DecHead.GrossWt,
                NetWt = DecHead.NetWt,

                IsInspection = (DecHead.IsInspection || (DecHead.IsQuarantine == null ? false : DecHead.IsQuarantine.Value)) ? "是" : "否",

                //
                ManualNo = DecHead.ManualNo ?? "",//备案号
                IEDate = DecHead.IEDate,//进口日期
                DDate = DecHead.DDate.Value.ToString("yyyy-MM-dd"),//申报日期
                TrafMode = DecHead.TrafMode,//运输方式
                TrafModeName = new BaseTrafModesView().FirstOrDefault(t => t.Code == DecHead.TrafMode)?.Name,//运输方式名称
                TrafName = DecHead.TrafName ?? "",//运输工具
                TradeMode = DecHead.TradeMode,//监管方式
                TradeModeName = new BaseTradeModesView().FirstOrDefault(t => t.Code == DecHead.TradeMode)?.Name,//监管方式名称
                CutMode = DecHead.CutMode,//征免性质
                CutModeName = new BaseCutModeView().FirstOrDefault(t => t.Code == DecHead.CutMode)?.Name,//征免性质名称
                LicenseNo = DecHead.LicenseNo ?? "",//许可证号
                TradeCountry = DecHead.TradeCountry,//启运国
                TradeCountryName = new BaseCountriesView().FirstOrDefault(t => t.Code == DecHead.TradeCountry)?.Name,//启运国名称
                DistinatePort = DecHead.DistinatePort,//经停港
                DistinatePortName = new BasePortsView().FirstOrDefault(t => t.Code == DecHead.DistinatePort)?.Name,//经停港名称
                TransMode = DecHead.TransMode,//成交方式
                TransModeName = new BaseTransModesView().FirstOrDefault(t => t.Code == DecHead.TransMode)?.Name,//成交方式名称
                DespPortCode = DecHead.DespPortCode,//启运港
                DespPortCodeName = new BasePortsView().FirstOrDefault(t => t.Code == DecHead.DistinatePort)?.Name,//启运港名称
                WrapType = DecHead.WrapType,//包装
                WrapTypeName = new BasePackTypesView().FirstOrDefault(t => t.Code == DecHead.WrapType)?.Name,//包装类型名称
                TradeAreaCode = DecHead.TradeAreaCode,//贸易国别
                TradeAreaCodeName = new BaseCountriesView().FirstOrDefault(t => t.Code == DecHead.TradeAreaCode)?.Name,//贸易国别名称 
                GoodsPlace = DecHead.GoodsPlace,//货物存放地点
                EntryType = DecHead.EntryType,//报关单类型
                MarkNo = DecHead.MarkNo,
                NoteS = DecHead.NoteS,
                Container = Container

            }.Json();

            Response.Write(new { result = true, info = jsonResult }.Json());
        }

        protected void ListPrint()
        {
            string DeclarationID = Request.Form["ID"];
            var DecHead = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHead[DeclarationID];
            CheckDocuments doc = new CheckDocuments(DecHead);
            string jsonResult = doc.Json();

            Response.Write(new { result = true, info = jsonResult }.Json());
        }

        protected void LoadFiles()
        {
            string DeclarationID = Request.QueryString["ID"];
            string VoyageID = Request.QueryString["VoyageID"];
            string OrderID = Request.QueryString["OrderID"];
            var edoc = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.EdocRealations.Where(t => t.DeclarationID == DeclarationID);
            string FileServerUrl = System.Configuration.ConfigurationManager.AppSettings["FileServerUrl"];

            var contract = edoc.Where(t => t.Edoc.Name == "合同").FirstOrDefault();
            var invoice = edoc.Where(t => t.Edoc.Name == "发票").FirstOrDefault();
            var packingList = edoc.Where(t => t.Edoc.Name == "装箱单").FirstOrDefault();

            this.Model.ContractUrl = contract == null ? "" : FileServerUrl + @"/" + contract.FileUrl.ToUrl();
            this.Model.InvoiceUrl = invoice == null ? "" : FileServerUrl + @"/" + invoice.FileUrl.ToUrl();
            this.Model.PackingListUrl = packingList == null ? "" : FileServerUrl + @"/" + packingList.FileUrl.ToUrl();


            //六联单附件
            var voyagefile = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.VoyageFiles.FirstOrDefault(f => f.VoyageID == VoyageID && f.FileType == Needs.Ccs.Services.Enums.FileType.LiuLianDan);
            this.Model.VoyageFileUrl = voyagefile == null ? "" : FileServerUrl + @"/" + voyagefile.Url.ToUrl();

            //3C目录外
            var CCCWai = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.DecFileView.Where(t => t.DecHeadID == DeclarationID && t.FileType == Needs.Ccs.Services.Enums.FileType.AppraiseReuslt).ToList();
            this.Model.CCCWai = CCCWai == null ? "" : CCCWai.Select(t => new
            {
                FileName = t.Name,
                Url = FileServerUrl + @"/" + t.Url.ToUrl()
            }).ToArray().Json();

            //加征排除编码

            //订单号
            this.Model.OrderID = OrderID;

            this.Model.IsReturnd = new Needs.Ccs.Services.Views.DecTracesView().Any(t => t.DeclarationID == DeclarationID && t.Message.Contains("拒绝理由"));
        }

        protected void Submit()
        {
            try
            {
                string DeclarationID = Request.Form["DeclarationID"];
                string summary = Request.Form["Reason"];
                string openTime = Request.Form["StartTime"];
                string Form = Request.Form["Form"];
                var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                var decHead = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHead[DeclarationID];

                string UnknownCountryModels = "";
                foreach (var p in decHead.Lists)
                {

                    if (p.OriginCountry == Needs.Ccs.Services.Icgoo.UnknownCountry)
                    {
                        UnknownCountryModels += decHead.ID + ":" + p.GoodsModel + ";";
                    }
                    if (UnknownCountryModels != "")
                    {
                        return;
                    }
                }

                if (UnknownCountryModels != "")
                {
                    Response.Write((new { success = false, message = UnknownCountryModels.Substring(0, UnknownCountryModels.Length - 1) + "原产地未识别，请先更改！" }).Json());
                    return;
                }

                var traceMsg = admin.ByName + "在 " + openTime + " 打开了页面,开始复核,在 " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 结束复核。";
                if (!string.IsNullOrEmpty(summary))
                {
                    traceMsg += "备注:" + summary;
                }
                decHead.Trace(traceMsg);

                if (Form.ToLower().Equals("maker"))
                {
                    var currentUnDecNoticeCount = new Needs.Ccs.Services.Views.CurrentDoubleCheckedCountView().GetCurrentUnDecNoticeCount();
                    string adminID = GetDeclareCreatorAdminID(currentUnDecNoticeCount);
                    decHead.MakerDoubleCheck(adminID);
                }
                else if (Form.ToLower().Equals("checker"))
                {
                    // 给报关单设置“发单员” Begin

                    if (string.IsNullOrEmpty(decHead.SubmitCustomAdminID))
                    {
                        var currentDraftCount = new Needs.Ccs.Services.Views.CurrentDraftCountView().GetCurrentDraftCount();
                        string submiterAdminID = GetCustomSubmiterAdminID(currentDraftCount);
                        decHead.SetCustomSubmiter(submiterAdminID);
                    }

                    // 给报关单设置“发单员” End

                    decHead.CheckerDoubleCheck();
                    var ManifestBills = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ManifestConsignmentLists.Where(t => t.BillNo == decHead.BillNo).FirstOrDefault();
                    if (ManifestBills == null)
                    {
                        decHead.ToManifest();
                    }
                }

                Response.Write((new { success = true, message = "提交成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new
                {
                    success = false,
                    message = "提交失败：" + ex.Message
                }).Json());
            }
        }

        protected void Refuse()
        {
            try
            {
                string DeclarationID = Request.Form["DeclarationID"];
                string summary = Request.Form["Reason"];
                string openTime = Request.Form["StartTime"];
                var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                var decHead = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHead[DeclarationID];

                var traceMsg = admin.ByName + "在 " + openTime + " 打开了页面,开始复核,在 " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 退回该报关单。拒绝理由：" + summary;

                decHead.Trace(traceMsg);
                decHead.CheckerDoubleRefuse();

                Response.Write((new { success = true, message = "提交成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new
                {
                    success = false,
                    message = "提交失败：" + ex.Message
                }).Json());
            }
        }

        /// <summary>
        /// 获取发单人ID
        /// </summary>
        /// <param name="listModel"></param>
        /// <returns></returns>
        private string GetCustomSubmiterAdminID(List<Needs.Ccs.Services.Views.CurrentDraftCountViewModel> listModel)
        {
            if (listModel == null || !listModel.Any())
            {
                return null;
            }

            var minCount = listModel.OrderBy(t => t.DraftCount).FirstOrDefault().DraftCount;
            int[] serialNos = listModel.Where(t => t.DraftCount == minCount).Select(t => t.SerialNo).ToArray();

            Random rand = new Random();
            int arrNum = rand.Next(0, serialNos.Count() - 1);

            var theSelectedModel = listModel.Where(t => t.SerialNo == serialNos[arrNum]).FirstOrDefault();

            for (int i = 0; i < listModel.Count; i++)
            {
                if (listModel[i].SerialNo == serialNos[arrNum])
                {
                    listModel[i].DraftCount = listModel[i].DraftCount + 1;
                    break;
                }
            }

            return theSelectedModel.AdminID;
        }

        private string GetDeclareCreatorAdminID(List<CurrentUnDecNoticeCountViewModel> listModel)
        {
            if (listModel == null || !listModel.Any())
            {
                return null;
            }

            var minCount = listModel.OrderBy(t => t.UnDecNoticeCount).FirstOrDefault().UnDecNoticeCount;
            int[] serialNos = listModel.Where(t => t.UnDecNoticeCount == minCount).Select(t => t.SerialNo).ToArray();

            Random rand = new Random();
            int arrNum = rand.Next(0, serialNos.Count() - 1);

            var theSelectedModel = listModel.Where(t => t.SerialNo == serialNos[arrNum]).FirstOrDefault();

            for (int i = 0; i < listModel.Count; i++)
            {
                if (listModel[i].SerialNo == serialNos[arrNum])
                {
                    listModel[i].UnDecNoticeCount = listModel[i].UnDecNoticeCount + 1;
                    break;
                }
            }

            return theSelectedModel.AdminID;
        }
    }
}
