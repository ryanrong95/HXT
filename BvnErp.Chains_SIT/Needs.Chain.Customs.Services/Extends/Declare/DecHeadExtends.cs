using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public static class DecHeadExtends
    {
        public static Layer.Data.Sqls.ScCustoms.DecHeads ToLinq(this Models.DecHead entity)
        {
            return new Layer.Data.Sqls.ScCustoms.DecHeads
            {
                ID = entity.ID,
                DeclarationNoticeID = entity.DeclarationNoticeID,
                OrderID = entity.OrderID,
                CustomMaster = entity.CustomMaster,
                CusDecStatus = entity.CusDecStatus,
                SeqNo = entity.SeqNo,
                PreEntryId = entity.PreEntryId,
                EntryId = entity.EntryId,
                IEPort = entity.IEPort,
                ManualNo = entity.ManualNo,
                ContrNo = entity.ContrNo,

                IEDate = entity.IEDate,
                DDate = entity.DDate,
                ConsigneeName = entity.ConsigneeName,
                ConsigneeScc = entity.ConsigneeScc,
                ConsigneeCusCode = entity.ConsigneeCusCode,
                ConsigneeCiqCode = entity.ConsigneeCiqCode,
                ConsignorName = entity.ConsignorName,
                ConsignorCode = entity.ConsignorCode,
                OwnerName = entity.OwnerName,
                OwnerScc = entity.OwnerScc,

                OwnerCusCode = entity.OwnerCusCode,
                OwnerCiqCode = entity.OwnerCiqCode,
                AgentName = entity.AgentName,
                AgentScc = entity.AgentScc,
                AgentCusCode = entity.AgentCusCode,
                AgentCiqCode = entity.AgentCiqCode,
                TrafMode = entity.TrafMode,
                TrafName = entity.TrafName,
                VoyNo = entity.VoyNo,
                BillNo = entity.BillNo,

                TradeMode = entity.TradeMode,
                CutMode = entity.CutMode,
                LicenseNo = entity.LicenseNo,
                TradeCountry = entity.TradeCountry,
                DistinatePort = entity.DistinatePort,
                TransMode = entity.TransMode,
                FeeCurr = entity.FeeCurr,
                FeeMark = entity.FeeMark,
                FeeRate = entity.FeeRate,
                InsurCurr = entity.InsurCurr,

                InsurMark = entity.InsurMark,
                InsurRate = entity.InsurRate,
                OtherCurr = entity.OtherCurr,
                OtherMark = entity.OtherMark,
                OtherRate = entity.OtherRate,
                PackNo = entity.PackNo,
                WrapType = entity.WrapType,
                GrossWt = entity.GrossWt,
                NetWt = entity.NetWt,
                TradeAreaCode = entity.TradeAreaCode,

                EntyPortCode = entity.EntyPortCode,
                GoodsPlace = entity.GoodsPlace,
                DespPortCode = entity.DespPortCode,
                EntryType = entity.EntryType,
                NoteS = entity.NoteS,
                MarkNo = entity.MarkNo,
                PromiseItmes = entity.PromiseItmes,
                ChkSurety = entity.ChkSurety,
                Type = entity.Type,
                ApprNo = entity.ApprNo,

                DeclTrnRel = entity.DeclTrnRel,
                BillType = entity.BillType,
                CustomsExchangeRate = entity.CustomsExchangeRate,
                IsInspection = entity.IsInspection,
                IsQuarantine = entity.IsQuarantine,
                DeclareName = entity.DeclareName,
                TypistNo = entity.TypistNo,
                InputerID = entity.Inputer.ID,
                OrgCode = entity.OrgCode,
                VsaOrgCode = entity.VsaOrgCode,
                InspOrgCode = entity.InspOrgCode,
                PurpOrgCode = entity.PurpOrgCode,
                DespDate = entity.DespDate,

                BLNo = entity.BLNo,
                CorrelationNo = entity.CorrelationNo,
                CorrelationReasonFlag = entity.CorrelationReasonFlag,
                OrigBoxFlag = entity.OrigBoxFlag,
                SpecDeclFlag = entity.SpecDeclFlag,
                UseOrgPersonCode = entity.UseOrgPersonCode,
                UseOrgPersonTel = entity.UseOrgPersonTel,
                DomesticConsigneeEname = entity.DomesticConsigneeEname,
                OverseasConsignorCname = entity.OverseasConsignorCname,
                OverseasConsignorAddr = entity.OverseasConsignorAddr,

                CmplDschrgDt = entity.CmplDschrgDt,
                CreateTime = entity.CreateTime,
                MarkingUrl = entity.MarkingUrl,
                SwapStatus = (int)entity.SwapStatus,
                IsSuccess = entity.IsSuccess,
                MKImport = entity.MKImport
            };
        }
    }
}
