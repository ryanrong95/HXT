using Needs.Utils;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 报关单报文拓展
    /// </summary>
    public partial class DecMessage
    {
        public DecMessage(DecHead decHead) : this()
        {
            var hasOtherPack = decHead.OtherPacks.Count() > 0;
            //表头信息
            DecHeadType headType = new DecHeadType();

            headType.IEFlag = "I";
            headType.Type = decHead.Type;
            headType.AgentCode = decHead.AgentCusCode;//"7663746551";
            headType.AgentName = decHead.AgentName; //"深圳市南方电子口岸有限公司";
            headType.ApprNo = decHead.ApprNo;
            headType.BillNo = decHead.BillNo;// "TX180703TD";
            headType.ContrNo = decHead.ContrNo;// "TX180703XY";
            headType.CustomMaster = decHead.CustomMaster;// "5345";
            //headType.SeqNo = decHead.SeqNo;
            headType.CutMode = decHead.CutMode;// "101";
            headType.DistinatePort = decHead.DistinatePort;//"HKG000";
            headType.FeeCurr = decHead.FeeCurr;// "HKG";
            headType.FeeMark = decHead.FeeMark.ToString(); // "3";
            headType.FeeRate = decHead.FeeRate == null ? decHead.FeeRate.ToString() : decHead.FeeRate.Value.ToRound(4).ToString(); // "1000";
            headType.GrossWet = decHead.GrossWt.ToString(); //"2.0000";
            headType.IEDate = decHead.IEDate;// "20180703";
            headType.IEPort = decHead.IEPort;// "5345";
            headType.InsurCurr = decHead.InsurCurr;
            headType.InsurMark = decHead.InsurMark.ToString(); // "3";
            headType.InsurRate = decHead.InsurRate == null ? decHead.InsurRate.ToString() : decHead.InsurRate.Value.ToRound(4).ToString(); // "1000";
            headType.LicenseNo = decHead.LicenseNo; // "20-18-030701";
            headType.ManualNo = decHead.ManualNo; // "T20180703001";
            headType.NetWt = decHead.NetWt.ToString();// "1.1900";
            headType.NoteS = decHead.NoteS;// "12T车测试";
            headType.OtherCurr = decHead.OtherCurr; // "HKG";
            headType.OtherMark = decHead.OtherMark.ToString(); // "3";
            headType.OtherRate = decHead.OtherRate == null ? decHead.OtherRate.ToString() : decHead.OtherRate.Value.ToRound(4).ToString(); // "1000";
            headType.OwnerCode = decHead.OwnerCusCode;// "7663746551";
            headType.OwnerName = decHead.OwnerName;// "深圳市南方电子口岸有限公司";
            headType.PackNo = decHead.PackNo.ToString();// "3";
            headType.TradeCode = decHead.ConsigneeCusCode;// "7663746551";
            headType.TradeCountry = decHead.TradeCountry;// "HKG";
            headType.TradeMode = decHead.TradeMode;// "0110";
            headType.TradeName = decHead.ConsigneeName;// "深圳市南方电子口岸有限公司";
            headType.TrafMode = decHead.TrafMode;// "2";
            headType.TrafName = decHead.TrafName; // "UN20170703";
            headType.TransMode = decHead.TransMode.ToString();// "3";
            headType.WrapType = decHead.WrapType;// "22";
            headType.EdiId = "0";// ;
            //headType.Risk = string.Empty;
            headType.CopName = decHead.AgentName;// "深圳市南方电子口岸有限公司";
            headType.CopCode = decHead.AgentCusCode;// "766374655";
            headType.EntryType = decHead.EntryType;// "M";
            headType.PDate = DateTime.Now.ToString("yyyyMMdd");// "20170703";
            headType.TypistNo = decHead.TypistNo;//ic卡号
            //TODO:此处填写录入员姓名
            headType.InputerName = decHead.DeclareName;//decHead.InputerID;
            headType.DeclTrnRel = decHead.DeclTrnRel.ToString();// "0";
            headType.ChkSurety = decHead.ChkSurety.ToString();
            //headType.BillType = decHead.BillType.ToString();
            headType.CopCodeScc = decHead.AgentScc;// "914403007663746551";
            headType.OwnerCodeScc = decHead.OwnerScc;// "914403007663746551";
            headType.AgentCodeScc = decHead.AgentScc;// "914403007663746551";
            headType.TradeCoScc = decHead.ConsigneeScc;// "914403007663746551";
            headType.PromiseItmes = decHead.PromiseItmes;// "000";
            headType.TradeAreaCode = decHead.TradeAreaCode;// "HKG";
            //headType.CheckFlow = string.Empty;
            //headType.TaxAaminMark = string.Empty;
            headType.MarkNo = decHead.MarkNo;// "M/N";
            headType.DespPortCode = decHead.DespPortCode;// "HKG000";
            headType.EntyPortCode = decHead.EntyPortCode;// "471401";
            headType.GoodsPlace = decHead.GoodsPlace;// "货物存放地点（深圳）";
            headType.BLNo = decHead.BLNo;// "TX18060750BL";
            headType.InspOrgCode = decHead.InspOrgCode;// "471400";
            headType.PurpOrgCode = decHead.PurpOrgCode;// "471400";
            headType.DespDate = decHead.DespDate;// "20180703";
            headType.CmplDschrgDt = decHead.CmplDschrgDt;// "20180704";
            headType.CorrelationReasonFlag = decHead.CorrelationReasonFlag;// "1";
            headType.VsaOrgCode = decHead.VsaOrgCode;// "471400";
            headType.OrigBoxFlag = decHead.OrigBoxFlag;// "0";
            headType.DeclareName = decHead.DeclareName;
            headType.NoOtherPack = !hasOtherPack ? "0" : "1";
            headType.OrgCode = decHead.OrgCode;// "471400";
            //headType.OverseasConsignorCode = string.Empty;
            headType.OverseasConsignorCname = decHead.OverseasConsignorCname;// "境外收发货人名称(中文)";
            headType.OverseasConsignorEname = decHead.ConsignorName;// "Hong Kong Overseas Consignor Ename";
            headType.OverseasConsignorAddr = decHead.OverseasConsignorAddr;// "境外发货人地址";
            //headType.OverseasConsigneeCode = string.Empty;
            //headType.OverseasConsigneeEname = string.Empty;
            headType.DomesticConsigneeEname = decHead.DomesticConsigneeEname;// "Domestic Consignee Ename";
            headType.CorrelationNo = decHead.CorrelationNo;// "TX20180703GLHM";
            //headType.EdiRemark2 = string.Empty;
            //headType.EdiRemark = string.Empty;
            headType.TradeCiqCode = decHead.ConsigneeCiqCode;
            headType.OwnerCiqCode = decHead.OwnerCiqCode;
            headType.DeclCiqCode = decHead.AgentCiqCode;

            this.DecHead = headType;

            //是否商检检疫
            bool isInsp = decHead.IsInspection || decHead.IsQuarantine.Value;

            var specialList = new List<CustomsSpecialGoodsAttr>();
            //两步申报时获取特殊税号
            //if (decHead.IsSplitDeclare)
            //{
            //    specialList = new Views.CustomsSpecialGoodsAttrView().ToList();
            //}
            //2023-10-10 李文忠 不管一步还是两步都需要设置为特殊属性
            specialList = new Views.CustomsSpecialGoodsAttrView().ToList();

            //表体信息
            this.DecLists = decHead.Lists.Select(item => new DecListItemType
            {
                CodeTS = item.CodeTS,
                DeclPrice = item.DeclPrice.ToRound(4).ToString(),
                DutyMode = item.DutyMode.ToString(),
                GModel = item.GModel,
                GName = item.GName,
                GNo = item.GNo.ToString(),
                OriginCountry = item.OriginCountry,
                TradeCurr = item.TradeCurr,
                DeclTotal = item.DeclTotal.ToRound(2).ToString(),
                GQty = item.GQty.ToString(),
                FirstQty = item.FirstQty.ToString(),
                SecondQty = item.SecondQty.ToString(),
                GUnit = item.GUnit,
                FirstUnit = item.FirstUnit,
                SecondUnit = item.SecondUnit,
                DestinationCountry = item.DestinationCountry,
                CiqCode = isInsp == true ? item.CiqCode : "",
                CiqName = isInsp == true ? item.CiqName : "",
                //decListItem.DeclGoodsEname = "Integrated circuit";
                OrigPlaceCode = item.OrigPlaceCode,
                Purpose = isInsp == true ? item.Purpose : "",
                //decListItem.ProdQgp = "1000";
                //魏晓毅 20230131 电话需求： 两步申报时，遇到某些特殊税号的 需要传货物属性(3C目录外)
                //GoodsAttr = isInsp == true ? item.GoodsAttr:"",
                GoodsAttr = (isInsp == true ? item.GoodsAttr :
                (specialList.Any(t => t.HSCode == item.CodeTS) ? specialList.FirstOrDefault(t => t.HSCode == item.CodeTS).GoodsAttr : "")),
                //decListItem.Stuff = "集成电路成份/原料/组份";
                //decListItem.EngManEntCnm = "集成电路境外生产企业";
                DestCode = item.DestCode,
                GoodsSpec = isInsp == true ? item.GoodsSpec : "",
                GoodsModel = isInsp == true ? item.GoodsModel : "",
                GoodsBrand = isInsp == true ? item.GoodsBrand : "",
                ProdBatchNo = isInsp == true ? item.GoodsBatch : "",
                //decListItem.ProduceDate = "20180603";
                //decListItem.ProdBatchNo = "TX20180703PC";
                DistrictCode = item.DistrictCode,

                //许可证
                DecGoodsLimits = item.Limits.Select(t => new DecGoodsLimitType
                {
                    GoodsNo = item.GNo.ToString(),
                    LicTypeCode = t.LicTypeCode,
                    LicenceNo = t.LicenceNo,
                    LicWrtofDetailNo = t.LicWrtofDetailNo,
                    LicWrtofQty = t.LicWrtofQty,
                    LicWrtofQtyUnit = t.LicWrtofQtyUnit
                }).ToArray()
            }).ToArray();

            //集装箱信息
            this.DecContainers = decHead.Containers.Select(item => new DecContainerType
            {
                ContainerId = item.ContainerID,
                ContainerMd = item.ContainerMd.ToString(),
                GoodsNo = item.GoodsNo,
                LclFlag = item.LclFlag.ToString(),
                //GoodsContaWt = item.GoodsContaWt.ToString()
            }).ToArray();

            //随附单证信息
            this.DecLicenseDocus = decHead.LicenseDocus.Select(item => new DecLicenseType
            {
                DocuCode = item.DocuCode,
                CertCode = item.CertCode
            }).ToArray();

            //申请单证信息
            this.DecRequestCerts = decHead.RequestCerts.Select(item => new DecRequestCertType
            {
                AppCertCode = item.AppCertCode,
                ApplOri = item.ApplOri.ToString(),
                ApplCopyQuan = item.ApplCopyQuan.ToString()
            }).ToArray();

            //其它包装信息
            this.DecOtherPacks = decHead.OtherPacks.Select(item => new DecOtherPackType
            {
                PackQty = item.PackQty.ToString(),
                PackType = item.PackType
            }).ToArray();

            #region DecUsers 检疫和商检才需要

            ///TODO:缺少 是否商检 和 检疫
            var decUsers = new List<DecUserType>();
            DecUserType decUserItem = new DecUserType();
            decUserItem.UseOrgPersonCode = decHead.UseOrgPersonCode;
            decUserItem.UseOrgPersonTel = decHead.UseOrgPersonTel;
            decUsers.Add(decUserItem);

            this.DecUsers = decUsers.ToArray();

            #endregion

            #region 其它(作占位 暂无业务需要)

            this.DecFreeTxt = new DecFreeTxtType();
            this.DecFreeTxt.VoyNo = decHead.VoyNo;

            #endregion

            #region DecSign 签名信息

            var sign = new DecSignType();
            sign.OperType = "G";//报关单暂存
            sign.ICCode = this.DecHead.TypistNo;
            sign.CopCode = this.DecHead.CopCode;
            sign.OperName = this.DecHead.InputerName;
            sign.ClientSeqNo = decHead.ID;//decHead.IsSuccess ? decHead.ID + "0" : decHead.ID;
            sign.Sign = string.Empty;
            sign.SignDate = string.Empty;
            sign.Certificate = string.Empty;
            sign.HostId = string.Empty;
            sign.BillSeqNo = string.Empty;
            sign.DomainId = string.Empty;
            sign.Note = string.Empty;
            this.DecSign = sign;

            #endregion

            #region 其它(作占位 暂无业务需要)

            //this.TrnHead = new TrnHeadType();
            //this.TrnList = new TrnListType();
            //message.TrnContainers = new List<TrnContainerType>().ToArray();
            //message.TrnContaGoodsList = new List<TrnContaGoodsType>().ToArray();
            //this.DecSupplementLists = new List<DecSupplementListType>().ToArray();

            #endregion

            #region 其它(作占位 暂无业务需要)

            //message.EcoRelation = new List<ECO_RealationType>().ToArray();
            //message.SddTax = new DecMessageSddTax();
            //message.SddTax.SddTaxHead = new DecMessageSddTaxSddTaxHead();
            //message.SddTax.SddTaxLists = new List<DecMessageSddTaxSddTaxList>().ToArray();
            //message.SddTax.SddTaxDetails = new List<DecMessageSddTaxSddTaxDetail>().ToArray();
            //message.DecRisk = new DecRiskType();
            //message.DecCopPromises = new List<DecCopPromiseType>().ToArray();

            #endregion

            #region DecCopPromises 企业承诺信息 检疫和商检才需要

            if (decHead.IsInspection)
            {
                var decCopPromises = new List<DecCopPromiseType>();
                DecCopPromiseType decdecCopPromisesItem = new DecCopPromiseType();
                //证明/声明材料代码 进口填写：“101040”; 出口填写：“102053”.
                decdecCopPromisesItem.DeclaratioMaterialCode = "101040";
                decCopPromises.Add(decdecCopPromisesItem);

                this.DecCopPromises = decCopPromises.ToArray();
            }

            #endregion

            #region EdocRealation 电子随附单据关联关系

            this.EdocRealation = decHead.EdocRealations.Select(item => new Edoc_RealationType
            {
                EdocID = item.EdocID,
                EdocCode = item.EdocCode,
                EdocFomatType = item.EdocFomatType,
                OpNote = item.OpNote,
                EdocCopId = item.EdocCopId,
                EdocOwnerCode = item.EdocOwnerCode,
                SignUnit = item.EdocOwnerCode,
                SignTime = item.SignTime.ToString("yyyyMMdd hh:mm:ss"),
                EdocOwnerName = item.EdocOwnerName,
                EdocSize = item.EdocSize
            }).ToArray();

            #endregion
        }

        /// <summary>
        /// 转换为xml
        /// </summary>
        /// <returns></returns>
        public string ToXml()
        {
            return this.Xml(Encoding.GetEncoding("utf-8"), true, false, false);
        }

        /// <summary>
        /// XML保存
        /// </summary>
        /// <param name="fileName">保存到硬盘的文件名称</param>
        /// <returns></returns>
        public string SaveAs(string fileName)
        {
            //创建文件目录
            FileDirectory fileDic = new FileDirectory(fileName);
            fileDic.SetChildFolder(SysConfig.DecMessageDirectory);
            fileDic.CreateDataDirectory();

            var xmldoc = new System.Xml.XmlDocument();
            xmldoc.LoadXml(this.ToXml());
            xmldoc.Save(fileDic.FilePath);

            return fileDic.VirtualPath;
        }
    }
}