using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class DecHeadsView : UniqueView<Models.DecHead, ScCustomsReponsitory>
    {
        public DecHeadsView()
        {
        }

        internal DecHeadsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.DecHead> GetIQueryable()
        {
            var adminView = new AdminsTopView(this.Reponsitory);

            return from head in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                   join admin in adminView on head.InputerID equals admin.ID into g
                   from temp in g.DefaultIfEmpty()
                   select new Models.DecHead
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
                       IsQuarantine = head.IsQuarantine,
                       DeclareName = head.DeclareName,
                       TypistNo = head.TypistNo,
                       Inputer = temp,
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
                       SwapStatus = (Enums.SwapStatus)head.SwapStatus,
                       IsSuccess=head.IsSuccess,
                       SubmitCustomAdminID = head.SubmitCustomAdminID,
                       DoubleCheckerAdminID = head.DoubleCheckerAdminID,
                       MKImport = head.MKImport
                   };
        }
    }
}
