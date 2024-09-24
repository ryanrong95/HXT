using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Services;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Serializers;
using Needs.Wl.Models;
using NPOI.Util.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Declare
{
    public partial class DecHead : Uc.PageBase
    {
        Purchaser purchaser = PurchaserContext.Current;
        List<SingleOwnerCompany> innerCompany = SingleOwnerCompanyContext.Current;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                load();
            }
        }

        private void load()
        {           
            string OrderID = Request.QueryString["OrderId"];
            DefaultLoadNEW(OrderID);
          
            string DecHeadID = Request.QueryString["ID"];          
            if (!string.IsNullOrEmpty(DecHeadID))
            {
                Needs.Ccs.Services.Models.DecHead headinfo = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHead[DecHeadID];
                if (headinfo != null)
                {
                    EditLoad(headinfo);
                }
            }
            else
            {

                string ClientID = Request.QueryString["ClientID"];
                
                NewLoad(ClientID, OrderID);
            }

            this.Model.CandidateData = new Needs.Ccs.Services.Views.SelectableCandidatesView().GetUseCandidates(Needs.Ccs.Services.Enums.DeclarantCandidateType.CustomSubmiter)
                    .Select(item => new { value = item.AdminID, text = item.AdminName }).Json();
        }

        private void DefaultLoad(string OrderID)
        {
            string AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
            string AdminName = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName;
            this.Model.CurrentAdmin = new { ID = AdminID, Name = AdminName }.Json();
            this.Model.CustomMaster = Needs.Wl.Admin.Plat.AdminPlat.BaseCustomMaster.Select(item => new { ID = item.Code, Text = item.Code + "-" + item.Name }).OrderBy(item => item.ID).Json();
            List<Needs.Ccs.Services.Models.Company> companies = GetCompanies();
            this.Model.DeclareCompanies = companies.Select(item => new { ID = item.Code, Text = item.Name }).Json();

            this.Model.DeclareCompanySZHY = new
            {
                Name = purchaser.CompanyName,
                Code = purchaser.Code,
                CustomsCode = purchaser.CustomsCode,
                CIQCode = purchaser.CiqCode,
                TypistNo = purchaser.ICCode,
                DeclareName = purchaser.DeclareName
            }.Json();

            var vendor = new VendorContext(VendorContextInitParam.OrderID, OrderID).Current1;

            this.Model.ConsignorCompany = new { Name = vendor.OverseasConsignorCname, Code = vendor.CompanyName }.Json();
            List<Needs.Ccs.Services.Models.Company> Foreigncompanies = GetForeignCompanies(vendor);
            this.Model.ForgienCompany = Foreigncompanies.Select(item => new { ID = item.Name, Text = item.Name }).Json();           
            //运输方式
            this.Model.TrafMode = Needs.Wl.Admin.Plat.AdminPlat.BaseTrafMode.Select(item => new { Value = item.Code, Text = item.Name }).OrderBy(item => item.Value).Json();
            //监管方式
            this.Model.TradeMode = Needs.Wl.Admin.Plat.AdminPlat.BaseTradeMode.Select(item => new { Value = item.Code, Text = item.Name }).OrderBy(item => item.Value).Json();
            //征免性质
            this.Model.CutMode = Needs.Wl.Admin.Plat.AdminPlat.BaseCutMode.Select(item => new { Value = item.Code, Text = item.Name }).OrderBy(item => item.Value).Json();
            //启运国
            this.Model.TradeCountry = Needs.Wl.Admin.Plat.AdminPlat.Countries.Select(item => new { Value = item.Code, Text = item.Name }).OrderBy(item => item.Value).Json();
            //经停港口
            this.Model.DistinatePort = Needs.Wl.Admin.Plat.AdminPlat.BasePort.Select(item => new { Value = item.Code, Text = item.Name }).OrderBy(item => item.Value).Json();
            //成交方式
            this.Model.TransMode = Needs.Wl.Admin.Plat.AdminPlat.BaseTransMode.Select(item => new { Value = item.Code, Text = item.Name }).OrderBy(item => item.Value).Json();
            var FeeMarkList = getFeeMark();
            this.Model.FeeMark = FeeMarkList.Select(item => new { Value = item.Key, Text = item.Key + "-" + item.Value }).OrderBy(item => item.Value).Json();
            //保险费杂费标记
            var MarkList = getMark();
            this.Model.Mark = MarkList.Select(item => new { Value = item.Key, Text = item.Key + "-" + item.Value }).OrderBy(item => item.Value).Json();
            //币制
            this.Model.Currency = Needs.Wl.Admin.Plat.AdminPlat.Currencies.Select(item => new { Value = item.Code, Text = item.Name }).OrderBy(item => item.Value).Json();
            //包装种类
            this.Model.WrapType = Needs.Wl.Admin.Plat.AdminPlat.BasePackType.Select(item => new { Value = item.Code, Text = item.Code + " " + item.Name }).OrderBy(item => item.Value).Json();
            //报关单类型        
            this.Model.EntryType = getEntryType().Select(item => new { Value = item.Key, Text = item.Key + "-" + item.Value }).OrderBy(item => item.Value).Json();
            //入境口岸
            this.Model.EntyPortCode = Needs.Wl.Admin.Plat.AdminPlat.BaseEntryPort.Select(item => new { Value = item.Code, Text = item.Code + "-" + item.Name }).OrderBy(item => item.Value).Json();
            //报关转关关系标志
            this.Model.DeclTrnRel = getDeclTrnRel().Select(item => new { Value = item.Key, Text = item.Key + "-" + item.Value }).OrderBy(item => item.Value).Json();
            //备案清单类型
            this.Model.BillType = getBillType().Select(item => new { Value = item.Key, Text = item.Key + "-" + item.Value }).OrderBy(item => item.Value).Json();

        }

        private void DefaultLoadNEW(string OrderID)
        {
            string AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
            string AdminName = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName;
            this.Model.CurrentAdmin = new { ID = AdminID, Name = AdminName }.Json();           
            List<Needs.Ccs.Services.Models.Company> companies = GetCompanies();
            this.Model.DeclareCompanies = companies.Select(item => new { ID = item.Code, Text = item.Name }).Json();

            this.Model.DeclareCompanySZHY = new
            {
                Name = purchaser.CompanyName,
                Code = purchaser.Code,
                CustomsCode = purchaser.CustomsCode,
                CIQCode = purchaser.CiqCode,
                TypistNo = purchaser.ICCode,
                //DeclareName = purchaser.DeclareName
                DeclareName = "-"//20200805 魏晓毅 报关员不显示法人姓名 新增时赋空值
            }.Json();

            var vendor = new VendorContext(VendorContextInitParam.OrderID, OrderID).Current1;

            this.Model.ConsignorCompany = new { Name = vendor.OverseasConsignorCname, Code = vendor.CompanyName }.Json();
            List<Needs.Ccs.Services.Models.Company> Foreigncompanies = GetForeignCompanies(vendor);
            this.Model.ForgienCompany = Foreigncompanies.Select(item => new { ID = item.Name, Text = item.Name }).Json();
            var FeeMarkList = getFeeMark();
            this.Model.FeeMark = FeeMarkList.Select(item => new { Value = item.Key, Text = item.Key + "-" + item.Value }).OrderBy(item => item.Value).Json();
            //保险费杂费标记
            var MarkList = getMark();
            this.Model.Mark = MarkList.Select(item => new { Value = item.Key, Text = item.Key + "-" + item.Value }).OrderBy(item => item.Value).Json();
            //报关单类型        
            this.Model.EntryType = getEntryType().Select(item => new { Value = item.Key, Text = item.Key + "-" + item.Value }).OrderBy(item => item.Value).Json();
            //报关转关关系标志
            this.Model.DeclTrnRel = getDeclTrnRel().Select(item => new { Value = item.Key, Text = item.Key + "-" + item.Value }).OrderBy(item => item.Value).Json();
            //备案清单类型
            this.Model.BillType = getBillType().Select(item => new { Value = item.Key, Text = item.Key + "-" + item.Value }).OrderBy(item => item.Value).Json();

            this.Model.CustomMaster = DecHeadDefaultLoad.Current.LoadElements["CustomMaster"];
            //运输方式
            this.Model.TrafMode = DecHeadDefaultLoad.Current.LoadElements["TrafMode"];
            //监管方式
            this.Model.TradeMode = DecHeadDefaultLoad.Current.LoadElements["TradeMode"];
            //征免性质
            this.Model.CutMode = DecHeadDefaultLoad.Current.LoadElements["CutMode"];
            //启运国
            this.Model.TradeCountry = DecHeadDefaultLoad.Current.LoadElements["TradeCountry"];
            //经停港口
            this.Model.DistinatePort = DecHeadDefaultLoad.Current.LoadElements["DistinatePort"];
            //成交方式
            this.Model.TransMode = DecHeadDefaultLoad.Current.LoadElements["TransMode"];
            //币制
            this.Model.Currency = DecHeadDefaultLoad.Current.LoadElements["Currency"];
            //包装种类
            this.Model.WrapType = DecHeadDefaultLoad.Current.LoadElements["WrapType"];
            //入境口岸
            this.Model.EntyPortCode = DecHeadDefaultLoad.Current.LoadElements["EntyPortCode"];

            //默认申报地海关
            var CustomMasterDefault = new Needs.Ccs.Services.Views.BaseCustomMasterDefaultView().FirstOrDefault(t => t.IsDefault);
            this.Model.CustomMasterValue = CustomMasterDefault == null ? "5354" : CustomMasterDefault.Code;
        }
        private void NewLoad(string ClientID, string OrderID)
        {
            //消费使用单位            
            Needs.Ccs.Services.Models.Company Client = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Companies[ClientID];
            //this.Model.Client = new { Name = Client.Name, Code = Client.Code, CustomsCode = Client.CustomsCode }.Json();
            if (innerCompany.Any(t => t.Name == Client.Name))
            {
                this.Model.Client = new { Name = purchaser.CompanyName, Code = purchaser.Code, CustomsCode = purchaser.CustomsCode }.Json();
            }
            else
            {
                this.Model.Client = new { Name = Client.Name, Code = Client.Code, CustomsCode = Client.CustomsCode }.Json();
            }

            var vendor = new VendorContext(VendorContextInitParam.Pointed, Client.Name).Current1;

            this.Model.ConsignorCompany = new { Name = vendor.OverseasConsignorCname, Code = vendor.CompanyName }.Json();
            List<Needs.Ccs.Services.Models.Company> Foreigncompanies = GetForeignCompanies(vendor);
            this.Model.ForgienCompany = Foreigncompanies.Select(item => new { ID = item.Name, Text = item.Name }).Json();

            //计算件数，毛重，净重  
            string DeclarationNoticeID = Request.QueryString["NoticeID"];
            var declarationNotice = new Needs.Ccs.Services.Views.DeclarationNoticesView()[DeclarationNoticeID];
            //var Pakcings = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.Sorting.AsQueryable().Where(item=>item.OrderID== OrderID);

            if (declarationNotice != null)
            {
                var GrossWeight = Math.Round(declarationNotice.Items.Sum(item => item.Sorting.GrossWeight), 2, MidpointRounding.AwayFromZero);
                this.Model.GrossWeight = GrossWeight < ConstConfig.MinDecHeadGrossWeight ? ConstConfig.MinDecHeadGrossWeight : GrossWeight;
                var NetWeight = Math.Round(declarationNotice.Items.Sum(item => item.Sorting.NetWeight), 2, MidpointRounding.AwayFromZero);
                this.Model.NetWeight = NetWeight < ConstConfig.MinDecHeadNetWeight ? ConstConfig.MinDecHeadNetWeight : NetWeight; ;
                //此处计算太慢，改为参数传入 ryan 20211215
                this.Model.TotalPacks = Request.QueryString["TotalPack"];
            }
            else
            {
                this.Model.GrossWeight = new { }.Json();
                this.Model.NetWeight = new { }.Json();
                this.Model.TotalPacks = new { }.Json();
            }

            //获取航次号，自动带出运输批次号作为航次号，可以修改，不用选择，只能有一个，并且只能使用为截单的运输批次号
            //this.Model.VoyaNos = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.VoyageNos.Select(item => item.ID).FirstOrDefault().Json(); 

            //判断订单需要单独运输，取OrderVoyage中的运输批次号 
            //var orderVoyages = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.OrderVoyageNo.Where(t => t.Order.ID == OrderID).ToList();
            //this.Model.IsSingalTransport = orderVoyages.Count > 0 ? true : false;
            //if (this.Model.IsSingalTransport)
            //{
            //this.Model.VoyaNos = orderVoyages.Select(item => item.Voyage.ID).FirstOrDefault().Json();
            //}
            //else
            //{
            //    this.Model.VoyaNos = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.VoyageNos.Select(item => item.ID).FirstOrDefault().Json();
            //}
                     
            var decNoticeVoyage =  new Needs.Ccs.Services.Views.DecNoticeVoyagesOrigin().Where(t => t.DeclarationNotice.ID == DeclarationNoticeID).FirstOrDefault();
            this.Model.VoyaNos = decNoticeVoyage.Voyage?.ID;

            this.Model.DecHeadInfo = new { }.Json();
            this.Model.OtherPacks = new { }.Json();
        }

        private void EditLoad(Needs.Ccs.Services.Models.DecHead headinfo)
        {
            this.Model.Client = new { }.Json();
            this.Model.VoyaNos = new { }.Json();
            this.Model.GrossWeight = 0;
            this.Model.NetWeight = 0;
            this.Model.TotalPacks = 0;

            string SpecialRelationShip = "0", PriceConfirm = "0", PayConfirm = "0", FormulaPrice = "0", ProvisionalPrice = "0", Disinfect = "0";
            if (headinfo.PromiseItmes != null && headinfo.PromiseItmes != "")
            {
                SpecialRelationShip = headinfo.PromiseItmes.Substring(0, 1);
                PriceConfirm = headinfo.PromiseItmes.Substring(1, 1);
                PayConfirm = headinfo.PromiseItmes.Substring(2, 1);
                FormulaPrice = headinfo.PromiseItmes.Substring(3, 1);
                ProvisionalPrice = headinfo.PromiseItmes.Substring(4, 1);
                Disinfect = headinfo.PromiseItmes.Length < 6 ? "1" : headinfo.PromiseItmes.Substring(5, 1);
            }

            this.Model.DecHeadInfo = new
            {
                NoticeID = headinfo.DeclarationNoticeID,
                CustomMaster = headinfo.CustomMaster,
                SeqNo = "",
                PreEntryId = headinfo.PreEntryId,
                EntryId = headinfo.EntryId,
                IEPort = headinfo.IEPort,
                ManualNo = headinfo.ManualNo,
                ContrNo = headinfo.ContrNo,
                IEDate = headinfo.IEDate,
                DDate = headinfo.DDate,
                ConsigneeName = headinfo.ConsigneeName,
                ConsigneeScc = headinfo.ConsigneeScc,
                ConsigneeCusCode = headinfo.ConsigneeCusCode,
                ConsigneeCiqCode = headinfo.ConsigneeCiqCode,
                ConsignorName = headinfo.ConsignorName,
                ConsignorCode = headinfo.ConsignorCode,
                OwnerName = headinfo.OwnerName,
                OwnerScc = headinfo.OwnerScc,
                OwnerCusCode = headinfo.OwnerCusCode,
                OwnerCiqCode = headinfo.OwnerCiqCode,
                AgentName = headinfo.AgentName,
                AgentScc = headinfo.AgentScc,
                AgentCusCode = headinfo.AgentCusCode,
                AgentCiqCode = headinfo.AgentCiqCode,
                TrafMode = headinfo.TrafMode,
                TrafName = headinfo.TrafName,
                VoyNo = headinfo.VoyNo,
                BillNo = headinfo.BillNo,
                TradeMode = headinfo.TradeMode,
                CutMode = headinfo.CutMode,
                LicenseNo = headinfo.LicenseNo,
                TradeCountry = headinfo.TradeCountry,
                DistinatePort = headinfo.DistinatePort,
                TransMode = headinfo.TransMode,
                FeeMark = headinfo.FeeMark,
                FeeCurr = headinfo.FeeCurr,
                FeeRate = headinfo.FeeRate,
                InsurMark = headinfo.InsurMark,
                InsurCurr = headinfo.InsurCurr,
                InsurRate = headinfo.InsurRate,
                OtherMark = headinfo.OtherMark,
                OtherCurr = headinfo.OtherCurr,
                OtherRate = headinfo.OtherRate,
                PackNo = headinfo.PackNo,
                WrapType = headinfo.WrapType,
                GrossWt = headinfo.GrossWt,
                NetWt = headinfo.NetWt,
                TradeAreaCode = headinfo.TradeAreaCode,
                EntyPortCode = headinfo.EntyPortCode,
                GoodsPlace = headinfo.GoodsPlace,
                EntryType = headinfo.EntryType,
                DespPortCode = headinfo.DespPortCode,
                MarkNo = headinfo.MarkNo,
                NoteS = headinfo.NoteS,
                ApprNo = headinfo.ApprNo,
                DeclTrnRel = headinfo.DeclTrnRel,
                BillType = headinfo.BillType,
                InputerID = headinfo.Inputer.RealName,
                DeclareName = headinfo.DeclareName,
                TypistNo = headinfo.TypistNo,
                SpecialRelationShip = SpecialRelationShip,
                PriceConfirm = PriceConfirm,
                PayConfirm = PayConfirm,
                FormulaPrice = FormulaPrice,
                ProvisionalPrice = ProvisionalPrice,
                Disinfect = Disinfect,
                ChkSurety = headinfo.ChkSurety,
                Type = headinfo.Type,
            }.Json();

            this.Model.OtherPacks = headinfo.OtherPacks.Json();
        }

        protected void getDeclareCompnay()
        {
            string companycode = Request.Form["CompanyCode"];
            List<Needs.Ccs.Services.Models.Company> companies = GetCompanies();

            Needs.Ccs.Services.Models.Company reslut = companies.Where(item => item.Code == companycode).FirstOrDefault();
            if (reslut != null)
            {
                Response.Write(new
                {
                    Name = reslut.Name,
                    Code = reslut.Code,
                    CustomsCode = reslut.CustomsCode,
                    CIQCode = reslut.CIQCode,
                    Corporate = reslut.Corporate,
                    Summary = reslut.Summary,
                    //TODO:如何很好的区分报关公司？
                    TypistNo = purchaser.ICCode,
                    //DeclareName = purchaser.DeclareName
                    DeclareName = "-"//20200805 魏晓毅 报关员不显示法人姓名 新增时赋空值
                }.Json());
            }
        }

        protected void getBaseCustomMasterDefault()
        {
            string Code = Request.Form["Code"];
            var MasterDefault = Needs.Wl.Admin.Plat.AdminPlat.BaseCustomMasterDefault.Where(item => item.Code == Code && item.IsDefault).FirstOrDefault();
            if (MasterDefault != null)
            {
                Response.Write(new { IEPortCode = MasterDefault.IEPortCode, EntyPortCode = MasterDefault.EntyPortCode }.Json());
            }
        }

        protected void getContractNo()
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            DateTime dtStart = Convert.ToDateTime(today + " 00:00:00");
            DateTime dtEnd = Convert.ToDateTime(today + " 23:59:59");
            var HeadInfo = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHead.Where(item => item.CreateTime >= dtStart && item.CreateTime <= dtEnd).OrderByDescending(item => item.CreateTime).FirstOrDefault();
            if (HeadInfo != null)
            {
                //string contractno = HeadInfo.ContrNo;
                //string lastNo = (Convert.ToInt16(contractno.Substring(contractno.Length - 2, 2)) + 1).ToString().PadLeft(2, '0');
                Response.Write(new
                {
                    //ContrNo = "HY" + DateTime.Now.ToString("yyyyMM-dd-") + lastNo,
                    //BillNo = "CX" + DateTime.Now.ToString("MMdd") + lastNo,
                    ContrNo = "",
                    BillNo = "",
                    VoyNo = HeadInfo.VoyNo
                }.Json());
            }
            else
            {
                Response.Write(new
                {
                    ContrNo = purchaser.ContractNoPrefix + DateTime.Now.ToString("yyyyMM-dd-") + "01",
                    BillNo = purchaser.BillNoPrefix + DateTime.Now.ToString("MMdd") + "01",
                    VoyNo = ""
                }.Json());
            }
        }

        protected void SaveHead()
        {
            try
            {

                string OrderID = Request.Form["OrderID"];
                string DecHeadID = Request.Form["DecHeadID"];
                var TaxFeeTotal = 0M;
                var usingTax = 0M;
                
        

                if (!string.IsNullOrEmpty(DecHeadID))
                {
                    //修改
                    Needs.Ccs.Services.Models.DecHead head = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHead[DecHeadID];
                    #region 组装实体                  
                    head.DeclarationNoticeID = Request.Form["NoticeID"];
                    head.CustomMaster = Request.Form["CustomMaster"];

                    head.IEPort = Request.Form["IEPort"];
                    if (!string.IsNullOrEmpty(Request.Form["ManualNo"]))
                    {
                        head.ManualNo = Request.Form["ManualNo"];
                    }
                    //head.ContrNo = Request.Form["ContrNo"];
                    head.IEDate = Request.Form["IEDate"];
                    head.ConsigneeName = Request.Form["ConsigneeName"];
                    head.ConsigneeScc = Request.Form["ConsigneeScc"];
                    head.ConsigneeCusCode = Request.Form["ConsigneeCusCode"];
                    if (!string.IsNullOrEmpty(Request.Form["ConsigneeCiqCode"]))
                    {
                        head.ConsigneeCiqCode = Request.Form["ConsigneeCiqCode"];
                    }
                    head.ConsignorName = Request.Form["ConsignorCode"];
                    head.ConsignorCode = Request.Form["ConsignorName"];
                    head.OwnerName = Request.Form["OwnerName"];
                    head.OwnerScc = Request.Form["OwnerScc"];
                    head.OwnerCusCode = Request.Form["OwnerCusCode"];
                    head.OwnerCiqCode = Request.Form["OwnerCiqCode"];
                    head.AgentName = Request.Form["AgentName"];
                    head.AgentScc = Request.Form["AgentScc"];
                    head.AgentCusCode = Request.Form["AgentCusCode"];
                    head.AgentCiqCode = Request.Form["AgentCiqCode"];
                    head.TrafMode = Request.Form["TrafMode"];
                    if (!string.IsNullOrEmpty(Request.Form["TrafName"]))
                    {
                        head.TrafName = Request.Form["TrafName"];
                    }
                    head.VoyNo = Request.Form["VoyNo"];
                    //head.BillNo = Request.Form["BillNo"];
                    head.TradeMode = Request.Form["TradeMode"];
                    head.CutMode = Request.Form["CutMode"];
                    if (!string.IsNullOrEmpty(Request.Form["LicenseNo"]))
                    {
                        head.LicenseNo = Request.Form["LicenseNo"];
                    }
                    head.TradeCountry = Request.Form["TradeCountry"];
                    head.DistinatePort = Request.Form["DistinatePort"];
                    head.TransMode = Convert.ToInt16(Request.Form["TransMode"]);
                    if (!string.IsNullOrEmpty(Request.Form["FeeMark"]))
                    {
                        head.FeeMark = Convert.ToInt16(Request.Form["FeeMark"]);
                        head.FeeCurr = Request.Form["FeeCurr"];
                        head.FeeRate = Convert.ToDecimal(Request.Form["FeeRate"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Form["InsurMark"]))
                    {
                        head.InsurMark = Convert.ToInt16(Request.Form["InsurMark"]);
                        head.InsurCurr = Request.Form["InsurCurr"];
                        head.InsurRate = Convert.ToDecimal(Request.Form["InsurRate"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Form["OtherMark"]))
                    {
                        head.OtherMark = Convert.ToInt16(Request.Form["OtherMark"]);
                        head.OtherCurr = Request.Form["OtherCurr"];
                        head.OtherRate = Convert.ToDecimal(Request.Form["OtherRate"]);
                    }
                    head.PackNo = Convert.ToInt16(Request.Form["PackNo"]);
                    head.WrapType = Request.Form["WrapType"];
                    head.GrossWt = Convert.ToDecimal(Request.Form["GrossWt"]);
                    head.NetWt = Convert.ToDecimal(Request.Form["NetWt"]);
                    head.TradeAreaCode = Request.Form["TradeAreaCode"];
                    head.EntyPortCode = Request.Form["EntyPortCode"];
                    head.GoodsPlace = Request.Form["GoodsPlace"];
                    head.EntryType = Request.Form["EntryType"];

                    head.DespPortCode = Request.Form["DespPortCode"];
                    head.MarkNo = Request.Form["MarkNo"];

                    head.NoteS = Request.Form["NoteS"];
                    head.ApprNo = Request.Form["ApprNo"];
                    head.DeclTrnRel = Convert.ToInt16(Request.Form["DeclTrnRel"]);
                    head.BillType = Convert.ToInt16(Request.Form["BillType"]);
                    //head.InputerID = Request.Form["InputerID"];
                    head.DeclareName = Request.Form["DeclareName"];
                    head.TypistNo = Request.Form["TypistNo"];
                    head.PromiseItmes = Request.Form["SpecialRelationShip"] + Request.Form["PriceConfirm"] + Request.Form["PayConfirm"] + Request.Form["FormulaPrice"] + Request.Form["ProvisionalPrice"]; //+ Request.Form["Disinfect"];//                FormulaPrice       ProvisionalPrice
                    head.ChkSurety = Convert.ToInt16(Request.Form["ChkSurety"]);
                    if (Request.Form["Type"] != null)
                    {
                        head.Type = Request.Form["Type"];
                    }
                    #endregion

                    //其他包装
                    var test = Request.Form["OtherPacks"].Replace("&quot;", "\'");
                    dynamic model = test.JsonTo<dynamic>();
                    head.OtherPacks.RemoveAll();
                    foreach (var item in model)
                    {
                        head.OtherPacks.Add(new Needs.Ccs.Services.Models.DecOtherPack
                        {
                            PackType = item.PackType,
                            PackQty = item.PackQty,
                        });
                    }
                    head.DeclareHeadChange();
                }
                else
                {
                    #region  申报时进行海关税费额度预警
                    //DateTime dtToday = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));//今天
                    //DateTime dtNexDay = Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));//明天
                    ////获取当天已使用的税费
                    //var taxFee = new Needs.Ccs.Services.Views.DecTaxQuotasView().Where(x => x.Status == Status.Normal && x.PayStatus == TaxStatus.Unpaid && x.CreateDate > dtToday && x.CreateDate < dtNexDay).ToArray();
                    //var usedTax = 0M;
                    //foreach (var item in taxFee)
                    //{
                    //    usedTax = item.AddedValueTax + item.Tariff;
                    //}
                    // usingTax = 1000000 - usedTax;
                    /////获取当前需要报关的报关单的税费
                    //var entity = new DecTaxQuota();
                    //var currentHeadTaxFee = new CustomsFeeService().GetCustomsFeeSum(OrderID);
                    // TaxFeeTotal = currentHeadTaxFee.TaxFeeTotal;
                    //if (TaxFeeTotal > usingTax)
                    //{
                    //    Response.Write(new { result = false, info = "额度不足，请先进行缴税" }.Json());
                    //    return;
                    //}
                    //else if (TaxFeeTotal < usingTax && usingTax < 200000)
                    //{
                    //    Response.Write(new { result = false, info = "税费额度即将使用完" }.Json());
                    //    //记录报关税费
                    //    entity.DeclarationID = string.Concat(PurchaserContext.Current.DecHeadIDPrefix, Needs.Overall.PKeySigner.Pick(Needs.Ccs.Services.PKeyType.DecHead));
                    //    entity.AddedValueTax = currentHeadTaxFee.AddedValue.Value;
                    //    entity.Tariff = currentHeadTaxFee.TariffValue.Value;
                    //    entity.PayStatus = TaxStatus.Unpaid;
                    //    entity.Enter();
                    //}
                    //else
                    //{
                    //    //记录报关税费
                    //    entity.DeclarationID = string.Concat(PurchaserContext.Current.DecHeadIDPrefix, Needs.Overall.PKeySigner.Pick(Needs.Ccs.Services.PKeyType.DecHead));
                    //    entity.AddedValueTax = currentHeadTaxFee.AddedValue.Value;
                    //    entity.Tariff = currentHeadTaxFee.TariffValue.Value;
                    //    entity.PayStatus = TaxStatus.Unpaid;
                    //    entity.Enter();
                    //}
                    #endregion
                    //新增
                    string voyNoID = Request.Form["VoyNo"];
                    //var voyNo = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.VoyageNos[voyNoID];
                    var voyNo = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.ManifestSure[voyNoID];
                    if (voyNo != null)
                    {
                        if (voyNo.CutStatus != CutStatus.UnCutting)
                        {
                            Response.Write(new { result = false, info = "请输入未截单的运输批次号" }.Json());
                            return;
                        }
                    }
                    else
                    {
                        Response.Write(new { result = false, info = "请输入有效的航次号" }.Json());
                        return;
                    }

                    //订单
                    var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders[OrderID];

                    #region 组装实体
                    Needs.Ccs.Services.Models.DecHead head = new Needs.Ccs.Services.Models.DecHead();
                    ///  head.ID = Needs.Overall.PKeySigner.Pick(PKeyType.DecHead);
                    //head.ID = string.Concat(PurchaserContext.Current.DecHeadIDPrefix, Needs.Overall.PKeySigner.Pick(Needs.Ccs.Services.PKeyType.DecHead));
                    head.ID = string.Concat(PurchaserContext.Current.DecHeadIDPrefix, Needs.Overall.PKeySigner.Pick(Needs.Ccs.Services.PKeyType.DecHead));
                    head.DeclarationNoticeID = Request.Form["NoticeID"];
                    head.OrderID = OrderID;
                    head.CustomMaster = Request.Form["CustomMaster"].Trim();
                    head.SeqNo = "";
                    head.PreEntryId = "";
                    head.EntryId = "";
                    head.IEPort = Request.Form["IEPort"];
                    if (!string.IsNullOrEmpty(Request.Form["ManualNo"]))
                    {
                        head.ManualNo = Request.Form["ManualNo"];
                    }
                    //head.ContrNo = Request.Form["ContrNo"];
                    head.IEDate = Request.Form["IEDate"];
                    head.DDate = DateTime.Now;
                    head.ConsigneeName = Request.Form["ConsigneeName"].Trim();
                    head.ConsigneeScc = Request.Form["ConsigneeScc"].Trim();
                    head.ConsigneeCusCode = Request.Form["ConsigneeCusCode"].Trim();
                    if (!string.IsNullOrEmpty(Request.Form["ConsigneeCiqCode"]))
                    {
                        head.ConsigneeCiqCode = Request.Form["ConsigneeCiqCode"].Trim();
                    }
                    head.ConsignorName = Request.Form["ConsignorCode"].Trim();
                    head.ConsignorCode = Request.Form["ConsignorName"].Trim();
                    head.OwnerName = Request.Form["OwnerName"].Trim();
                    head.OwnerScc = Request.Form["OwnerScc"].Trim();
                    head.OwnerCusCode = Request.Form["OwnerCusCode"].Trim();
                    head.OwnerCiqCode = Request.Form["OwnerCiqCode"].Trim();
                    head.AgentName = Request.Form["AgentName"].Trim();
                    head.AgentScc = Request.Form["AgentScc"].Trim();
                    head.AgentCusCode = Request.Form["AgentCusCode"].Trim();
                    head.AgentCiqCode = Request.Form["AgentCiqCode"].Trim();
                    head.TrafMode = Request.Form["TrafMode"];
                    if (!string.IsNullOrEmpty(Request.Form["TrafName"]))
                    {
                        head.TrafName = Request.Form["TrafName"].Trim();
                    }
                    head.VoyNo = Request.Form["VoyNo"];
                    //head.BillNo = Request.Form["BillNo"];
                    head.TradeMode = Request.Form["TradeMode"];
                    head.CutMode = Request.Form["CutMode"];
                    if (!string.IsNullOrEmpty(Request.Form["LicenseNo"]))
                    {
                        head.LicenseNo = Request.Form["LicenseNo"];
                    }
                    head.TradeCountry = Request.Form["TradeCountry"];
                    head.DistinatePort = Request.Form["DistinatePort"];
                    head.TransMode = Convert.ToInt16(Request.Form["TransMode"]);
                    if (!string.IsNullOrEmpty(Request.Form["FeeMark"]))
                    {
                        head.FeeMark = Convert.ToInt16(Request.Form["FeeMark"]);
                        head.FeeCurr = Request.Form["FeeCurr"];
                        head.FeeRate = Convert.ToDecimal(Request.Form["FeeRate"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Form["InsurMark"]))
                    {
                        head.InsurMark = Convert.ToInt16(Request.Form["InsurMark"]);
                        head.InsurCurr = Request.Form["InsurCurr"];
                        head.InsurRate = Convert.ToDecimal(Request.Form["InsurRate"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Form["OtherMark"]))
                    {
                        head.OtherMark = Convert.ToInt16(Request.Form["OtherMark"]);
                        head.OtherCurr = Request.Form["OtherCurr"];
                        head.OtherRate = Convert.ToDecimal(Request.Form["OtherRate"]);
                    }
                    head.PackNo = Convert.ToInt16(Request.Form["PackNo"]);
                    head.WrapType = Request.Form["WrapType"];
                    head.GrossWt = Convert.ToDecimal(Request.Form["GrossWt"]);
                    head.NetWt = Convert.ToDecimal(Request.Form["NetWt"]);
                    head.TradeAreaCode = Request.Form["TradeAreaCode"];
                    head.EntyPortCode = Request.Form["EntyPortCode"];
                    head.GoodsPlace = Request.Form["GoodsPlace"];
                    head.EntryType = Request.Form["EntryType"];

                    head.DespPortCode = Request.Form["DespPortCode"];
                    head.MarkNo = Request.Form["MarkNo"];

                    head.NoteS = Request.Form["NoteS"];
                    head.ApprNo = Request.Form["ApprNo"];
                    head.DeclTrnRel = Convert.ToInt16(Request.Form["DeclTrnRel"]);
                    head.BillType = Convert.ToInt16(Request.Form["BillType"]);
                    head.Inputer = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                    head.DeclareName = Request.Form["DeclareName"];
                    head.TypistNo = Request.Form["TypistNo"];
                    head.PromiseItmes = Request.Form["SpecialRelationShip"] + Request.Form["PriceConfirm"] + Request.Form["PayConfirm"] + Request.Form["FormulaPrice"] + Request.Form["ProvisionalPrice"];// + Request.Form["Disinfect"];// FormulaPrice       ProvisionalPrice
                    head.ChkSurety = Convert.ToInt16(Request.Form["ChkSurety"]);
                    if (Request.Form["Type"] != null)
                    {
                        head.Type = Request.Form["Type"];
                    }
                    #endregion
                    head.CreateTime = DateTime.Now;
                    head.CustomsExchangeRate = order.CustomsExchangeRate;
                    head.MKImport = false;
                    head.IsSuccess = false;
                    //head.DespDate = head.IEDate;

                    #region 其它包装

                    var test = Request.Form["OtherPacks"].Replace("&quot;", "\'");
                    List<Needs.Ccs.Services.Models.DecOtherPack> otherPakcList = new List<Needs.Ccs.Services.Models.DecOtherPack>();
                    dynamic model = test.JsonTo<dynamic>();
                    if (model != null)
                    {
                        foreach (var item in model)
                        {
                            head.OtherPacks.Add(new Needs.Ccs.Services.Models.DecOtherPack
                            {
                                ID = ChainsGuid.NewGuidUp(),
                                PackType = item.PackType,
                                PackQty = item.PackQty,
                                DeclarationID = head.DeclarationNoticeID,
                            });
                        }
                    }


                    #endregion


                    #region 表体信息

                    var DeclarationNoticeItems = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DelareNotice[head.DeclarationNoticeID].Items.AsQueryable();

                    var i = 1;

                    List<string> orderItemIDS = new List<string>();

                    var sortingorderInfo = new Needs.Ccs.Services.Views.HKSortingsView().Where(item => item.OrderID == OrderID).
                                       OrderBy(item => item.BoxIndex).Select(t=>new {
                                           ID = t.ID,
                                           WrapType = t.WrapType
                                       });
                    var sortingorder = sortingorderInfo.Select(t => t.ID);
                    var wrapTypes = sortingorderInfo.Select(t => t.WrapType);
                    var CountryNameView = new Needs.Ccs.Services.Views.BaseCountriesView();

                    //按装箱顺序排列
                    foreach (var itemorder in sortingorder)
                    {
                        var item = DeclarationNoticeItems.Where(t => t.Sorting.ID == itemorder).FirstOrDefault();
                        orderItemIDS.Add(item.Sorting.OrderItem.ID);
                        var list = new Needs.Ccs.Services.Models.DecList();
                        list.ID = string.Concat(head.ID, i).MD5();
                        list.DecListID = Needs.Overall.PKeySigner.Pick(Needs.Ccs.Services.PKeyType.DecListItem);
                        list.DeclarationID = head.ID;
                        list.DeclarationNoticeItemID = DeclarationNoticeItems.Where(a => a.Sorting.ID == item.Sorting.ID).Select(b => b.ID).FirstOrDefault();
                        list.OrderID = item.Sorting.OrderID;
                        list.OrderItemID = item.Sorting.OrderItem.ID;
                        list.CusDecStatus = CusItemDecStatus.Normal;
                        list.GNo = i;
                        list.CodeTS = item.Sorting.OrderItem.Category.HSCode;
                        list.CiqCode = item.Sorting.OrderItem.Category.CIQCode;
                        list.GName = item.Sorting.OrderItem.Category.Name;
                        list.GModel = item.Sorting.OrderItem.Category.Elements;
                        list.GQty = item.Sorting.Quantity;
                        list.GUnit = item.Sorting.OrderItem.Unit;
                        list.FirstUnit = item.Sorting.OrderItem.Category.Unit1;
                        list.SecondUnit = item.Sorting.OrderItem.Category.Unit2;
                        list.TradeCurr = order.Currency;
                        list.OriginCountry = item.Sorting.OrderItem.Origin;
                        list.OriginCountryName = CountryNameView.Where(t => t.Code == list.OriginCountry).Select(t => t.Name).FirstOrDefault() == null ? "" : CountryNameView.Where(t => t.Code == list.OriginCountry).Select(t => t.Name).FirstOrDefault();
                        //计算价格
                        var totalPrice = item.Sorting.OrderItem.TotalPrice * Needs.Ccs.Services.ConstConfig.TransPremiumInsurance * (item.Sorting.Quantity / item.Sorting.OrderItem.Quantity);
                        list.DeclPrice = (totalPrice / item.Sorting.Quantity).ToRound(4);
                        list.DeclTotal = totalPrice.ToRound(2);
                       
                        //冗余项
                        list.CaseNo = item.Sorting.BoxIndex;
                        list.NetWt = item.Sorting.NetWeight < 0.01M ? 0.01M : item.Sorting.NetWeight.ToRound(2);
                        list.GrossWt = item.Sorting.GrossWeight < 0.02M ? 0.02M : item.Sorting.GrossWeight.ToRound(2);
                        list.GoodsModel = item.Sorting.OrderItem.Model;
                        list.GoodsBrand = item.Sorting.OrderItem.Manufacturer;

                        list.OrderPrice = item.Sorting.OrderItem.UnitPrice;
                        list.OrderTotal = (list.OrderPrice * list.GQty).ToRound(2);

                        head.Lists.Add(list);

                        i++;
                    }




                    #endregion

                    #region 商检/检疫  
                    var vendor = new VendorContext(VendorContextInitParam.Pointed, head.OwnerName).Current1;
                    //判断包装里面是否含有木托板
                    var WoodPackType = Needs.Wl.Admin.Plat.AdminPlat.BasePackType.Where(t=>t.Name.Contains("木")).Select(t=>t.Code).ToList();
                    var isWoodRelated = wrapTypes.Any(t => WoodPackType.Contains(t));
                    //判断是否商检/检疫,不是判断这个订单中的所有项是否要商检，而是判断该报关单的所有型号是否要商检，一个订单可以拆分成好几个报关单
                    var isQuarantines = order.Items.Where(item => orderItemIDS.Contains(item.ID)).Any(t => (t.Category.Type.GetHashCode() & Needs.Ccs.Services.Enums.ItemCategoryType.Quarantine.GetHashCode()) > 0);
                    var isInspections = order.Items.Where(item => orderItemIDS.Contains(item.ID)).Any(
                        t => (t.Category.Type.GetHashCode() & Needs.Ccs.Services.Enums.ItemCategoryType.Inspection.GetHashCode()) > 0
                        || (t.Category.Type.GetHashCode() & Needs.Ccs.Services.Enums.ItemCategoryType.CCC.GetHashCode()) > 0);

                    isQuarantines = isQuarantines || isWoodRelated;
                    //一车只有一单检疫的
                    isQuarantines = isQuarantines && !new Needs.Ccs.Services.Views.DecHeadsListView().Any(t => t.VoyageID == head.VoyNo && t.IsQuarantine && t.Status != "04");

                    if (isQuarantines || isInspections)
                    {
                        //检验检疫信息
                        //根据申报关别，带出检疫机关
                        var masterDefault = new Needs.Ccs.Services.Views.BaseCustomMasterDefaultView().Where(t => t.Code == head.CustomMaster && t.IsDefault == true)?.FirstOrDefault();
                        if (masterDefault != null)
                        {
                            head.OrgCode = masterDefault.OrgCode;
                            head.VsaOrgCode = masterDefault.VsaOrgCode;
                            head.InspOrgCode = masterDefault.InspOrgCode;
                            head.PurpOrgCode = masterDefault.PurpOrgCode;
                        }
                        head.DespDate = head.IEDate;//启运日期改为 必填项  //20230301 不再必填
                        head.UseOrgPersonCode = head.AgentName == purchaser.CompanyName ? purchaser.UseOrgPersonCode : KRComany.UseOrgPersonCode;
                        head.UseOrgPersonTel = head.AgentName == purchaser.CompanyName ? purchaser.UseOrgPersonTel : KRComany.UseOrgPersonTel;
                        head.DomesticConsigneeEname = head.AgentName == purchaser.CompanyName ? purchaser.DomesticConsigneeEname : KRComany.DomesticConsigneeEname;
                        head.OverseasConsignorCname = vendor.OverseasConsignorCname;
                        head.OverseasConsignorAddr = vendor.OverseasConsignorAddr;
                        //TODO:数据库可以直接使用datetime类型，报关是进行格式转换
                        //head.CmplDschrgDt = Convert.ToDateTime(head.DespDate.Insert(4, "-").Insert(7, "-")).AddDays(3).ToString("yyyyMMdd");
                        //卸毕日期改为当前日期向后三天
                        head.CmplDschrgDt = DateTime.Now.AddDays(3).ToString("yyyyMMdd");

                        //head.ContrNo = head.ContrNo.Replace(purchaser.ContractNoPrefix, purchaser.SJContractNoPrefix);
                        //head.BillNo = head.BillNo.Replace(purchaser.BillNoPrefix, purchaser.SJBillNoPrefix);

                        //处理商检/检疫的表体
                        foreach (var item in head.Lists)
                        {
                            var orderitem = order.Items.FirstOrDefault(t => t.ID == item.OrderItemID);
                            var isItemInspection = (orderitem.Category.Type.GetHashCode() & Needs.Ccs.Services.Enums.ItemCategoryType.Inspection.GetHashCode()) > 0;
                            var isItemCCC = (orderitem.Category.Type.GetHashCode() & Needs.Ccs.Services.Enums.ItemCategoryType.CCC.GetHashCode()) > 0;
                            //var isItemQuarantine = (orderitem.Category.Type.GetHashCode() & Needs.Ccs.Services.Enums.ItemCategoryType.Quarantine.GetHashCode()) > 0;

                            item.GoodsSpec = "***";
                            item.Purpose = "99";//用途 默认：99 其它
                            item.GoodsAttr = isItemCCC ? "11,19" : (isItemInspection ? "12,19" : "19");//默认：19 正常
                            item.CiqName = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.CustomsCiqCodes.FirstOrDefault(t => t.ID == (orderitem.Category.HSCode + orderitem.Category.CIQCode))?.Name;
                            //item.GoodsBatch = (string.IsNullOrEmpty(orderitem.Batch) || (isQuarantines && !isInspections)) ? "***" : orderitem.Batch;
                            item.GoodsBatch = "***";//20220325 weixiaoyi 默认***：以实物为准  by ryan
                        }
                    }
                    else
                    {
                        //防止再次制单，默认使用了商检的合同号
                        //head.ContrNo = head.ContrNo.Replace(purchaser.SJContractNoPrefix, purchaser.ContractNoPrefix);
                    }

                    head.IsInspection = isInspections;
                    head.IsQuarantine = isQuarantines;

                    //foreach (var item in head.Lists)
                    //{
                    //    if (head.IsInspection || head.IsQuarantine.Value)
                    //    {
                    //        item.GoodsSpec = ";;;;;" + item.GoodsModel + ";" + item.GoodsBrand + ";"+ order.Items.Where(t => t.ID == item.OrderItemID).Select(t=>t.Product.Batch) + ";";
                    //        item.Purpose = "99";//用途 默认：99 其它
                    //        item.GoodsAttr = "12,19";//默认：19 正常
                    //    }
                    //}


                    #endregion
                    head.CreateDeclare();
                }
                //剩余可用额度
               
                Response.Write(new { result = true, info = "保存成功，当前剩余可用额度为：" +(usingTax- TaxFeeTotal) +" 人民币" }.Json());
            }
            catch (Exception ex)
            {
                Response.Write(new { result = false, info = "保存错误" + ex.ToString() }.Json());
            }

        }

        /// <summary>
        /// 转换舱单
        /// </summary>
        protected void Transform()
        {
            try
            {
                string id = Request.Form["ID"];
                var head = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.DecHeads[id];
                head.ToManifest();

                Response.Write((new { success = true, message = "转换成功！" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "转换失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 批量制单
        /// </summary>
        protected void Make()
        {
            try
            {
                string id = Request.Form["ID"];
                var head = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.DecHeads[id];
                foreach (var p in head.Lists)
                {
                    string UnknownCountryModels = "";
                    if (p.OriginCountry == Needs.Ccs.Services.Icgoo.UnknownCountry)
                    {
                        UnknownCountryModels += p.GNo + ";";
                    }
                    if (UnknownCountryModels != "")
                    {
                        //Response.Write((new { success = false, message = "项号:"+ UnknownCountryModels.Substring(0, UnknownCountryModels.Length-1) + "原产地未识别，请先更改！" }).Json());
                        Response.Write((new { success = false, message = "有原产地未识别的型号，请先更改！" }).Json());
                        return;
                    }

                }
                head.Make();
                Response.Write((new { success = true, message = "已申报！" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "申报失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 导出Excel走泰州物流报关
        /// </summary>
        protected void DownloadExcel()
        {
            try
            {
                //1.创建文件夹(文件压缩后存放的地址)
                FileDirectory file = new FileDirectory();
                file.SetChildFolder(Needs.Ccs.Services.SysConfig.Export);
                file.CreateDataDirectory();

                string ID = Request.Form["ID"];
                //string CustomSubmiterAdminID = Request.Form["CustomSubmiterAdminID"];

                //var vendor = new VendorContext(VendorContextInitParam.DecHeadID, ID).Current1;
                Needs.Ccs.Services.Models.DecHead headinfo = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHead[ID];

                var vendor = new VendorContext(VendorContextInitParam.Pointed, headinfo.OwnerName).Current1;

                //因制单员复核前已经验证了产地，此处不再验证
                //foreach (var p in headinfo.Lists)
                //{
                //    string UnknownCountryModels = "";
                //    if (p.OriginCountry == Needs.Ccs.Services.Icgoo.UnknownCountry)
                //    {
                //        UnknownCountryModels += p.GNo + ";";
                //    }
                //    if (UnknownCountryModels != "")
                //    {
                //        //Response.Write((new { success = false, info = UnknownCountryModels.Substring(0, UnknownCountryModels.Length - 1) + "原产地未识别，请先更改！" }).Json());
                //        Response.Write((new { success = false, info = "有原产地未识别的型号，请先更改！" }).Json());
                //        return;
                //    }

                //}



                ExcelDeclareDocument excel = new ExcelDeclareDocument(headinfo);
                string FileName = headinfo.ContrNo + ".xlsx";
                string DomainUrl = System.Configuration.ConfigurationManager.AppSettings["DomainUrl"];
                string FileAddress = file.FileUrl.Replace(DomainUrl, "");
                excel.setFilePath(FileAddress.Substring(1, FileAddress.Length - 1));
                excel.SaveAs(FileName, vendor);
                Response.Write(new { result = true, info = "导出成功", url = file.FileUrl + FileName }.Json());
            }
            catch (Exception ex)
            {
                Response.Write(new { result = false, info = "保存错误" + ex.ToString() }.Json());
            }

        }

        private List<Needs.Ccs.Services.Models.Company> GetCompanies()
        {
            List<Needs.Ccs.Services.Models.Company> companies = new List<Needs.Ccs.Services.Models.Company>();

            Needs.Ccs.Services.Models.Company SZHY = new Needs.Ccs.Services.Models.Company();
            SZHY.Name = purchaser.CompanyName;
            SZHY.Code = purchaser.Code;
            SZHY.CustomsCode = purchaser.CustomsCode;
            SZHY.CIQCode = purchaser.CiqCode;
            SZHY.Corporate = purchaser.UseOrgPersonCode;
            SZHY.Summary = purchaser.ICCode;

            companies.Add(SZHY);

            //Company SZNew = new Company();
            //SZNew.Name = KRComany.CompanyName;
            //SZNew.Code = KRComany.Code;
            //SZNew.CustomsCode = KRComany.CustomsCode;
            //SZNew.CIQCode = KRComany.CiqCode;
            //SZNew.Corporate = KRComany.UseOrgPersonCode;
            //SZNew.Summary = KRComany.ICCode;

            //companies.Add(SZNew);
            return companies;
        }

        private List<Needs.Ccs.Services.Models.Company> GetForeignCompanies(Vendor vendor)
        {
            List<Needs.Ccs.Services.Models.Company> companies = new List<Needs.Ccs.Services.Models.Company>();

            Needs.Ccs.Services.Models.Company HKHT = new Needs.Ccs.Services.Models.Company();
            HKHT.Name = vendor.OverseasConsignorCname;
            HKHT.Code = vendor.OverseasConsignorCname;
            HKHT.Summary = vendor.CompanyName;
            companies.Add(HKHT);

            return companies;
        }

        private Dictionary<int, string> getFeeMark()
        {
            Dictionary<int, string> mark = new Dictionary<int, string>();
            mark.Add(1, "率");
            mark.Add(2, "单价");
            mark.Add(3, "总价");

            return mark;
        }

        private Dictionary<int, string> getMark()
        {
            Dictionary<int, string> mark = new Dictionary<int, string>();
            mark.Add(1, "率");
            mark.Add(3, "总价");

            return mark;
        }

        private Dictionary<string, string> getType()
        {
            Dictionary<string, string> mark = new Dictionary<string, string>();
            mark.Add("CL", "汇总征税报关单");
            mark.Add("LY", "两单一审备案清单");
            mark.Add("MF", "公路舱单跨境快速通关");
            mark.Add("MK", "公路舱单跨境快速通关备案清单");
            mark.Add("ML", "保税区进出境备案清单");
            mark.Add("MT", "多式联运");
            mark.Add("SD", "属地申报 口岸验放报关单");
            mark.Add("SL", "水运中转两单一审备案清单");
            mark.Add("SM", "水运中转保税区进出境备案清单");
            mark.Add("SS", "属地申报 属地验放报关单");
            mark.Add("SW", "税单无纸化");
            mark.Add("SZ", "水运中转普通报关单");
            mark.Add("Z", "自报自缴");
            mark.Add("ZB", "自主报税");

            return mark;
        }

        private Dictionary<string, string> getEntryType()
        {
            Dictionary<string, string> mark = new Dictionary<string, string>();
            mark.Add("0", "普通报关单");
            mark.Add("L", "未带报关单清单的报关单");
            mark.Add("W", "无纸报关类型");
            mark.Add("D", "既是清单又是无纸报关的情况");
            mark.Add("M", "无纸化通关");

            return mark;
        }

        private Dictionary<string, string> getDeclTrnRel()
        {
            Dictionary<string, string> mark = new Dictionary<string, string>();
            mark.Add("0", "一般报关单");
            mark.Add("1", "转关提前报关单");

            //mark.Add("1", "普通报关");
            //mark.Add("3", "北方转关提前");
            //mark.Add("5", "南方转关提前");
            //mark.Add("6", "普通报关 运输工具名称以◎开头 南方H2000直转");

            return mark;
        }

        private Dictionary<string, string> getBillType()
        {
            Dictionary<string, string> mark = new Dictionary<string, string>();
            mark.Add("1", "普通备案清单");
            mark.Add("2", "先进区后报关");
            mark.Add("3", "分送集报备案清单");
            mark.Add("4", "分送集报报关单");

            return mark;
        }
    }
}