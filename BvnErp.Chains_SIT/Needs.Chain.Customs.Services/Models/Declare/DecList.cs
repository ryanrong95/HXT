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
    /// 报关单表体
    /// </summary>
    [Serializable]
    public class DecList : IUnique, IPersist
    {
        #region 属性

        /// <summary>
        /// 主键ID（DeclarationID+项号）MD5
        /// </summary>
        string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.DeclarationID, this.GNo).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 报关单ID
        /// </summary>
        public string DeclarationID { get; set; }

        /// <summary>
        /// 报关单项ID
        /// </summary>
        public string DecListID { get; set; }

        /// <summary>
        /// 保管通知项ID
        /// </summary>
        public string DeclarationNoticeItemID { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 订单项ID
        /// </summary>
        public string OrderItemID { get; set; }

        public DeclarationNoticeItem DeclarationNoticeItem { get; set; }

        /// <summary>
        /// 商品序号/项号
        /// </summary>
        public int GNo { get; set; }

        /// <summary>
        /// 10位商编
        /// </summary>
        public string CodeTS { get; set; }

        /// <summary>
        /// 检验检疫编码
        /// </summary>
        public string CiqCode { get; set; }

        /// <summary>
        /// 报关品名
        /// </summary>
        public string GName { get; set; }

        /// <summary>
        /// 规格型号（申报要素）
        /// </summary>
        public string GModel { get; set; }

        /// <summary>
        /// 成交数量
        /// </summary>
        public decimal GQty { get; set; }

        /// <summary>
        /// 成交单位
        /// </summary>
        public string GUnit { get; set; }

        /// <summary>
        /// 法定第一单位
        /// </summary>
        public string FirstUnit { get; set; }

        private decimal? firstQty { get; set; }

        /// <summary>
        /// 法定第一数量
        /// </summary>
        public decimal? FirstQty
        {
            get
            {
                return this.getFirstQty();
            }

            set
            {
                this.firstQty = value;
            }
        }

        /// <summary>
        /// 法定第二单位
        /// </summary>
        public string SecondUnit { get; set; }

        /// <summary>
        /// 法定第二数量
        /// </summary>        
        private decimal? secondQty { get; set; }
        public decimal? SecondQty
        {
            get
            {
                return this.getSecondQty();
            }
            set
            {
                this.secondQty = value;
            }
        }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal DeclPrice { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal DeclTotal { get; set; }

        /// <summary>
        /// 成交币制
        /// </summary>
        public string TradeCurr { get; set; }

        /// <summary>
        /// 原产地国别
        /// </summary>
        public string OriginCountry { get; set; }

        /// <summary>
        /// 原产地国别名
        /// </summary>
        public string OriginCountryName { get; set; }

        /// <summary>
        /// 是否最惠国 产地
        /// </summary>
        public bool? IsHOrigin { get; set; }

        /// <summary>
        /// 最终目的国（地区）
        /// </summary>
        public string DestinationCountry
        {
            get
            {
                return "CHN";//目的国默认中国
            }
        }

        /// <summary>
        /// 目的地代码 国内行政区划代码 默认：440307 龙岗区
        /// </summary>
        public string DestCode
        {
            get
            {
                return "440307";//龙岗区
            }
        }

        /// <summary>
        /// 境内目的地/境内货源地 国内地区代码 默认：44031 深圳特区
        /// </summary>
        public string DistrictCode
        {
            get
            {
                return "44031";//深圳特区
            }
        }

        /// <summary>
        /// 征减免税方式 :默认照章征税1
        /// </summary>
        public int DutyMode
        {
            get
            {
                return 1;//照章征税
            }
        }

        /// <summary>
        /// 检验检疫货物规格
        /// </summary>
        public string GoodsSpec { get; set; }

        /// <summary>
        /// 货物型号
        /// </summary>
        public string GoodsModel { get; set; }

        /// <summary>
        /// 货物品牌
        /// </summary>
        public string GoodsBrand { get; set; }

        /// <summary>
        /// 箱号
        /// </summary>
        public string CaseNo { get; set; }

        /// <summary>
        /// 净重
        /// </summary>
        public decimal? NetWt { get; set; }

        /// <summary>
        /// 毛重
        /// </summary>
        public decimal? GrossWt { get; set; }

        /// <summary>
        /// 用途代码 默认：99 其它
        /// </summary>
        public string Purpose { get; set; }
        public string PurposeName
        {
            get
            {
                string name = "";
                if (this.Purpose != null && this.Purpose != "")
                {
                    using (var view = new Needs.Ccs.Services.Views.BasePurposesView())
                    {
                        name = view.Where(item => item.Code == this.Purpose).Select(item => item.Name).FirstOrDefault();
                    }
                }
                return name;
            }
        }

        /// <summary>
        /// 货物属性 默认：19 正常
        /// </summary>
        public string GoodsAttr { get; set; }
        public string GoodsAttrName
        {
            get
            {
                string name = "";
                if (this.GoodsAttr != null && this.GoodsAttr != "")
                {
                    string[] attr = this.GoodsAttr.Split(',');
                    using (var view = new Needs.Ccs.Services.Views.BaseGoodsAttrsView())
                    {
                        for (int i = 0; i < attr.Length; i++)
                        {
                            name += view.Where(item => item.Code == attr[i]).Select(item => item.Name).FirstOrDefault() + ",";
                        }
                    }
                    name = name.Substring(0, name.Length - 1);
                }
                return name;
            }
        }

        /// <summary>
        /// 检验检疫名称
        /// </summary>
        public string CiqName { get; set; }

        /// <summary>
        /// 产品批次号
        /// </summary>
        public string GoodsBatch { get; set; }

        /// <summary>
        /// 备案序号
        /// </summary>
        public decimal? ContrItem { get; set; }

        /// <summary>
        /// 原产地区
        /// </summary>
        public string OrigPlaceCode { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public CusItemDecStatus CusDecStatus { get; set; }


        /// <summary>
        /// 许可证信息
        /// </summary>
        DecGoodsLimits limits;
        public DecGoodsLimits Limits
        {
            get
            {
                if (limits == null)
                {
                    using (var view = new Views.DecGoodsLimitsView())
                    {
                        var query = view.Where(item => item.DecListID == this.ID);
                        this.Limits = new DecGoodsLimits(query);
                    }
                }
                return this.limits;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.limits = new DecGoodsLimits(value, new Action<DecGoodsLimit>(delegate (DecGoodsLimit item)
                {
                    item.DecListID = this.ID;
                }));
            }
        }


        /// <summary>
        /// 旧的 DecList
        /// </summary>
        public DecList OldDecList { get; set; }

        #region  订单的单价，总价 2020-09-03 by yeshuangshuang

        /// <summary>
        /// 订单单价
        /// </summary>
        public decimal OrderPrice { get; set; }

        /// <summary>
        /// 订单总价
        /// </summary>
        public decimal OrderTotal { get; set; }
        /// <summary>
        /// 完税价格（只包含关税）
        /// </summary>
        public decimal? TaxedPrice { get; set; }
        public string InputID { get; set; }

        #endregion

        #endregion

        public DecList()
        {
            //TODO：设置默认值
            ChangeSuccess += Changed;
        }

        /// <summary>
        /// 返回第一数量
        /// </summary>
        /// <returns></returns>
        private decimal? getFirstQty()
        {
            decimal? qty;
            qty = null;
            switch (this.FirstUnit)
            {
                case "054"://千个
                    qty = (this.GQty / 1000).ToRound(3);
                    break;
                case "035"://千克
                    qty = this.NetWt;
                    break;
                case "036"://克
                    qty = this.NetWt * 1000;
                    break;
                case "001"://台
                case "006"://套
                case "007"://个
                case "008"://只
                case "017"://块
                    qty = this.GQty;
                    break;
                default: break;
            }

            return qty;
        }

        /// <summary>
        /// 返回第二数量
        /// </summary>
        /// <returns></returns>
        private decimal? getSecondQty()
        {
            decimal? qty;
            qty = null;
            switch (this.SecondUnit)
            {
                case "054"://千个
                    qty = (this.GQty / 1000).ToRound(3);
                    break;
                case "035"://千克
                    qty = this.NetWt;
                    break;
                case "036"://克
                    qty = this.NetWt * 1000;
                    break;
                case "001"://台
                case "007"://个
                case "008"://只
                case "017"://块
                    qty = this.GQty;
                    break;
                default: break;
            }

            return qty;
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder ChangeSuccess;


        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>().Count(item => item.ID == this.ID);

                if (count == 0)
                {                    
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecLists>(new
                    {
                        ID = this.ID,
                        DeclarationID = this.DeclarationID,
                        DeclarationNoticeItemID = this.DeclarationNoticeItemID,
                        OrderID = this.OrderID,
                        OrderItemID = this.OrderItemID,
                        CusDecStatus = (int)this.CusDecStatus,
                        GNo = this.GNo,
                        CodeTS = this.CodeTS,
                        CiqCode = this.CiqCode,
                        GName = this.GName,
                        GModel = this.GModel,
                        GQty = this.GQty,
                        GUnit = this.GUnit,
                        FirstUnit = this.FirstUnit,
                        FirstQty = this.FirstQty,
                        SecondUnit = this.SecondUnit,
                        SecondQty = this.SecondQty,
                        DeclPrice = this.DeclPrice,
                        DeclTotal = this.DeclTotal,
                        TradeCurr = this.TradeCurr,
                        OriginCountry = this.OriginCountry,
                        DestinationCountry = this.DestinationCountry,
                        DestCode = this.DestCode,
                        DistrictCode = this.DistrictCode,
                        DutyMode = this.DutyMode,
                        GoodsSpec = this.GoodsSpec,
                        GoodsModel = this.GoodsModel,
                        GoodsBrand = this.GoodsBrand,
                        CaseNo = this.CaseNo,
                        NetWt = this.NetWt,
                        GrossWt = this.GrossWt,
                        Purpose = this.Purpose,
                        GoodsAttr = this.GoodsAttr,
                        ContrItem = this.ContrItem,
                        OrigPlaceCode = this.OrigPlaceCode,
                        CiqName = this.CiqName,
                        GoodsBatch = this.GoodsBatch
                    }, item => item.ID == this.ID);
                    this.OnChange();
                }
            }

            this.OnEnter();
        }

        virtual protected void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        virtual protected void OnChange()
        {
            if (this != null && this.ChangeSuccess != null)
            {
                this.ChangeSuccess(this, new SuccessEventArgs(this.DeclarationID));
            }
        }

        /// <summary>
        /// 已制单，已申报，暂存成功三种状态是可以修改的，修改完之后，重新生成xml，并且报关单状态改为草稿，不是已制单
        /// 如果改为已制单，会有问题，如果有两个产品需要更改，改第一个产品的时候，状态变为已制单，则会自动提交海关，第二个还会自动提交海关，会提交多次
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Changed(object sender, SuccessEventArgs e)
        {
            Needs.Ccs.Services.Models.DecHead decHead = new Ccs.Services.Views.DecHeadsView()[e.Object];
            if (decHead.CusDecStatus == MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.Make) ||
                decHead.CusDecStatus == MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.Declare) ||
                decHead.CusDecStatus == MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.S0)
               )
            {
                decHead.GoodsChange();
            }
        }
    }
}