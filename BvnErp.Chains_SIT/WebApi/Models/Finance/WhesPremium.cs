using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    /// <summary>
    /// 接口：库房费用调用数据要求
    /// </summary>
    public class WhesPremium
    {
        /// <summary>
        /// 
        /// </summary>
        public List<Premium> Premiums { get; set; }
    }

    /// <summary>
    /// 接口：库房费用调用数据要求
    /// </summary>
    public class HKWhesPremium
    {
        /// <summary>
        /// 
        /// </summary>
        public List<HKPremium> Premiums { get; set; }
    }

    /// <summary>
    /// 库房费用
    /// </summary>
    public class Premium
    {
       /// <summary>
       /// ID
       /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 小订单号
        /// </summary>
        public string TinyOrderID { get; set; }

        /// <summary>
        /// 添加人
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 库房费用类型
        /// </summary>
        public Needs.Ccs.Services.Enums.WarehousePremiumType WhesFeeType { get; set; }

        /// <summary>
        /// 数量 当前默认：1
        /// </summary>

        public int Count { get; set; } = 1;

        /// <summary>
        /// 单价 当前就写库房的金额
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 币种：CNY HKD
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 1:现金(实收)  2:非现金，记账(应收)
        /// </summary>
        public Needs.Ccs.Services.Enums.WhsePaymentType PaymentType { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }

        public Premium()
        {
            this.CreateDate = DateTime.Now;
        }
    }

    /// <summary>
    /// 香港库房重构，费用类型修改
    /// </summary>
    public class HKPremium : Premium
    {
        /// <summary>
        /// 香港库房费用名称：中文
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 费用类型转换
        /// </summary>
        public Needs.Ccs.Services.Enums.WarehousePremiumType HKWhesFeeType
        {
            get
            {
                WarehousePremiumType result = WarehousePremiumType.Other;
                switch (Subject)
                {
                    case SubjectConsts.入仓费:
                        result = WarehousePremiumType.EntryFee;
                        break;
                    case SubjectConsts.仓储费:
                        result = WarehousePremiumType.StorageFee;
                        break;
                    case SubjectConsts.收货异常费用:
                        result = WarehousePremiumType.UnNormalFee;
                        break;
                    case SubjectConsts.标签服务费:
                        result = WarehousePremiumType.ProcessLabelFee;
                        break;
                    case SubjectConsts.包装费:
                        result = WarehousePremiumType.ChangeBoxFee;
                        break;
                    case SubjectConsts.提货费:
                        result = WarehousePremiumType.DeliverFee;
                        break;
                    case SubjectConsts.快递费:
                        result = WarehousePremiumType.UnNormalFee;
                        break;
                    case SubjectConsts.登记费:
                        result = WarehousePremiumType.RegisterFee;
                        break;
                    case SubjectConsts.隧道费:
                        result = WarehousePremiumType.TunnelFee;
                        break;
                    case SubjectConsts.车场费:
                        result = WarehousePremiumType.parkingFee;
                        break;
                    case SubjectConsts.超重费:
                        result = WarehousePremiumType.OverweightFee;
                        break;
                    case SubjectConsts.包车费:
                        result = WarehousePremiumType.CharterFee;
                        break;
                    case "大陆来货清关费":
                        result = WarehousePremiumType.MainlandClearance;
                        break;
                    default:
                        break;
                }

                return result;
            }
        }
    }

    #region 科目名称
    /// <summary>
    /// 科目名称
    /// </summary>
    public class SubjectConsts : CodeType<string>
    {
        #region 代理费
        public const string 代理费 = nameof(代理费);
        #endregion

        #region 货款
        public const string 代付货款 = nameof(代付货款);
        public const string 代收货款 = nameof(代收货款);
        public const string 付汇 = nameof(付汇);
        public const string 供应商付汇 = nameof(供应商付汇);
        #endregion

        #region 税款
        public const string 关税 = nameof(关税);
        public const string 消费税 = nameof(消费税);
        public const string 海关增值税 = nameof(海关增值税);
        public const string 销售增值税 = nameof(销售增值税);
        #endregion

        #region 仓储费
        public const string 仓储费 = nameof(仓储费);
        #endregion

        #region 杂费
        public const string 商检费 = nameof(商检费);
        public const string 包车费 = nameof(包车费);
        public const string 提货费 = nameof(提货费);
        public const string 空车费 = nameof(空车费);

        public const string 清关费 = nameof(清关费);

        public const string 付汇手续费 = nameof(付汇手续费);
        public const string 付汇操作费 = nameof(付汇操作费);
        public const string 入仓费 = nameof(入仓费);
        public const string 隧道费 = nameof(隧道费);
        public const string 车场费 = nameof(车场费);
        public const string 登记费 = nameof(登记费);
        public const string 垫付运费 = nameof(垫付运费);
        public const string 承运商运费 = nameof(承运商运费);
        public const string 库位租赁费 = nameof(库位租赁费);
        public const string 超重费 = nameof(超重费);
        public const string 加急费 = nameof(加急费);
        public const string 等待费 = nameof(等待费);
        public const string 标签服务费 = nameof(标签服务费);
        //public const string 仓储费 = nameof(仓储费);
        public const string 包装费 = nameof(包装费);
        public const string 送货服务费 = nameof(送货服务费);
        public const string 收货服务费 = nameof(收货服务费);
        public const string 自提服务费 = nameof(自提服务费);
        public const string 快递运费 = nameof(快递运费);
        public const string 快递其他费用 = nameof(快递其他费用);

        public const string 其他 = nameof(其他);

        public const string 送货费 = nameof(送货费);
        public const string 快递费 = nameof(快递费);
        public const string 停车费 = nameof(停车费);
        public const string 收货异常费用 = nameof(收货异常费用);
        #endregion
    }
    #endregion

}