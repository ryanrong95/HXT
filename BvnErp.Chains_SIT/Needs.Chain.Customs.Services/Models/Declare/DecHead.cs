using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Linq;
using Needs.Utils.Converters;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.Grid;
using Spire.Pdf.Tables;
using System.IO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Utils.Serializers;
using Needs.Utils;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Needs.Ccs.Services.Views;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 报关单
    /// 销售单
    /// 销售单号（合同号）
    /// </summary>
    [Serializable]
    public class DecHead : IUnique, IPersist
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
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 主管海关/申报地海关 5345
        /// </summary>
        public string CustomMaster { get; set; }

        /// <summary>
        /// 申报状态
        /// </summary>
        public string CusDecStatus { get; set; }

        /// <summary>
        /// 是否申报成功
        /// </summary>
        //public bool IsDeclareSuccess
        //{
        //    get
        //    {
        //        if (
        //            this.CusDecStatus == MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.Succeed) ||
        //            this.CusDecStatus == MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.B) ||
        //            this.CusDecStatus == MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.K) ||
        //            this.CusDecStatus == MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.G) ||
        //            this.CusDecStatus == MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.P) ||
        //            this.CusDecStatus == MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.E1) ||
        //            this.CusDecStatus == MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.R))
        //        {
        //            return true;
        //        }
        //        return false;
        //    }
        //}
        public bool IsSuccess { get; set; }


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
        virtual public string ManualNo { get; set; }

        private string contrNo;

        /// <summary>
        /// 合同号
        /// </summary>
        //public string ContrNo
        //{
        //    get
        //    {
        //        return this.contrNo ?? this.CreateContractNo();
        //    }
        //    set
        //    {
        //        this.contrNo = value;
        //    }
        //}
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

        private string voyNo { get; set; }
        /// <summary>
        /// 航次号
        /// </summary>
        public string VoyNo
        {
            get
            {
                return this.voyNo ?? this.CreateVoyNo();
            }
            set
            {
                this.voyNo = value;
            }
        }

        private string billNo { get; set; }

        /// <summary>
        /// 提单号
        /// </summary>
        public string BillNo
        {
            get
            {
                return this.billNo ?? this.CreateBillNo();
            }
            set
            {
                this.billNo = value;
            }
        }

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
        /// 是否两步申报
        /// </summary>
        public bool IsSplitDeclare
        {
            get
            {
                if (Type.Length > 2 && Type.Substring(2, 1) == "3")
                {
                    return true;
                }
                return false;
            }
        }

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
        /// 报关完成时海关汇率
        /// </summary>
        public decimal? CustomsExchangeRate { get; set; }

        /// <summary>
        /// 冗余 是否商检
        /// </summary>
        public bool IsInspection { get; set; }
        /// <summary>
        /// 冗余 是否检疫
        /// </summary>
        public bool? IsQuarantine { get; set; }

        /// <summary>
        /// 申报人姓名
        /// </summary>
        public string DeclareName { get; set; }

        /// <summary>
        /// 录入人员IC卡号
        /// </summary>
        public string TypistNo { get; set; }


        /// <summary>
        /// 录入人
        /// </summary>
        public Admin Inputer { get; set; }

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
        /// 换汇状态
        /// </summary>
        public SwapStatus SwapStatus { get; set; }

        /// <summary>
        /// 是否两步申报
        /// 这个字段的作用就是为了给计算对账单时，报关总价取整还是取两位小数用的
        /// 20210127 之后所有的报关总价都取两位小数
        /// </summary>
        public bool isTwoStep
        {
            get
            {
                bool isTwo = false;
                DateTime dt20201126 = Convert.ToDateTime("2020-11-26");
                DateTime dt20210127 = Convert.ToDateTime("2021-01-27");

                Order order = new Orders2View().Where(t => t.ID == this.OrderID).FirstOrDefault();
                if (order.CreateDate >= dt20210127)
                {
                    isTwo = true;
                }
                else if (this.DDate != null)
                {
                    if (this.DDate > dt20201126 && this.DDate < dt20210127)
                    {
                        if (!string.IsNullOrEmpty(this.Type) && this.Type.Length >= 3)
                        {
                            if (this.Type.Substring(2, 1).Equals("3"))
                            {
                                isTwo = true;
                            }
                        }
                    }
                    else if (this.DDate >= dt20210127)
                    {
                        isTwo = true;
                    }
                }

                return isTwo;
            }
        }

        /// <summary>
        /// 发单人
        /// </summary>
        public string SubmitCustomAdminID { get; set; }

        /// <summary>
        /// 复核人
        /// </summary>
        public string DoubleCheckerAdminID { get; set; }

        /// <summary>
        /// 报关进口-做账凭证标记
        /// </summary>
        public bool MKImport { get; set; }

        /// <summary>
        /// 保函编号
        /// </summary>
        public string GuarateeNo { get; set; }

        #endregion

        /// <summary>
        /// 合同
        /// </summary>
        public Contract Contract
        {
            get
            {
                return new Contract(this);
            }
        }

        /// <summary>
        /// 发票
        /// </summary>
        public PaymentInstruction PaymentInstruction
        {
            get
            {
                return new PaymentInstruction(this);
            }
        }

        /// <summary>
        /// 装箱单
        /// </summary>
        public PackingList PackingList
        {
            get
            {
                return new PackingList(this);
            }
        }

        ///// <summary>
        ///// 运单
        ///// </summary>
        //public OutputWayBill OutputWayBill
        //{
        //    get
        //    {
        //        return new OutputWayBill(this);
        //    }
        //}

        private DecLists lists;

        /// <summary>
        /// 报关单表体
        /// </summary>
        public DecLists Lists
        {
            get
            {
                if (lists == null)
                {
                    using (var view = new Views.DecOriginListsView())
                    {
                        var query = view.Where(item => item.DeclarationID == this.ID).OrderBy(item => item.GNo);
                        this.Lists = new DecLists(query);
                    }
                }
                return this.lists;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.lists = new DecLists(value, new Action<DecList>(delegate (DecList item)
                {
                    item.DeclarationID = this.ID;
                }));
            }
        }

        /// <summary>
        /// 集装箱信息
        /// </summary>
        DecContainers containers;
        public DecContainers Containers
        {
            get
            {
                if (containers == null)
                {
                    using (var view = new Views.DecContainersView())
                    {
                        var query = view.Where(item => item.DeclarationID == this.ID);
                        this.Containers = new DecContainers(query);
                    }
                }
                return this.containers;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.containers = new DecContainers(value, new Action<DecContainer>(delegate (DecContainer item)
                {
                    item.DeclarationID = this.ID;
                }));
            }
        }

        /// <summary>
        /// 报关随附单证信息
        /// </summary>
        DecLicenseDocus licenseDocus;
        public DecLicenseDocus LicenseDocus
        {
            get
            {
                if (licenseDocus == null)
                {
                    using (var view = new Views.DecLicenseDocusView())
                    {
                        var query = view.Where(item => item.DeclarationID == this.ID);
                        this.LicenseDocus = new DecLicenseDocus(query);
                    }
                }
                return this.licenseDocus;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                this.licenseDocus = new DecLicenseDocus(value, new Action<DecLicenseDocu>(delegate (DecLicenseDocu item)
                 {
                     item.DeclarationID = this.ID;
                 }));
            }
        }

        /// <summary>
        /// 报关申请单证信息
        /// </summary>
        DecRequestCerts requestCerts;
        public DecRequestCerts RequestCerts
        {
            get
            {
                if (requestCerts == null)
                {
                    using (var view = new Views.DecRequestCertsView())
                    {
                        var query = view.Where(item => item.DeclarationID == this.ID);
                        this.RequestCerts = new DecRequestCerts(query);
                    }
                }
                return this.requestCerts;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                this.requestCerts = new DecRequestCerts(value, new Action<DecRequestCert>(delegate (DecRequestCert item)
                {
                    item.DeclarationID = this.ID;
                }));
            }
        }

        /// <summary>
        /// 报关单其它包装信息
        /// </summary>
        DecOtherPacks otherPacks;
        public DecOtherPacks OtherPacks
        {
            get
            {
                if (otherPacks == null)
                {
                    using (var view = new Views.DecOtherPacksView())
                    {
                        var query = view.Where(item => item.DeclarationID == this.ID);
                        this.OtherPacks = new DecOtherPacks(query);
                    }
                }
                return this.otherPacks;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                this.otherPacks = new DecOtherPacks(value, new Action<DecOtherPack>(delegate (DecOtherPack item)
                {
                    item.DeclarationID = this.ID;
                }));
            }
        }

        /// <summary>
        /// 电子随附单据关联关系
        /// </summary>
        EdocRealations edocRealations;
        public EdocRealations EdocRealations
        {
            get
            {
                if (edocRealations == null)
                {
                    using (var view = new Views.EdocRealationsView())
                    {
                        var query = view.Where(item => item.DeclarationID == this.ID);
                        this.EdocRealations = new EdocRealations(query);
                    }
                }
                return this.edocRealations;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                this.edocRealations = new EdocRealations(value, new Action<EdocRealation>(delegate (EdocRealation item)
                {
                    item.DeclarationID = this.ID;
                }));
            }
        }

        /// <summary>
        /// 报关单轨迹/回执
        /// </summary>
        DecTraces traces;
        public DecTraces Traces
        {
            get
            {
                if (traces == null)
                {
                    using (var view = new Views.DecTracesView())
                    {
                        var query = view.Where(item => item.DeclarationID == this.ID);
                        this.traces = new DecTraces(query);
                    }
                }
                return this.traces;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                this.traces = new DecTraces(value, new Action<DecTrace>(delegate (DecTrace item)
                {
                    item.DeclarationID = this.ID;
                }));
            }
        }

        #region Event

        /// <summary>
        /// 报关单创建时发生
        /// </summary>
        public event DeclareCreatedHanlder DeclareCreated;

        /// <summary>
        /// 报关单取消时发生
        /// </summary>
        public event DeclareCancelHandler DeclareCanceled;

        /// <summary>
        /// 报关单编辑时发生
        /// </summary>
        public event DeclareEditedHandler DeclareEdited;

        /// <summary>
        /// 报关单生成发票时发生
        /// </summary>
        public event PaymentInstructionSavedHanlder PaymentInstructionSaved;

        /// <summary>
        /// 报关单生成合同时发生
        /// </summary>
        public event ContractSavedHanlder ContractSaved;

        /// <summary>
        /// 报关单生成装箱单时发生
        /// </summary>
        public event PackingListSavedHanlder PackingListSaved;

        /// <summary>
        /// 报关成功时发生
        /// </summary>
        public event DeclareSucceedHanlder DeclareSucceed;

        /// <summary>
        /// 报关制单成功
        /// </summary>
        public event DeclareMakedHanlder DeclareMaked;

        /// <summary>
        /// 报关申报（报文准备就绪）
        /// </summary>
        public event DeclareApplyHanlder DeclareApply;
        /// <summary>
        /// Excel申报
        /// </summary>
        public event DeclareApplyHanlder DeclareExcel;
        /// <summary>
        /// Excel申报成功
        /// </summary>
        public event DeclareApplyHanlder DeclareExcelDone;
        /// <summary>
        /// 商品项更改
        /// </summary>
        public event DeclareApplyHanlder DeclareGoodsChanged;


        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        #endregion

        public DecHead()
        {
            this.CusDecStatus = MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.Draft);//草稿

            this.SwapStatus = SwapStatus.UnAuditing;//未换汇

            //报关单创建时发生
            this.DeclareCreated += DecHead_DeclareCreated;

            //发票生成成功时发生
            this.PaymentInstructionSaved += DecHead_PaymentInstructionSaved;
            //合同生成成功时发生
            this.ContractSaved += DecHead_ContractSaved;
            //装箱单生成成功时发生
            this.PackingListSaved += DecHead_PackingListSaved;

            //报关制单成功
            this.DeclareMaked += DecHead_DeclareMaked;
            //报关申报（报文准备就绪）
            this.DeclareApply += DecHead_DeclareApply;
            //报关成功时发生
            this.DeclareSucceed += DecHead_DeclareSucceed;
            //报关取消时发生
            this.DeclareCanceled += DecHead_Canceled;
            //生成报关Excel时发生
            this.DeclareExcel += DecHead_ExcelDeclared;
            //更新海关号码时发生
            this.DeclareExcelDone += DecHead_ExcelDeclareDone;
            //商品项更改时发生
            this.DeclareGoodsChanged += DecHead_DeclareGoodsChanged;
            //报关编辑时发生
            this.DeclareEdited += DecHead_DeclareEdited;
        }

        /// <summary>
        /// 合同协议号
        /// </summary>
        private string CreateContractNo()
        {
            //这个订单之前是否有取消的报关单，延用之前的合同号
            var headbefore = new Ccs.Services.Views.DecHeadsView().Where(item => item.OrderID == this.OrderID).FirstOrDefault();
            if (headbefore != null)
            {
                this.ContrNo = headbefore.ContrNo;
            }
            else
            {
                string today = DateTime.Now.ToString("yyyy-MM-dd");
                DateTime dtStart = Convert.ToDateTime(today + " 00:00:00");
                DateTime dtEnd = Convert.ToDateTime(today + " 23:59:59");

                string orderIndex = "";
                using (var view = new Views.DecHeadsView())
                {
                    int count = view.Where(item => item.CreateTime >= dtStart && item.CreateTime <= dtEnd).OrderByDescending(item => item.CreateTime).Count();
                    orderIndex = (count + 1).ToString();
                    orderIndex = orderIndex.PadLeft(3, '0');
                }

                this.ContrNo = PurchaserContext.Current.ContractNoPrefix + DateTime.Now.ToString("yyyyMM-dd") + "-" + orderIndex;
            }


            return this.ContrNo;
        }

        /// <summary>
        /// 提运单号
        /// </summary>
        /// <returns></returns>
        private string CreateBillNo()
        {
            //var ht = PurchaserContext.Current.ContractNoPrefix.Length;
            //int ContractNo = Convert.ToInt16(this.ContrNo.Substring(10 + ht, 3));
            //string BillNo = PurchaserContext.Current.BillNoPrefix + DateTime.Now.ToString("MMdd") + ContractNo.ToString().PadLeft(3, '0');

            var last = this.ContrNo.Substring(this.ContrNo.Length - 3);
            string BillNo = PurchaserContext.Current.BillNoPrefix + DateTime.Now.ToString("yyyyMMdd") + last;

            return BillNo;
        }

        private string CreateVoyNo()
        {
            string VoyNo = "";
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            DateTime dtStart = Convert.ToDateTime(today + " 00:00:00");
            DateTime dtEnd = Convert.ToDateTime(today + " 23:59:59");

            using (var view = new Views.DecHeadsView())
            {
                var head = view.Where(item => item.CreateTime >= dtStart && item.CreateTime <= dtEnd).FirstOrDefault();
                if (head != null)
                {
                    VoyNo = head.VoyNo;
                    this.VoyNo = VoyNo;
                }

            }

            return this.voyNo;
        }

        #region Enter
        //编辑报关单时使用
        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>().Count(item => item.ID == this.ID);

                    if (count == 0)
                    {
                        //  this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.DecHead);

                        this.ID = string.Concat(PurchaserContext.Current.DecHeadIDPrefix, Needs.Overall.PKeySigner.Pick(PKeyType.DecHead));
                        reponsitory.Insert(this.ToLinq());
                    }
                    else
                    {
                        reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);

                        if (this.OtherPacks != null)
                        {
                            //修改其他包装,先删除，再新增
                            reponsitory.Delete<Layer.Data.Sqls.ScCustoms.DecOtherPacks>(item => item.DeclarationID == this.ID);
                            for (int i = 0; i < this.OtherPacks.Count; i++)
                            {
                                reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.DecOtherPacks
                                {
                                    ID = ChainsGuid.NewGuidUp(),
                                    DeclarationID = this.ID,
                                    PackQty = this.OtherPacks[i].PackQty,
                                    PackType = this.OtherPacks[i].PackType
                                });
                            }
                        }
                    }
                }

                this.OnEnter();
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// enter后发生
        /// </summary>
        virtual protected void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        #endregion

        /// <summary>
        /// 报关单表头改变
        /// </summary>
        public void DeclareHeadChange()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);

                if (this.OtherPacks != null)
                {
                    //修改其他包装,先删除，再新增
                    reponsitory.Delete<Layer.Data.Sqls.ScCustoms.DecOtherPacks>(item => item.DeclarationID == this.ID);
                    for (int i = 0; i < this.OtherPacks.Count; i++)
                    {
                        reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.DecOtherPacks
                        {
                            ID = ChainsGuid.NewGuidUp(),
                            DeclarationID = this.ID,
                            PackQty = this.OtherPacks[i].PackQty,
                            PackType = this.OtherPacks[i].PackType,
                        });
                    }
                }
            }
            this.OnEnter();
            this.OnEdited(new DeclareCreatedEventArgs(this));
        }

        public virtual void OnEdited(DeclareCreatedEventArgs args)
        {
            this.DeclareEdited?.Invoke(this, args);
        }

        private void DecHead_DeclareEdited(object sender, DeclareCreatedEventArgs e)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //插入 DecHeadSpecialType 数据
                GenerateDecHeadSpecialType(reponsitory, e.DecHead);
            }
        }

        /// <summary>
        /// 将删除
        /// 报文回执(报关单状态更新)
        /// </summary>
        /// <param name="status"></param>
        public void DeclareRecepit(string status)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { CusDecStatus = status }, item => item.ID == this.ID);
            }

            if (status == "7")//枚举值，可读的
            {
                // using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                //{
                //    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { CusDecStatus = status }, item => item.ID == this.ID);
                //}

                //this.OnDeclareSucceed(new DeclareSucceedEventArgs(this));
            }

            //根据状态做相应的事情
            this.OnEnter();
        }

        #region 新增报关单

        /// <summary>
        /// 新增报关单时使用
        /// </summary>
        public void CreateDeclare()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    try
                    {
                        Needs.Ccs.Services.Models.DecHeadLock.CurrentDecHeadLock.DecContrNoLock.EnterWriteLock();
                        CreateContractNo();
                        if (this.IsInspection || (this.IsQuarantine == null ? false : this.IsQuarantine.Value))
                        {
                            this.ContrNo = this.ContrNo.Replace(PurchaserContext.Current.ContractNoPrefix, PurchaserContext.Current.SJContractNoPrefix);
                            string billNo = this.BillNo.Replace(PurchaserContext.Current.BillNoPrefix, PurchaserContext.Current.SJBillNoPrefix);
                            this.billNo = billNo;
                        }
                        else
                        {
                            //防止再次制单，默认使用了商检的合同号
                            this.ContrNo = this.ContrNo.Replace(PurchaserContext.Current.SJContractNoPrefix, PurchaserContext.Current.ContractNoPrefix);
                        }
                        reponsitory.Insert(this.ToLinq());
                    }
                    finally
                    {
                        Needs.Ccs.Services.Models.DecHeadLock.CurrentDecHeadLock.DecContrNoLock.ExitWriteLock();
                    }

                    foreach (var list in this.Lists)
                    {
                        reponsitory.Insert(list.ToLinq());
                    }

                    foreach (var pack in this.OtherPacks)
                    {
                        reponsitory.Insert(pack.ToLinq());
                    }
                }

                this.OnCreated(new DeclareCreatedEventArgs(this));
            }
            catch (Exception ex)
            {

            }
        }

        public virtual void OnCreated(DeclareCreatedEventArgs args)
        {
            this.DeclareCreated?.Invoke(this, args);
        }

        private void DecHead_DeclareCreated(object sender, DeclareCreatedEventArgs e)
        {
            // 修改报关通知以及报关通知项的状态
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //更改报关通知项的状态
                var NoticeIDs = e.DecHead.lists.Select(item => item.DeclarationNoticeItemID).ToList();
                NoticeIDs.ForEach(item =>
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DeclarationNoticeItems>(new { Status = (int)Enums.DeclareNoticeItemStatus.Make }, notice => notice.ID == item);
                });
                //更改报关通知的状态               
                var DeclareNotice = new Views.DeclarationNoticesView(reponsitory);
                var DeclareNoticeView = DeclareNotice.Where(item => item.ID == e.DecHead.DeclarationNoticeID).FirstOrDefault();
                if (DeclareNoticeView != null)
                {
                    //int unMakeCount = DeclareNoticeView.Items.Where(item => item.Status == (int)Enums.DeclareNoticeItemStatus.UnMake).Count();
                    //if (unMakeCount > 0)
                    //{
                    //    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DeclarationNotices>(new { Status = (int)Enums.DeclareNoticeStatus.PartDec }, notice => notice.ID == e.DecHead.DeclarationNoticeID);
                    //}
                    //else
                    //{
                    //    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DeclarationNotices>(new { Status = (int)Enums.DeclareNoticeStatus.AllDec }, notice => notice.ID == e.DecHead.DeclarationNoticeID);
                    //}

                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DeclarationNotices>(new { Status = (int)Enums.DeclareNoticeStatus.AllDec }, notice => notice.ID == e.DecHead.DeclarationNoticeID);
                }

                //插入 DecHeadSpecialType 数据
                GenerateDecHeadSpecialType(reponsitory, e.DecHead);
            }

            //var vendor = new VendorContext(VendorContextInitParam.DecHeadID, e.DecHead.ID).Current1;
            var vendor = new Needs.Ccs.Services.VendorContext(Needs.Ccs.Services.VendorContextInitParam.Pointed, this.OwnerName).Current1;

            //生成发票、合同、箱单
            e.DecHead.PaymentInstructionSaveAs();
            e.DecHead.ContractSaveAs();
            e.DecHead.PackingListSaveAs(vendor);

            //写入日志与轨迹
            e.DecHead.Trace("报关单新增");


            //非外单自动生成对账单PDF并上传
            Task.Run(() =>
            {
                var order = new Views.OrdersView()[e.DecHead.OrderID];
                var ermAdmin = new Needs.Ccs.Services.Views.AdminsTopView2().FirstOrDefault(x => x.OriginID == order.AdminID);
                if (order.Type != OrderType.Outside)
                {
                    var Orders = new Views.Orders2View().Where(item => item.MainOrderID == order.MainOrderID
                                                                 && item.OrderStatus != OrderStatus.Canceled
                                                                 && item.OrderStatus != OrderStatus.Returned
                                                                 && item.Status == Status.Normal).ToList();
                    var orderIds = Orders.Select(t => t.ID).ToList();
                    var decheads = new Views.DecHeadsView().Where(item => orderIds.Contains(item.OrderID));
                    //如果这个主订单下的所有的子订单都已经制单了，才自动上传对账单，委托书
                    if (orderIds.Count() == decheads.Count())
                    {
                        #region 自动上传对账单
                        var bill = getModel(order.MainOrderID);

                        bill.ProductsForIcgoo = new List<MainOrderBillItemProduct>();
                        bill.PartProductsForIcgoo = new List<MainOrderBillItemProduct>();
                        foreach (var t in bill.Bills)
                        {
                            bill.ProductsForIcgoo.AddRange(t.Products);
                            bill.PartProductsForIcgoo.AddRange(t.PartProducts);
                        }

                        //保存文件
                        string fileName = DateTime.Now.Ticks + ".pdf";
                        FileDirectory fileDic = new FileDirectory(fileName);
                        fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                        fileDic.CreateDataDirectory();

                        //更换 itextsharp插件
                        var orderbill = new OrderBillToPdf(bill);
                        orderbill.SaveAs(fileDic.FilePath);
                        //bill.SaveASIcgoo(fileDic.FilePath);

                        //先删除之前上传的委托书
                        var origFiles = order.MainOrderFiles.Where(f => f.FileType == FileType.OrderBill && f.Status == Status.Normal);
                        foreach (var aorigFile in origFiles)
                        {
                            Needs.Ccs.Services.Models.MainOrderFile orderBill = new Needs.Ccs.Services.Models.MainOrderFile();
                            orderBill.ID = aorigFile.ID;
                            orderBill.Abandon();
                            new CenterFilesTopView().Modify(new { Status = FileDescriptionStatus.Delete }, aorigFile.ID);
                        }

                        #region 本地文件同步中心文件库

                        var ErmAdminID = ermAdmin?.ID ?? "";
                        var dic = new { CustomName = fileName, WsOrderID = order.MainOrderID, AdminID = ErmAdminID };

                        var centerType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.OrderBill;
                        //本地文件上传到服务器
                        var tempPath = fileDic.FilePath;
                        var result = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(tempPath, centerType, dic);
                        string[] ID = { result[0].FileID };
                        new CenterFilesTopView().Modify(new { Status = FileDescriptionStatus.Approved }, ID);
                        #endregion

                        #endregion

                        #region 自动上传委托书                      
                        var agentProxy = new Needs.Ccs.Services.Views.MainOrderAgentProxiesView().Where(t => t.Order.ID == Orders.FirstOrDefault().ID).FirstOrDefault();
                        //保存文件
                        string afileName = DateTime.Now.Ticks + ".pdf";
                        FileDirectory afileDic = new FileDirectory(afileName);
                        afileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                        afileDic.CreateDataDirectory();
                        if (agentProxy.Client.ClientType == ClientType.External)
                        {
                            AgentProxyToPdf model = new AgentProxyToPdf();
                            model.Orders = new List<Needs.Ccs.Services.Models.Order>();
                            model.Orders = Orders;
                            var orderagent = Orders.FirstOrDefault();
                            model.ID = orderagent.MainOrderID;
                            model.Client = agentProxy.Client;
                            model.PackNo = Orders.Sum(t => t.PackNo);
                            model.WarpType = orderagent.WarpType;
                            model.Currency = orderagent.Currency;
                            model.SaveAs(afileDic.FilePath);
                        }
                        else
                        {
                            //itextsharp生成，超过10页
                            AgentProxyToPdf model = new AgentProxyToPdf();
                            model.Orders = new List<Needs.Ccs.Services.Models.Order>();
                            model.Orders = Orders;
                            var order1 = Orders.FirstOrDefault();
                            model.ID = order1.MainOrderID;
                            model.Client = agentProxy.Client;
                            model.PackNo = Orders.Sum(t => t.PackNo);
                            model.WarpType = order1.WarpType;
                            model.Currency = order1.Currency;
                            model.SaveAs(afileDic.FilePath);
                        }


                        //先删除之前上传的委托书
                        var aorigFiles = order.MainOrderFiles.Where(f => f.FileType == FileType.AgentTrustInstrument && f.Status == Status.Normal);
                        foreach (var aorigFile in aorigFiles)
                        {
                            Needs.Ccs.Services.Models.MainOrderFile orderBill = new Needs.Ccs.Services.Models.MainOrderFile();
                            orderBill.ID = aorigFile.ID;
                            orderBill.Abandon();
                            new CenterFilesTopView().Modify(new { Status = FileDescriptionStatus.Delete }, aorigFile.ID);
                        }

                        var dicAgent = new { CustomName = afileName, WsOrderID = order.MainOrderID, AdminID = ErmAdminID };

                        var centerTypeAgent = Needs.Ccs.Services.Models.ApiModels.Files.FileType.AgentTrustInstrument;
                        //本地文件上传到服务器
                        var atempPath = afileDic.FilePath;
                        var resultAgent = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(atempPath, centerTypeAgent, dicAgent);
                        string[] AgentID = { resultAgent[0].FileID };
                        new CenterFilesTopView().Modify(new { Status = FileDescriptionStatus.Approved }, AgentID);
                        #endregion
                    }

                }
            });

        }

        #endregion

        /// <summary>
        /// 插入 DecHeadSpecialType 数据
        /// </summary>
        /// <param name="reponsitory"></param>
        /// <param name="decNoticeID"></param>
        private void GenerateDecHeadSpecialType(Layer.Data.Sqls.ScCustomsReponsitory reponsitory, DecHead decHead)
        {
            Dictionary<Enums.OrderSpecialType, Enums.DecHeadSpecialTypeEnum> _dicSpecialType = new Dictionary<Enums.OrderSpecialType, Enums.DecHeadSpecialTypeEnum>();
            _dicSpecialType.Add(Enums.OrderSpecialType.HighValue, Enums.DecHeadSpecialTypeEnum.HighValue);
            _dicSpecialType.Add(Enums.OrderSpecialType.Inspection, Enums.DecHeadSpecialTypeEnum.Inspection);
            _dicSpecialType.Add(Enums.OrderSpecialType.Quarantine, Enums.DecHeadSpecialTypeEnum.Quarantine);
            _dicSpecialType.Add(Enums.OrderSpecialType.CCC, Enums.DecHeadSpecialTypeEnum.CCC);

            Views.DecHeadSpecialTypesView decHeadSpecialTypesView = new Views.DecHeadSpecialTypesView(reponsitory);
            var listOrderVoyageModel = decHeadSpecialTypesView.GetOrderVoyageModel(decHead.DeclarationNoticeID).ToList();
            var decNoticeVoyageModel = decHeadSpecialTypesView.GetDecNoticeVoyageModel(decHead.DeclarationNoticeID).FirstOrDefault();

            List<Models.DecHeadSpecialType> listDecHeadSpecialType = new List<DecHeadSpecialType>();
            if (listOrderVoyageModel != null && listOrderVoyageModel.Any())
            {
                foreach (var orderVoyageModel in listOrderVoyageModel)
                {
                    if (_dicSpecialType.Keys.Contains(orderVoyageModel.Type))
                    {
                        if ((orderVoyageModel.Type == OrderSpecialType.Quarantine && decHead.IsQuarantine.Value) || orderVoyageModel.Type != OrderSpecialType.Quarantine)
                        {
                            listDecHeadSpecialType.Add(new Models.DecHeadSpecialType()
                            {
                                DecHeadID = decHead.ID,
                                Type = _dicSpecialType[orderVoyageModel.Type],
                                CreateDate = DateTime.Now,
                                UpdateDate = DateTime.Now,
                                Status = Enums.Status.Normal,
                            });
                        }
                    }
                }
            }

            if (decNoticeVoyageModel != null && decNoticeVoyageModel.Type == Enums.VoyageType.CharterBus)
            {
                listDecHeadSpecialType.Add(new Models.DecHeadSpecialType()
                {
                    DecHeadID = decHead.ID,
                    Type = Enums.DecHeadSpecialTypeEnum.CharterBus,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Status = Enums.Status.Normal,
                });
            }

            //原产地加征
            var dateNow = DateTime.Now.Date;//当前日期
            var originRate = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OriginsATRateTopView>().ToArray();
            foreach (var list in decHead.Lists)
            {
                if (originRate.Any(t => t.TariffID == list.CodeTS && t.Origin == list.OriginCountry && t.StartDate <= dateNow && (t.EndDate > dateNow || t.EndDate == null)))
                {
                    listDecHeadSpecialType.Add(new Models.DecHeadSpecialType()
                    {
                        DecHeadID = decHead.ID,
                        Type = Enums.DecHeadSpecialTypeEnum.OriginATRate,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Status = Enums.Status.Normal,
                    });
                    break;
                }
            }

            //敏感产地
            var SenOriginStr = System.Configuration.ConfigurationManager.AppSettings["SenOrigin"];
            var SenList = SenOriginStr.Split('|');
            foreach (var list in decHead.Lists)
            {
                //判断是否敏感产地
                if (SenList.Contains(list.OriginCountry))
                {
                    //有敏感产地，将订单加入特殊类型Array
                    listDecHeadSpecialType.Add(new Models.DecHeadSpecialType()
                    {
                        DecHeadID = decHead.ID,
                        Type = Enums.DecHeadSpecialTypeEnum.SenOrigin,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Status = Enums.Status.Normal,
                    });
                    break;
                }
            }

            foreach (var decHeadSpecialType in listDecHeadSpecialType)
            {
                decHeadSpecialType.OnewayInsert(reponsitory);
            }
        }

        #region 制单
        /// <summary>
        /// 制单完成
        /// </summary>
        public void Make()
        {
            var message = new DecMessage(this);
            //带路径的文件名称，保存后的 :Decmessage/201902/13/o0100000.xml
            string fileName = message.SaveAs(this.ID + ".xml");

            //更新报关单状态
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                this.CusDecStatus = MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.Make);
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { this.CusDecStatus, this.Type, MarkingUrl = fileName }, item => item.ID == this.ID);
            }

            this.OnMaked(new DeclareMakedEventArgs(this, fileName));
        }

        public virtual void OnMaked(DeclareMakedEventArgs args)
        {
            this.DeclareMaked?.Invoke(this, args);
        }

        private void DecHead_DeclareMaked(object sender, DeclareMakedEventArgs e)
        {
            //写入日志与轨迹
            e.DecHead.Trace("制单，生成报文");

            var orderID = e.DecHead.OrderID;
            var order = new Views.OrdersView()[orderID];
            order?.Trace(OrderTraceStep.Declaring, "您的订单已申报,请您耐心等待");
        }
        #endregion

        #region 报关单商品项更改
        public void GoodsChange()
        {
            //更新报关单状态
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                this.CusDecStatus = MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.Draft);
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { this.CusDecStatus }, item => item.ID == this.ID);
            }
            this.OnGoodsChanged(new DeclareApplyEventArgs(this));
        }

        public virtual void OnGoodsChanged(DeclareApplyEventArgs args)
        {
            this.DeclareGoodsChanged?.Invoke(this, args);
        }
        private void DecHead_DeclareGoodsChanged(object sender, DeclareApplyEventArgs e)
        {
            //写入日志与轨迹
            e.DecHead.Trace("制单，商品项更改");
        }
        #endregion

        #region Excel申报
        public void ExcelMake()
        {
            ////更新报关单状态
            //using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            //{
            //    this.CusDecStatus = MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.E0);
            //    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { this.CusDecStatus }, item => item.ID == this.ID);
            //}
            this.OnExcel(new DeclareApplyEventArgs(this));
        }

        public virtual void OnExcel(DeclareApplyEventArgs e)
        {
            this.DeclareExcel?.Invoke(this, e);
        }

        private void DecHead_ExcelDeclared(object sender, DeclareApplyEventArgs e)
        {
            e.DecHead.Trace("制单，生成Excel报关");
        }
        #endregion

        /// <summary>
        /// 设置发单人
        /// </summary>
        /// <param name="customSubmiterAdminID"></param>
        public void SetCustomSubmiter(string customSubmiterAdminID)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {

                var linq_SubmitCustomAdmin = from admin in new AdminsTopView2(reponsitory)
                                             where admin.OriginID == customSubmiterAdminID
                                             select admin;

                var SubmitCustomAdminName = linq_SubmitCustomAdmin.FirstOrDefault().RealName;

                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new
                {
                    SubmitCustomAdminID = customSubmiterAdminID,
                    DeclareName = SubmitCustomAdminName
                },
                item => item.ID == this.ID);
            }
        }


        #region 更新海关编号
        public void ExcelDeclareDone()
        {
            //更新报关单状态,以及海关编号
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                this.CusDecStatus = MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.E1);
                this.DDate = DateTime.Now;
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { this.CusDecStatus, this.SeqNo, this.EntryId, this.DDate }, item => item.ID == this.ID);
            }
            this.OnExcelDeclareDone(new DeclareApplyEventArgs(this));
        }

        public virtual void OnExcelDeclareDone(DeclareApplyEventArgs e)
        {
            this.DeclareExcelDone?.Invoke(this, e);
        }

        private void DecHead_ExcelDeclareDone(object sender, DeclareApplyEventArgs e)
        {
            e.DecHead.Trace("完成，更新海关号码");
        }

        #endregion

        #region 申报

        /// <summary>
        /// 申报（报文准备就绪,.zip文件下载完成）(废弃)
        /// </summary>
        public void Declare()
        {
            string fileName = this.ID + ".zip";

            //创建文件夹
            //FileDirectory file = new FileDirectory();
            //file.SetChildFolder(SysConfig.ZipFiles);
            //file.CreateDataDirectory();

            ////string zipPath = AppContext.Current.FileDirectory + @"\" + AppContext.Current.CreateDataFileDirectory(SysConfig.ZipFiles) + @"\";
            ////TODO:根据报文类型对应不同文件夹
            //string targetDirectory = SysConfig.DeclareMessageUrl;

            //List<string> files = new List<string>();
            ////原则：谁使用谁拼接
            //foreach (var doc in this.EdocRealations)
            //{
            //    files.Add(file.RootDirectory + @"\" + doc.FileUrl);
            //}

            //files.Add(file.RootDirectory + @"\" + this.MarkingUrl);

            //ZipFile zip = new ZipFile(fileName);
            //zip.SetFilePath(file.FilePath);
            //zip.Files = files;
            //zip.ZipFiles();

            ////下载文件
            //RWFile rw = new RWFile();
            //string SourcePath = file.FilePath + fileName;
            //string TargetPath = targetDirectory + fileName;
            //rw.SourcePath = SourcePath;
            //rw.TargetPath = TargetPath;
            //rw.TargetDirectory = targetDirectory;
            //rw.ReadAndWriteFile();

            //更新报关单状态
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                this.CusDecStatus = MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.Declare);
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { this.CusDecStatus }, item => item.ID == this.ID);
            }

            this.OnApply(new DeclareApplyEventArgs(this));
        }

        public void ClientDeclare()
        {
            string fileName = this.ID + ".zip.temp";

            //创建文件夹
            FileDirectory file = new FileDirectory();
            System.Net.WebClient wbClient = new System.Net.WebClient();
            List<string> files = new List<string>();
            var clientPath = string.Empty;

            var DeclareMessageRootPath = System.Configuration.ConfigurationManager.AppSettings["DeclareMessageRootPath"];

            //原则：谁使用谁拼接
            foreach (var doc in this.EdocRealations)
            {
                clientPath = DeclareMessageRootPath + SysConfig.DeclareDirectory + @"\" + Path.GetFileName(doc.FileUrl);
                files.Add(clientPath);
                wbClient.DownloadFile(file.FileServerUrl + @"\" + doc.FileUrl, clientPath);
            }

            clientPath = DeclareMessageRootPath + SysConfig.DecMessageDirectory + @"\" + this.ID + ".xml";
            files.Add(clientPath);
            wbClient.DownloadFile(file.FileServerUrl + @"\" + this.MarkingUrl, clientPath);


            //压缩文件
            ZipFile zip = new ZipFile(fileName);
            zip.Files = files;
            zip.ZipedPath = DeclareMessageRootPath + SysConfig.MeaasgeFolder + @"\";
            zip.ZipFiles();

            //删除已被压缩的源文件
            files.ForEach(t =>
            {
                File.Delete(t);
            });

            //.temp文件重命名
            File.Move(zip.ZipedPath + fileName, zip.ZipedPath + this.ID + ".zip");

            //更新报关单状态
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                this.CusDecStatus = MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.Declare);
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { this.CusDecStatus }, item => item.ID == this.ID);
            }

            this.OnApply(new DeclareApplyEventArgs(this));
        }

        public virtual void OnApply(DeclareApplyEventArgs args)
        {
            this.DeclareApply?.Invoke(this, args);
        }

        private void DecHead_DeclareApply(object sender, DeclareApplyEventArgs e)
        {
            //写入日志与轨迹
            e.DecHead.Trace("导出报文.zip至文件夹，等待发送或自动发送至海关");
        }

        #endregion

        #region 复核
        /// <summary>
        /// 制单人复核
        /// </summary>
        public void MakerDoubleCheck(string adminID)
        {
            //更新报关单状态
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                this.CusDecStatus = MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.PendingDoubleCheck);
                var dechead = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>().Where(t => t.ID == this.ID).FirstOrDefault();
                if (dechead != null)
                {
                    if (string.IsNullOrEmpty(dechead.DoubleCheckerAdminID))
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { CusDecStatus = this.CusDecStatus, DoubleCheckerAdminID = adminID }, item => item.ID == this.ID);
                    }
                    else
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { CusDecStatus = this.CusDecStatus }, item => item.ID == this.ID);
                    }
                }
            }
        }

        /// <summary>
        /// 复核人复核
        /// </summary>
        public void CheckerDoubleCheck()
        {
            //更新报关单状态
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                this.CusDecStatus = MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.DoubleChecked);
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { this.CusDecStatus }, item => item.ID == this.ID);
            }
        }

        public void CheckerDoubleRefuse()
        {
            //更新报关单状态
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                this.CusDecStatus = MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.Draft);
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { this.CusDecStatus }, item => item.ID == this.ID);
            }
        }

        #endregion

        /// <summary>
        /// 舱单生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ToManifest()
        {
            var manifestConsignment = new ManifestConsignment(this);
            manifestConsignment.CreateManifestConsignment();
        }

        #region 报关成功

        /// <summary>
        /// 报关成功
        /// </summary>
        public void DeclareSucceess()
        {
            var order = new Views.QuoteConfirmedOrdersView().Where(item => item.ID == this.OrderID).FirstOrDefault();
            if (order != null)
            {
                order.SetAdmin(this.Inputer);
                order.DeclareSuccess();
            }
            //更新成功状态
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { IsSuccess = true }, item => item.ID == this.ID);
            }
            this.OnDeclareSucceed(new DeclareSucceedEventArgs(this, order));
        }

        /// <summary>
        /// 报关成功触发事件
        /// </summary>
        /// <param name="args"></param>
        public virtual void OnDeclareSucceed(DeclareSucceedEventArgs args)
        {
            this.DeclareSucceed?.Invoke(this, args);
        }

        /// <summary>
        /// 报关成功后执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecHead_DeclareSucceed(object sender, DeclareSucceedEventArgs e)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {

                //生成香港出库通知
                //if (reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNotices>().Count(item => item.OrderID == e.DecHead.OrderID && item.WarehouseType == (int)WarehouseType.HongKong) == 0)
                //{
                //    var exitNotice = new Layer.Data.Sqls.ScCustoms.ExitNotices();
                //    exitNotice.ID = Needs.Overall.PKeySigner.Pick(PKeyType.ExitNotice);
                //    exitNotice.OrderID = e.DecHead.OrderID;
                //    exitNotice.AdminID = e.DecHead.Inputer.ID;
                //    exitNotice.DecHeadID = e.DecHead.ID;
                //    exitNotice.WarehouseType = (int)WarehouseType.HongKong;
                //    exitNotice.ExitNoticeStatus = (int)ExitNoticeStatus.UnExited;
                //    exitNotice.Status = (int)Status.Normal;
                //    exitNotice.CreateDate = DateTime.Now;
                //    exitNotice.UpdateDate = DateTime.Now;
                //    reponsitory.Insert(exitNotice);

                //    foreach (var list in e.DecHead.Lists)
                //    {
                //        var itemID = Needs.Overall.PKeySigner.Pick(PKeyType.ExitNoticeItem);
                //        reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ExitNoticeItems
                //        {
                //            ID = itemID,
                //            ExitNoticeID = exitNotice.ID,
                //            DecListID = list.ID,
                //            Quantity = list.GQty,
                //            ExitNoticeStatus = (int)ExitNoticeStatus.UnExited,
                //            Status = (int)Status.Normal,
                //            CreateDate = DateTime.Now,
                //            UpdateDate = DateTime.Now
                //        });
                //    }
                //}

                //写入海关汇率             
                var currency = e.DecHead.Lists.FirstOrDefault().TradeCurr;
                var Rate = new Views.CustomExchangeRatesView(currency).ToRate();
                if (Rate != null)
                {
                    e.DecHead.CustomsExchangeRate = Rate.Rate;
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { e.DecHead.CustomsExchangeRate }, item => item.ID == e.DecHead.ID);
                }

                //写入财务报关单数据
                var decTax = new Layer.Data.Sqls.ScCustoms.DecTaxs();
                decTax.ID = e.DecHead.ID;
                decTax.InvoiceType = (int)e.Order.ClientAgreement.InvoiceType;
                decTax.Status = (int)DecTaxStatus.Unpaid;
                decTax.CreateDate = DateTime.Now;
                decTax.UpdateDate = decTax.CreateDate;
                reponsitory.Insert(decTax);

                //【融合】调用接口，生成香港出库通知--表格申报
                var exit = new GenerateExitNotice(this);
                exit.Excute();

                System.Threading.Tasks.Task.Run(() =>
                {
                    PushMsg pushMsg = new PushMsg((int)SpotName.Declare, e.DecHead.OrderID);
                    pushMsg.push();
                });
            }
        }

        /// <summary>
        /// 荣检的报关工具失败后，只调用库房的方法
        /// </summary>
        public void Only2WareHouse()
        {
            var exit = new GenerateExitNotice(this);
            exit.Excute();
        }

        #endregion

        #region 发票

        /// <summary>
        /// 生成电子单据-发票
        /// </summary>
        public void PaymentInstructionSaveAs()
        {
            string decHeadID = this.ID;
            //var vendor = new VendorContext(VendorContextInitParam.DecHeadID, decHeadID).Current1;
            //this.OwnerName = "北京创新在线电子产品销售有限公司杭州分公司";
            var vendor = new Needs.Ccs.Services.VendorContext(Needs.Ccs.Services.VendorContextInitParam.Pointed, this.OwnerName).Current1;

            var result = this.PaymentInstruction.SaveAs(vendor);

            this.OnPaymentInstructionSaved(new PaymentInstructionSavedEventArgs(this, result[0], result[1], result[2]));
        }

        /// <summary>
        /// 生成电子单据-发票调用事件
        /// </summary>
        /// <param name="args"></param>
        public virtual void OnPaymentInstructionSaved(PaymentInstructionSavedEventArgs args)
        {
            this.PaymentInstructionSaved?.Invoke(this, args);
        }

        /// <summary>
        /// 生成电子单据-发票成功后执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecHead_PaymentInstructionSaved(object sender, PaymentInstructionSavedEventArgs e)
        {
            var serino = Needs.Overall.PKeySigner.Pick(PKeyType.EdocInvoice);
            var edoc = new EdocRealation()
            {
                DeclarationID = e.DecHead.ID,
                EdocID = string.Concat(e.DecHead.CustomMaster, serino),
                EdocCode = ConstConfig.PaymentInstruction,
                EdocFomatType = ConstConfig.EdocFomatType,
                EdocCopId = e.FileName,
                EdocSize = e.FileSize,
                FileUrl = e.FilePath,
                SignTime = DateTime.Now,
                EdocOwnerCode = e.DecHead.AgentCusCode,
                EdocOwnerName = e.DecHead.AgentName
            };
            edoc.Enter();
        }

        #endregion

        #region 合同

        /// <summary>
        /// 生成电子单据-合同
        /// </summary>
        public void ContractSaveAs()
        {
            //因每天10个报关单，需要生成30个文件，如果做到每天100个，数量就很大，要做到自动按时间新建文件夹 （201901/31）
            //输入的值如果是系统统一配置、固定的值，可在函数内部读取，不再进行输入。但是，系统配置的固定值一定要统一封装。

            string decHeadID = this.ID;
            //var vendor = new VendorContext(VendorContextInitParam.DecHeadID, decHeadID).Current1;
            //this.OwnerName = "北京创新在线电子产品销售有限公司杭州分公司";
            var vendor = new Needs.Ccs.Services.VendorContext(Needs.Ccs.Services.VendorContextInitParam.Pointed, this.OwnerName).Current1;
            var result = this.Contract.SaveAs(vendor);
            this.OnContractSaved(new ContractSavedEventArgs(this, result[0], result[1], result[2]));
        }

        /// <summary>
        /// 生成电子单据-合同调用事件
        /// </summary>
        /// <param name="args"></param>
        public virtual void OnContractSaved(ContractSavedEventArgs args)
        {
            this.ContractSaved?.Invoke(this, args);
        }

        /// <summary>
        /// 生成电子单据-合同成功后执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecHead_ContractSaved(object sender, ContractSavedEventArgs e)
        {
            var serino = Needs.Overall.PKeySigner.Pick(PKeyType.EdocContact);
            var edoc = new EdocRealation()
            {
                DeclarationID = e.DecHead.ID,
                EdocID = string.Concat(e.DecHead.CustomMaster, serino),
                EdocCode = ConstConfig.Contract,
                EdocFomatType = ConstConfig.EdocFomatType,
                EdocCopId = e.FileName,
                SignTime = DateTime.Now,
                EdocOwnerCode = e.DecHead.AgentCusCode,
                EdocOwnerName = e.DecHead.AgentName,
                EdocSize = e.FileSize,
                FileUrl = e.FilePath
            };
            edoc.Enter();
        }

        #endregion

        #region 装箱单

        /// <summary>
        /// 生成电子单据-装箱单
        /// </summary>
        public void PackingListSaveAs(Vendor vendor)
        {
            var result = this.PackingList.SaveAs(vendor);
            this.OnPackingListSaved(new PackingListSavedEventArgs(this, result[0], result[1], result[2]));
        }

        /// <summary>
        /// 生成电子单据-装箱单调用事件
        /// </summary>
        /// <param name="args"></param>
        public virtual void OnPackingListSaved(PackingListSavedEventArgs args)
        {
            this.PackingListSaved?.Invoke(this, args);
        }

        /// <summary>
        /// 生成电子单据-装箱单成功后执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecHead_PackingListSaved(object sender, PackingListSavedEventArgs e)
        {
            var serino = Needs.Overall.PKeySigner.Pick(PKeyType.EdocPacking);
            var edoc = new EdocRealation()
            {
                DeclarationID = e.DecHead.ID,
                EdocID = string.Concat(e.DecHead.CustomMaster, serino),
                EdocCode = ConstConfig.PackingList,
                EdocFomatType = ConstConfig.EdocFomatType,
                EdocCopId = e.FileName,
                SignTime = DateTime.Now,
                EdocOwnerCode = e.DecHead.AgentCusCode,
                EdocOwnerName = e.DecHead.AgentName,
                EdocSize = e.FileSize,
                FileUrl = e.FilePath
            };
            edoc.Enter();
        }

        #endregion

        #region 取消报关单
        public void CancelDecHead()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    //更改报关单状态
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { CusDecStatus = MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.Cancel) }, item => item.ID == this.ID);
                    //更改报关通知项状态
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecLists>(new { CusDecStatus = (int)Enums.CusItemDecStatus.Cancel }, item => item.DeclarationID == this.ID);

                    var ManifestConsignments = new Ccs.Services.Views.ManifestConsignmentsView().Where(item => item.ID == this.BillNo).FirstOrDefault();
                    if (ManifestConsignments != null)
                    {
                        ManifestConsignments.Cancel();
                    }
                }

                this.OnCanceled(new DeclareCreatedEventArgs(this));
            }
            catch (Exception ex)
            {

            }
        }

        public virtual void OnCanceled(DeclareCreatedEventArgs args)
        {
            this.DeclareCanceled?.Invoke(this, args);
        }

        private void DecHead_Canceled(object sender, DeclareCreatedEventArgs e)
        {
            //写入日志与轨迹
            e.DecHead.Trace("取消报关单");
        }

        #endregion

        #region 提交完税价格
        public string PostInsideDutiablePrice()
        {
            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            Expression<Func<DutiablePriceItem, bool>> expression = item => item.DecHeadID == this.ID;

            var Result = new Needs.Ccs.Services.Views.DutiablePriceView<DutiablePriceItem>().GetIQueryableResult(expression);

            List<DutiablePriceItem> calcedItems = new List<DutiablePriceItem>();

            foreach (var item in Result)
            {
                DutiablePriceItem m = new DutiablePriceItem();
                m.ProductUniqueCode = item.ProductUniqueCode;
                m.DutiablePrice = (item.DutiablePrice * this.CustomsExchangeRate.Value).ToRound(0) + ((item.DutiablePrice * this.CustomsExchangeRate.Value).ToRound(0) * item.TariffRate).ToRound(2);
                m.TaxName = item.TaxName;
                m.TaxCode = item.TaxCode;
                m.DeclareDate = item.DeclareDate;
                m.EntryId = item.EntryId;
                m.Model = item.Model;
                m.Qty = item.Qty;
                calcedItems.Add(m);
            }

            var PostMessage = calcedItems.Select(item => new
            {
                单据号 = item.ProductUniqueCode == "" ? "" : Encryption.Encrypt(item.ProductUniqueCode),
                完税价格 = item.DutiablePrice,
                合同号 = this.ContrNo,
                型号信息分类 = item.TaxName,
                型号信息分类值 = item.TaxCode,
                报关完成日期 = this.IEDate.Insert(4, "-").Insert(7, "-"),
                报关单号 = this.EntryId,
                开票公司AA = "",
                规格型号E = item.Model,
            }).OrderByDescending(item => item.单据号);

            var SendJson = new
            {
                requestitem = "匹配价格",
                data = PostMessage,
                key = System.Configuration.ConfigurationManager.AppSettings["InsideDutiablePricePostKey"]
            }.Json();

            IcgooPost post = new IcgooPost();
            post.PostUrl = System.Configuration.ConfigurationManager.AppSettings["InsideDutiablePricePostUrl"];
            string result = post.PostInside(SendJson);

            return result;
        }


        #endregion

        #region 深圳仓库自动出库
        public void SZWarehouseAutoOut()
        {
            var Order = new IcgooOrdersView().Where(item => item.ID == this.OrderID).FirstOrDefault();

            if (Order != null)
            {
                if (Order.Type == OrderType.Icgoo || Order.Type == OrderType.Inside)
                {
                    using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    {
                        //更新ExitNotice主表的状态
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.ExitNotices>(
                          new
                          {
                              UpdateDate = DateTime.Now,
                              ExitNoticeStatus = ExitNoticeStatus.Exited
                          }, item => item.OrderID == this.OrderID);

                        //更新ExitNoticeItems表状态
                        var hkexitNotice = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNotices>().Where(item => item.OrderID == this.OrderID).FirstOrDefault();
                        string ExitNoticesID = hkexitNotice.ID;
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.ExitNoticeItems>(
                          new
                          {
                              UpdateDate = DateTime.Now,
                              ExitNoticeStatus = ExitNoticeStatus.Exited
                          }, item => item.ExitNoticeID == ExitNoticesID);

                        //生成深圳仓库的入库通知
                        var hkExitNotices = new Needs.Ccs.Services.Views.ExitNoticeView().Where(item => item.ID == ExitNoticesID);
                        string szEntryNoticesID = "SZ" + Needs.Overall.PKeySigner.Pick(PKeyType.EntryNotice);
                        var szEntryNotices = new Layer.Data.Sqls.ScCustoms.EntryNotices
                        {
                            ID = szEntryNoticesID,
                            OrderID = Order.ID,
                            WarehouseType = (int)WarehouseType.ShenZhen,
                            DecHeadID = this.ID,
                            ClientCode = Order.Client.ClientCode,
                            SortingRequire = (int)SortingRequire.Packed,
                            EntryNoticeStatus = (int)EntryNoticeStatus.UnBoxed,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            Status = (int)Status.Normal
                        };
                        reponsitory.Insert(szEntryNotices);

                        //生成深圳入库通知项
                        var hkItems = new Views.HKExitNoticeItemView().Where(eni => eni.ExitNoticeID == ExitNoticesID)
                                                         .OrderBy(eni => eni.ExitNoticeID).ThenBy(eni => eni.ID).ToArray();
                        var szEntryNoticeItems = hkItems.Select(item => new Layer.Data.Sqls.ScCustoms.EntryNoticeItems()
                        {
                            ID = Needs.Overall.PKeySigner.Pick(PKeyType.EntryNoticeItem),
                            EntryNoticeID = szEntryNoticesID,
                            DecListID = item.DecList.ID,
                            IsSpotCheck = false,
                            EntryNoticeStatus = (int)EntryNoticeStatus.UnBoxed,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            Status = (int)Status.Normal,
                        }).ToArray();
                        reponsitory.Insert(szEntryNoticeItems);

                        //生成深圳分拣结果
                        var szSortings = hkItems.Select(item => new Layer.Data.Sqls.ScCustoms.Sortings()
                        {
                            ID = Needs.Overall.PKeySigner.Pick(PKeyType.Sorting),
                            OrderID = item.DecList.OrderID,
                            OrderItemID = item.DecList.OrderItemID,
                            EntryNoticeItemID = szEntryNoticeItems.FirstOrDefault(eni => eni.DecListID == item.DecList.ID).ID,
                            //ProductID = item.DecList.DeclarationNoticeItem.Sorting.OrderItem.ID,
                            Quantity = item.DecList.GQty,
                            BoxIndex = item.DecList.CaseNo,
                            NetWeight = item.DecList.NetWt == null ? 0M : (decimal)item.DecList.NetWt,
                            GrossWeight = item.DecList.GrossWt == null ? 0M : (decimal)item.DecList.GrossWt,
                            DecStatus = (int)SortingDecStatus.Yes,
                            AdminID = hkexitNotice.AdminID,
                            WrapType = this.WrapType,
                            WarehouseType = (int)WarehouseType.ShenZhen,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            Status = (int)Status.Normal,
                        }).ToArray();
                        reponsitory.Insert(szSortings);

                    }


                    //出库通知
                    var exitNotice = new SZExitNotice();
                    exitNotice.WarehouseType = WarehouseType.ShenZhen;
                    var order = new Needs.Ccs.Services.Models.Order();
                    order.ID = Order.MainOrderID;
                    exitNotice.Order = order;
                    string AdminID = System.Configuration.ConfigurationManager.AppSettings["OutAdminID"];
                    exitNotice.Admin = Needs.Underly.FkoFactory<Admin>.Create(AdminID);


                    //送货信息
                    var sortings = new SZSortingsView().Where(item => item.OrderID == this.OrderID).OrderBy(item => item.BoxIndex).AsQueryable();
                    exitNotice.ExitDeliver = new ExitDeliver();
                    //exitNotice.ExitDeliver.ExitNoticeID = exitNotice.ID;
                    //exitNotice.ExitDeliver.Code = exitNotice.ID;
                    exitNotice.ExitDeliver.PackNo = this.PackNo;

                    exitNotice.ExitType = ExitType.Delivery;
                    exitNotice.ExitDeliver.Name = Order.Client.Company.Name;
                    exitNotice.ExitDeliver.DeliverDate = DateTime.Now.AddDays(1);

                    //送货人
                    var ClientConsignees = new Needs.Ccs.Services.Views.ClientConsigneesView().Where(item => item.ClientID == Order.Client.ID).FirstOrDefault();
                    exitNotice.ExitDeliver.Deliver = new Deliver();
                    exitNotice.ExitDeliver.Deliver.Driver = new Needs.Ccs.Services.Views.DriverView().Where(item => item.Name == System.Configuration.ConfigurationManager.AppSettings["AutoDriver"]).FirstOrDefault();
                    exitNotice.ExitDeliver.Deliver.Vehicle = new Needs.Ccs.Services.Views.VehicleView().Where(item => item.License == System.Configuration.ConfigurationManager.AppSettings["AutoVehicle"]).FirstOrDefault();
                    //exitNotice.ExitDeliver.Deliver.Driver = new Needs.Ccs.Services.Views.DriverView().Where(item => item.Name == "李昌伟").FirstOrDefault();
                    //exitNotice.ExitDeliver.Deliver.Vehicle = new Needs.Ccs.Services.Views.VehicleView().Where(item => item.License == "粤B8P1K1").FirstOrDefault();
                    exitNotice.ExitDeliver.Deliver.Contact = ClientConsignees?.Contact.Name;
                    exitNotice.ExitDeliver.Deliver.Mobile = ClientConsignees?.Contact.Mobile;
                    exitNotice.ExitDeliver.Deliver.Address = ClientConsignees?.Address;
                    exitNotice.IsPrint = IsPrint.UnPrint;


                    //出库通知项
                    foreach (var sorting in sortings)
                    {
                        string sortingID = sorting.ID;
                        exitNotice.Items.Add(new ExitNoticeItem
                        {
                            ExitNoticeID = exitNotice.ID,
                            Sorting = sorting,
                            Quantity = sorting.Quantity,
                        });
                    }

                    exitNotice.Enter();
                }
            }

        }
        #endregion

        #region 深圳仓库自动出库 导历史数据专用
        public void SZWarehouseAutoOut(string forhistory)
        {
            var Order = new IcgooOrdersView().Where(item => item.ID == this.OrderID).FirstOrDefault();

            if (Order != null)
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    //更新ExitNotice主表的状态
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.ExitNotices>(
                      new
                      {
                          UpdateDate = DateTime.Now,
                          ExitNoticeStatus = ExitNoticeStatus.Exited
                      }, item => item.OrderID == this.OrderID);

                    //更新ExitNoticeItems表状态
                    string ExitNoticesID = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNotices>().Where(item => item.OrderID == this.OrderID).FirstOrDefault().ID;
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.ExitNoticeItems>(
                      new
                      {
                          UpdateDate = DateTime.Now,
                          ExitNoticeStatus = ExitNoticeStatus.Exited
                      }, item => item.ExitNoticeID == ExitNoticesID);

                    //生成深圳仓库的入库通知
                    var hkExitNotices = new Needs.Ccs.Services.Views.ExitNoticeView().Where(item => item.ID == ExitNoticesID);
                    string szEntryNoticesID = "SZ" + Needs.Overall.PKeySigner.Pick(PKeyType.EntryNotice);
                    var szEntryNotices = new Layer.Data.Sqls.ScCustoms.EntryNotices
                    {
                        ID = szEntryNoticesID,
                        OrderID = Order.ID,
                        WarehouseType = (int)WarehouseType.ShenZhen,
                        DecHeadID = this.ID,
                        ClientCode = Order.Client.ClientCode,
                        SortingRequire = (int)SortingRequire.Packed,
                        EntryNoticeStatus = (int)EntryNoticeStatus.UnBoxed,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Status = (int)Status.Normal
                    };
                    reponsitory.Insert(szEntryNotices);

                    //生成深圳入库通知项
                    var hkItems = new Views.HKExitNoticeItemView().Where(eni => eni.ExitNoticeID == ExitNoticesID)
                                                     .OrderBy(eni => eni.ExitNoticeID).ThenBy(eni => eni.ID).ToArray();
                    var szEntryNoticeItems = hkItems.Select(item => new Layer.Data.Sqls.ScCustoms.EntryNoticeItems()
                    {
                        ID = Needs.Overall.PKeySigner.Pick(PKeyType.EntryNoticeItem),
                        EntryNoticeID = szEntryNoticesID,
                        DecListID = item.DecList.ID,
                        IsSpotCheck = false,
                        EntryNoticeStatus = (int)EntryNoticeStatus.UnBoxed,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Status = (int)Status.Normal,
                    }).ToArray();
                    reponsitory.Insert(szEntryNoticeItems);

                    //生成深圳分拣结果
                    var szSortings = hkItems.Select(item => new Layer.Data.Sqls.ScCustoms.Sortings()
                    {
                        ID = Needs.Overall.PKeySigner.Pick(PKeyType.Sorting),
                        OrderID = item.DecList.OrderID,
                        OrderItemID = item.DecList.OrderItemID,
                        EntryNoticeItemID = szEntryNoticeItems.FirstOrDefault(eni => eni.DecListID == item.DecList.ID).ID,
                        //ProductID = item.DecList.DeclarationNoticeItem.Sorting.OrderItem.ID,
                        Quantity = item.DecList.GQty,
                        BoxIndex = item.DecList.CaseNo,
                        NetWeight = item.DecList.NetWt == null ? 0M : (decimal)item.DecList.NetWt,
                        GrossWeight = item.DecList.GrossWt == null ? 0M : (decimal)item.DecList.GrossWt,
                        DecStatus = (int)SortingDecStatus.Yes,
                        AdminID = hkExitNotices.FirstOrDefault().Admin.ID,
                        WrapType = this.WrapType,
                        WarehouseType = (int)WarehouseType.ShenZhen,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Status = (int)Status.Normal,
                    }).ToArray();
                    reponsitory.Insert(szSortings);

                }


                //出库通知
                var exitNotice = new SZExitNotice();
                exitNotice.WarehouseType = WarehouseType.ShenZhen;
                exitNotice.Order = Order;
                string AdminID = System.Configuration.ConfigurationManager.AppSettings["OutAdminID"];
                exitNotice.Admin = Needs.Underly.FkoFactory<Admin>.Create(AdminID);


                //送货信息
                var sortings = new SZSortingsView().Where(item => item.OrderID == this.OrderID).OrderBy(item => item.BoxIndex).AsQueryable();
                exitNotice.ExitDeliver = new ExitDeliver();
                //exitNotice.ExitDeliver.ExitNoticeID = exitNotice.ID;
                //exitNotice.ExitDeliver.Code = exitNotice.ID;
                exitNotice.ExitDeliver.PackNo = this.PackNo;

                exitNotice.ExitType = ExitType.Delivery;
                exitNotice.ExitDeliver.Name = Order.Client.Company.Name;
                exitNotice.ExitDeliver.DeliverDate = DateTime.Now.AddDays(1);

                //送货人
                exitNotice.ExitDeliver.Deliver = new Deliver();
                exitNotice.ExitDeliver.Deliver.Driver = new Needs.Ccs.Services.Views.DriverView().Where(item => item.Name == System.Configuration.ConfigurationManager.AppSettings["AutoDriver"]).FirstOrDefault();
                exitNotice.ExitDeliver.Deliver.Vehicle = new Needs.Ccs.Services.Views.VehicleView().Where(item => item.License == System.Configuration.ConfigurationManager.AppSettings["AutoVehicle"]).FirstOrDefault();
                //exitNotice.ExitDeliver.Deliver.Driver = new Needs.Ccs.Services.Views.DriverView().Where(item => item.Name == "李昌伟").FirstOrDefault();
                //exitNotice.ExitDeliver.Deliver.Vehicle = new Needs.Ccs.Services.Views.VehicleView().Where(item => item.License == "粤B8P1K1").FirstOrDefault();
                exitNotice.ExitDeliver.Deliver.Contact = Order.Client.Company.Contact.Name;
                exitNotice.ExitDeliver.Deliver.Mobile = Order.Client.Company.Contact.Mobile;
                exitNotice.ExitDeliver.Deliver.Address = Order.Client.Company.Address;
                exitNotice.IsPrint = IsPrint.UnPrint;
                exitNotice.ExitNoticeStatus = ExitNoticeStatus.Exited;


                //出库通知项
                foreach (var sorting in sortings)
                {
                    string sortingID = sorting.ID;
                    exitNotice.Items.Add(new ExitNoticeItem
                    {
                        ExitNoticeID = exitNotice.ID,
                        Sorting = sorting,
                        Quantity = sorting.Quantity,
                        ExitNoticeStatus = ExitNoticeStatus.Exited
                    });
                }

                exitNotice.Enter();

            }

        }
        #endregion

        #region Icgoo完税价格提交
        public string PostIcgooDutiablePrice()
        {
            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            Expression<Func<DutiablePriceItem, bool>> expression = item => item.DecHeadID == this.ID;

            var Result = new Needs.Ccs.Services.Views.DutiablePriceView<DutiablePriceItem>().GetIQueryableResult(expression);

            List<IcgooDutiablePriceItem> calcedItems = new List<IcgooDutiablePriceItem>();

            foreach (var item in Result)
            {
                IcgooDutiablePriceItem m = new IcgooDutiablePriceItem();
                m.id = item.OrderItemID;
                m.sale_orderline_id = item.ProductUniqueCode;
                m.partno = item.Model;
                m.supplier = item.Supplier;
                m.mfr = item.Manfacture;
                m.brand = item.Manfacture;
                m.origin = item.Origin;
                m.customs_rate = CalcAdded(item.Origin, item.HSCode, item.TariffRate)[1];
                m.origin_tax = CalcAdded(item.Origin, item.HSCode, item.TariffRate)[0];
                m.add_rate = item.AddedValueRate;
                m.product_name = StringToUnicode(item.ProductName);
                m.category = "";
                m.hs_code = item.HSCode;
                m.tax_code = item.TaxCode;
                m.qty = Convert.ToInt16(item.Qty);
                calcedItems.Add(m);
            }


            string json = JsonConvert.SerializeObject(new
            {
                total_item_num = calcedItems.Count(),
                custom_partner = 94,
                item = calcedItems
            });


            IcgooPost post = new IcgooPost();
            post.PostUrl = System.Configuration.ConfigurationManager.AppSettings["IcgooDutiablePricePostUrl"];
            string result = post.PostIcgooDutiablePrice(json);

            return result;
        }

        private decimal[] CalcAdded(string Origin, string HSCode, decimal Tariff)
        {
            var tariffView = new Needs.Ccs.Services.Views.CustomsOriginTariffsView();

            var reTariff = tariffView.Where(item => item.Type == CustomsRateType.ImportTax &&
                                            item.Origin == Origin &&
                                            item.CustomsTariffID == HSCode).FirstOrDefault();

            if (reTariff != null)
            {
                return new decimal[2] { reTariff.Rate / 100, Tariff - reTariff.Rate / 100 };
            }
            else
            {
                return new decimal[2] { 0, Tariff };
            }
        }

        private static string StringToUnicode(string value)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(value);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i += 2)
            {
                stringBuilder.AppendFormat("\\u{0}{1}", bytes[i + 1].ToString("x").PadLeft(2, '0'), bytes[i].ToString("x").PadLeft(2, '0'));
            }
            return stringBuilder.ToString();
        }
        #endregion

        #region TOD0: 主订单的对账单对象
        protected MainOrderBillViewModel getModel(string id)
        {

            var viewModel = new MainOrderBillViewModel();
            var model = getModelStander(id);
            if (model == null)
            {
                return null;
            }
            else
            {
                #region 两个Model 转换              
                viewModel.MainOrderID = id;

                viewModel.Bills = model.Bills;

                var purchaser = PurchaserContext.Current;
                viewModel.AgentName = purchaser.CompanyName;
                viewModel.AgentAddress = purchaser.Address;
                viewModel.AgentTel = purchaser.Tel;
                viewModel.AgentFax = purchaser.UseOrgPersonTel;
                viewModel.Purchaser = purchaser.CompanyName;
                viewModel.Bank = purchaser.BankName;
                viewModel.Account = purchaser.AccountName;
                viewModel.AccountId = purchaser.AccountId;
                viewModel.SealUrl = PurchaserContext.Current.SealUrl.ToUrl();

                viewModel.ClientName = model.OrderBill.Client.Company.Name;
                viewModel.ClientTel = model.OrderBill.Client.Company.Contact.Tel;
                viewModel.Currency = model.OrderBill.Currency;
                viewModel.IsLoan = model.OrderBill.IsLoan;
                viewModel.DueDate = model.OrderBill.GetDueDate().ToString("yyyy年MM月dd日");
                viewModel.CreateDate = model.OrderBill.CreateDate.ToString();
                viewModel.ClientType = model.OrderBill.Client.ClientType;


                viewModel.summaryTotalPrice = model.BillTotalPrice;
                viewModel.summaryTotalCNYPrice = model.BillTotalCNYPrice;
                viewModel.summaryTotalTariff = model.BillTotalTariff;
                viewModel.summaryTotalExciseTax = model.BillTotalExciseTax;
                viewModel.summaryTotalAddedValueTax = model.BillTotalAddedValueTax;
                viewModel.summaryTotalAgencyFee = model.BillTotalAgencyFee;
                viewModel.summaryTotalIncidentalFee = model.BillTotalIncidentalFee;

                viewModel.summaryPay = model.BillTotalTaxAndFee;
                viewModel.summaryPayAmount = model.BillTotalDeclarePrice;


                viewModel.CreateDate = model.MainOrder.CreateDate.ToString("yyyy-MM-dd HH:mm");
                #endregion

                return viewModel;
            }

        }

        private MainOrderBillStander getModelStander(string id)
        {
            var Orders = new Orders2View().Where(item => item.MainOrderID == id && item.OrderStatus >= Needs.Ccs.Services.Enums.OrderStatus.Quoted
                                                  && item.OrderStatus != Needs.Ccs.Services.Enums.OrderStatus.Canceled && item.OrderStatus != Needs.Ccs.Services.Enums.OrderStatus.Returned)
                         .ToList();

            var purchaser = PurchaserContext.Current;
            if (Orders.Count == 0)
            {
                return null;
            }
            else
            {
                MainOrderBillStander mainOrderBillStander = new MainOrderBillStander(purchaser, Orders);

                return mainOrderBillStander;
            }
        }
        #endregion

        #region 重发报文

        public void ResendMsg()
        {
            this.CusDecStatus = MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.Make);
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //更改报关单状态
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { CusDecStatus = MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.Make) }, item => item.ID == this.ID);
            }
            this.Trace("报关单重发报文");
        }

        #endregion

        #region 修改报关单状态
        /// <summary>
        /// 修改报关单状态
        /// </summary>
        public void SaveDecheadStatus()
        {
            //更新报关单状态,以及海关编号
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { this.CusDecStatus }, item => item.ID == this.ID);
            }
        }

        #endregion

        #region 设置保函号

        /// <summary>
        /// 设置保函号
        /// </summary>
        public void SetGuarateeNo()
        {
           
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { GuaranteeNo = this.GuarateeNo }, item => item.ID == this.ID);
            }
        }

        #endregion

    }

    /// <summary>
    /// 客户端报关单
    /// </summary>
    public class ClientDecHead : DecHead
    {
        /// <summary>
        ///报关总金额
        /// </summary>
        public decimal TotalDeclarePrice { get; set; }

        /// <summary>
        /// 报关单文件
        /// </summary>
        public IEnumerable<DecHeadFile> files { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 报关单文件
        /// </summary>
        public DecHeadFile DecHeadFile
        {
            get
            {
                return this.files.Where(item => item.FileType == Enums.FileType.DecHeadFile && item.Status == Status.Normal).FirstOrDefault();
            }
        }
    }
}
