using Needs.Ccs.Services.Enums;
using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 制单数据，用于创建报关单
    /// </summary>
    public class DeclareSearchList : IUnique
    {
        #region 属性

        /// <summary>
        /// 主键ID（CDO+8位年月日+6位流水号）
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 报关通知ID
        /// </summary>
        public string DeclarationNoticeID { get; set; }

        /// <summary>
        /// 主管海关/申报地海关 5345
        /// </summary>
        public string CustomMaster { get; set; }

        /// <summary>
        /// 申报状态
        /// </summary>
        public string CusDecStatus { get; set; }

        /// <summary>
        /// 数据中心统一编号
        /// </summary>
        public string SeqNo { get; set; }

        /// <summary>
        /// 预录入编号
        /// </summary>
        public string PreEntryId { get; set; }

        /// <summary>
        /// 报关单号/海关编号
        /// </summary>
        public string EntryId { get; set; }

        /// <summary>
        /// 进境关别 5345
        /// </summary>
        public string IEPort { get; set; }

        /// <summary>
        /// 备案号/手册号
        /// </summary>
        public string ManualNo { get; set; }

        /// <summary>
        /// 合同号
        /// </summary>
        public string ContrNo { get; set; }

        /// <summary>
        /// 进口日期
        /// </summary>
        public string IEDate { get; set; }

        /// <summary>
        /// 申报日期
        /// </summary>
        public DateTime? DDate { get; set; }

        /// <summary>
        /// 境内收发货人
        /// </summary>
        public string ConsigneeName { get; set; }

        /// <summary>
        /// 境内收发货人18位统一社会信用代码
        /// </summary>
        public string ConsigneeScc { get; set; }

        /// <summary>
        /// 境内收发货人 10位海关编码
        /// </summary>
        public string ConsigneeCusCode { get; set; }

        /// <summary>
        /// 境内收发货人 10位检验检疫编码
        /// </summary>
        public string ConsigneeCiqCode { get; set; }

        /// <summary>
        /// 境外发货人代码
        /// </summary>
        public string ConsignorName { get; set; }

        /// <summary>
        /// 境外发货人英文名称
        /// </summary>
        public string ConsignorCode { get; set; }

        /// <summary>
        /// 消费使用/生产销售单位名称
        /// </summary>
        public string OwnerName { get; set; }

        /// <summary>
        /// 货主企业单位 消费使用/生产销售单位代码
        /// </summary>
        public string OwnerScc { get; set; }

        /// <summary>
        /// 消费使用单位海关代码
        /// </summary>
        public string OwnerCusCode { get; set; }

        /// <summary>
        /// 消费使用单位 10位检验检疫编码
        /// </summary>
        public string OwnerCiqCode { get; set; }

        /// <summary>
        /// 申报单位名称
        /// </summary>
        public string AgentName { get; set; }

        /// <summary>
        /// 申报单位 统一社会信用代码
        /// </summary>
        public string AgentScc { get; set; }

        /// <summary>
        /// 申报企业单位 10位海关代码
        /// </summary>
        public string AgentCusCode { get; set; }

        /// <summary>
        /// 申报单位 检验检疫编码
        /// </summary>
        public string AgentCiqCode { get; set; }

        /// <summary>
        /// 运输方式
        /// </summary>
        public string TrafMode { get; set; }

        /// <summary>
        /// 运输工具代码及名称
        /// </summary>
        public string TrafName { get; set; }

        /// <summary>
        /// 航次号
        /// </summary>
        public string VoyNo { get; set; }

        /// <summary>
        /// 提单号
        /// </summary>
        public string BillNo { get; set; }

        /// <summary>
        /// 监管方式
        /// </summary>
        public string TradeMode { get; set; }

        /// <summary>
        /// 征免性质
        /// </summary>
        public string CutMode { get; set; }

        /// <summary>
        /// 许可证编号
        /// </summary>
        public string LicenseNo { get; set; }

        /// <summary>
        /// 启运国/运抵国
        /// </summary>
        public string TradeCountry { get; set; }

        /// <summary>
        /// 经停港/指运港
        /// </summary>
        public string DistinatePort { get; set; }

        /// <summary>
        /// 成交方式
        /// </summary>
        public int TransMode { get; set; }

        /// <summary>
        /// 运费币制
        /// </summary>
        public string FeeCurr { get; set; }

        /// <summary>
        /// 运费标记 1：率 ，2：单价， 3：总价
        /// </summary>
        public int? FeeMark { get; set; }

        /// <summary>
        /// 运费/率
        /// </summary>
        public decimal? FeeRate { get; set; }

        /// <summary>
        /// 保险费币制
        /// </summary>
        public string InsurCurr { get; set; }

        /// <summary>
        /// 保险费标记 1：率 3：总价
        /// </summary>
        public int? InsurMark { get; set; }

        /// <summary>
        /// 保险费/率
        /// </summary>
        public decimal? InsurRate { get; set; }

        /// <summary>
        /// 杂费币制
        /// </summary>
        public string OtherCurr { get; set; }

        /// <summary>
        /// 杂费标记
        /// </summary>
        public int? OtherMark { get; set; }

        /// <summary>
        /// 杂费/率
        /// </summary>
        public decimal? OtherRate { get; set; }

        /// <summary>
        /// 件数
        /// </summary>
        public int PackNo { get; set; }

        /// <summary>
        /// 包装方式
        /// </summary>
        public string WrapType { get; set; }

        /// <summary>
        /// 毛重
        /// </summary>
        public decimal GrossWt { get; set; }

        /// <summary>
        /// 净重
        /// </summary>
        public decimal NetWt { get; set; }

        /// <summary>
        /// 贸易国别 HKG
        /// </summary>
        public string TradeAreaCode { get; set; }

        /// <summary>
        /// 入境口岸 471401
        /// </summary>
        public string EntyPortCode { get; set; }

        /// <summary>
        /// 货物存放地点
        /// </summary>
        public string GoodsPlace { get; set; }

        /// <summary>
        /// 启运港
        /// </summary>
        public string DespPortCode { get; set; }

        /// <summary>
        /// 报关单类型
        /// </summary>
        public string EntryType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string NoteS { get; set; }

        /// <summary>
        /// 标记及号码 默认 N/M
        /// </summary>
        public string MarkNo { get; set; }

        /// <summary>
        /// 其他关系确认 1：是，0：否，9：空    默认否 
        ///1勾选 0-未选9-空
        ///第一位特殊关系确认
        ///第二位价格影响确认
        ///第三位支付特许权使用费确认
        /// </summary>
        public string PromiseItmes { get; set; }

        /// <summary>
        /// 担保验放标志 默认否
        /// </summary>
        public int? ChkSurety { get; set; }

        /// <summary>
        /// 单据类型：一般为空：
        /// 属地报关SD；
        /// 备案清单：ML。
        /// LY：两单一审备案清单。
        /// CL:汇总征税报关单。
        /// SS:”属地申报，属地验放”报关单；
        /// MT:多式联运,Z:自报自缴。
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 批准文号/外汇核销单号
        /// </summary>
        public string ApprNo { get; set; }

        /// <summary>
        /// 报关/转关关系标志
        /// </summary>
        public int DeclTrnRel { get; set; }

        /// <summary>
        /// 备案清单类型
        /// </summary>
        public int? BillType { get; set; }

        /// <summary>
        /// 冗余 是否商检
        /// </summary>
        public bool IsInspection { get; set; }

        /// <summary>
        /// 是否检疫
        /// </summary>
        public bool IsQuarantine { get; set; }

        /// <summary>
        /// 录入人员IC卡号
        /// </summary>
        public string TypistNo { get; set; }

        /// <summary>
        /// 录入人
        /// </summary>
        public string InputerID { get; set; }

        /// <summary>
        /// 检验检疫受理机关 471400
        /// </summary>
        public string OrgCode { get; set; }

        /// <summary>
        /// 领证机关
        /// </summary>
        public string VsaOrgCode { get; set; }

        /// <summary>
        /// 口岸检验检疫机关
        /// </summary>
        public string InspOrgCode { get; set; }

        /// <summary>
        /// 目的地检验检疫机关
        /// </summary>
        public string PurpOrgCode { get; set; }

        /// <summary>
        /// 启运日期
        /// </summary>
        public string DespDate { get; set; }

        /// <summary>
        /// B/L号
        /// </summary>
        public string BLNo { get; set; }

        /// <summary>
        /// 关联号码
        /// </summary>
        public string CorrelationNo { get; set; }

        /// <summary>
        /// 关联理由
        /// </summary>
        public string CorrelationReasonFlag { get; set; }

        /// <summary>
        /// 原箱运输 1：是  ，0：否
        /// </summary>
        public string OrigBoxFlag { get; set; }

        /// <summary>
        /// 特种业务标识
        /// </summary>
        public string SpecDeclFlag { get; set; }

        /// <summary>
        /// 使用单位联系人
        /// </summary>
        public string UseOrgPersonCode { get; set; }

        /// <summary>
        /// 使用单位联系电话
        /// </summary>
        public string UseOrgPersonTel { get; set; }

        /// <summary>
        /// 境内收发货人名称(外文)
        /// </summary>
        public string DomesticConsigneeEname { get; set; }

        /// <summary>
        /// 境外收发货人名称(中文)
        /// </summary>
        public string OverseasConsignorCname { get; set; }

        /// <summary>
        /// 境外发货人地址
        /// </summary>
        public string OverseasConsignorAddr { get; set; }

        /// <summary>
        /// 卸毕日期
        /// </summary>
        public string CmplDschrgDt { get; set; }

        /// <summary>
        /// 录单时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 制单XML地址
        /// </summary>
        public string MarkingUrl { get; set; }

        /// <summary>
        /// 合同后缀
        /// </summary>
        public int ContrNoSuffix
        {
            get
            {
                try
                {
                    string[] contrNos = this.ContrNo.Split('-');
                    int suffix = Convert.ToInt32(contrNos[2]);
                    return suffix;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }

        #endregion

        /// <summary>
        /// 报关单表体
        /// </summary>
        DecLists lits;
        public DecLists List
        {
            get
            {
                if (lits == null)
                {
                    using (var view = new Views.DecOriginListsView())
                    {
                        var query = view.Where(item => item.DeclarationID == this.ID).OrderBy(item=>item.CaseNo);
                        this.List = new DecLists(query);
                    }
                }
                return this.lits;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.lits = new DecLists(value, new Action<DecList>(delegate (DecList item)
                {
                    item.DeclarationID = this.ID;
                }));
            }
        }





    }
}
