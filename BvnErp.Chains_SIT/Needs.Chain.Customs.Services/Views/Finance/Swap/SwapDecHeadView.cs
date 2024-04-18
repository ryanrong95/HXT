using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 换汇通知的视图
    /// </summary>
    public class SwapDecHeadView : UniqueView<Models.SwapDecHead, ScCustomsReponsitory>
    {
        public SwapDecHeadView()
        {
        }

        internal SwapDecHeadView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<SwapDecHead> GetIQueryable()
        {
            var decHeadView = new DecHeadsView(this.Reponsitory);
            var fileView = new DecHeadFilesView(this.Reponsitory);
            var decListView = new Views.DecOriginListsView(this.Reponsitory);

            var result = from head in decHeadView
                         join file in fileView on head.ID equals file.DecHeadID into files
                         join declist in decListView on head.ID equals declist.DeclarationID into declists
                         select new Models.SwapDecHead
                         {
                             ID = head.ID,
                             DeclarationNoticeID = head.DeclarationNoticeID,
                             OrderID = head.OrderID,
                             CustomMaster = head.CustomMaster,
                             CusDecStatus = head.CusDecStatus,
                             SeqNo = head.SeqNo,
                             PreEntryId = head.PreEntryId,
                             EntryId = head.EntryId,
                             IEPort = head.IEPort,
                             ManualNo = head.ManualNo,
                             ContrNo = head.ContrNo,
                             IEDate = head.IEDate,
                             DDate = head.DDate,
                             ConsigneeName = head.ConsigneeName,
                             ConsigneeScc = head.ConsigneeScc,
                             ConsigneeCusCode = head.ConsigneeCusCode,
                             ConsigneeCiqCode = head.ConsigneeCiqCode,
                             ConsignorName = head.ConsignorName,
                             ConsignorCode = head.ConsignorCode,
                             OwnerName = head.OwnerName,
                             OwnerScc = head.OwnerScc,
                             OwnerCusCode = head.OwnerCusCode,
                             OwnerCiqCode = head.OwnerCiqCode,
                             AgentName = head.AgentName,
                             AgentScc = head.AgentScc,
                             AgentCusCode = head.AgentCusCode,
                             AgentCiqCode = head.AgentCiqCode,
                             TrafMode = head.TrafMode,
                             TrafName = head.TrafName,
                             VoyNo = head.VoyNo,
                             BillNo = head.BillNo,
                             TradeMode = head.TradeMode,
                             CutMode = head.CutMode,
                             LicenseNo = head.LicenseNo,
                             TradeCountry = head.TradeCountry,
                             DistinatePort = head.DistinatePort,
                             TransMode = head.TransMode,
                             FeeCurr = head.FeeCurr,
                             FeeMark = head.FeeMark,
                             FeeRate = head.FeeRate,
                             InsurCurr = head.InsurCurr,
                             InsurMark = head.InsurMark,
                             InsurRate = head.InsurRate,
                             OtherCurr = head.OtherCurr,
                             OtherMark = head.OtherMark,
                             OtherRate = head.OtherRate,
                             PackNo = head.PackNo,
                             WrapType = head.WrapType,
                             GrossWt = head.GrossWt,
                             NetWt = head.NetWt,
                             TradeAreaCode = head.TradeAreaCode,
                             EntyPortCode = head.EntyPortCode,
                             GoodsPlace = head.GoodsPlace,
                             DespPortCode = head.DespPortCode,
                             EntryType = head.EntryType,
                             NoteS = head.NoteS,
                             MarkNo = head.MarkNo,
                             PromiseItmes = head.PromiseItmes,
                             ChkSurety = head.ChkSurety,
                             Type = head.Type,
                             ApprNo = head.ApprNo,
                             DeclTrnRel = head.DeclTrnRel,
                             BillType = head.BillType,
                             CustomsExchangeRate = head.CustomsExchangeRate,
                             IsInspection = head.IsInspection,
                             TypistNo = head.TypistNo,
                             Inputer = head.Inputer,
                             OrgCode = head.OrgCode,
                             VsaOrgCode = head.VsaOrgCode,
                             InspOrgCode = head.InspOrgCode,
                             PurpOrgCode = head.PurpOrgCode,
                             DespDate = head.DespDate,
                             BLNo = head.BLNo,
                             CorrelationNo = head.CorrelationNo,
                             CorrelationReasonFlag = head.CorrelationReasonFlag,
                             OrigBoxFlag = head.OrigBoxFlag,
                             SpecDeclFlag = head.SpecDeclFlag,
                             UseOrgPersonCode = head.UseOrgPersonCode,
                             UseOrgPersonTel = head.UseOrgPersonTel,
                             DomesticConsigneeEname = head.DomesticConsigneeEname,
                             OverseasConsignorCname = head.OverseasConsignorCname,
                             OverseasConsignorAddr = head.OverseasConsignorAddr,
                             CmplDschrgDt = head.CmplDschrgDt,
                             CreateTime = head.CreateTime,
                             MarkingUrl = head.MarkingUrl,
                             SwapStatus = head.SwapStatus,
                             //报关单文件
                             files = files,
                             Currency = declists.First() == null ? "" : declists.First().TradeCurr,
                             SwapAmount = declists.Sum(t=>t.DeclTotal),
                         };
            return result;
        }
    }
}
