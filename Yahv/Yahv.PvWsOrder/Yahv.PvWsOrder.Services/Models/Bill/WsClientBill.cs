using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Common;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Models
{
    /// <summary>
    /// 客户账单
    /// </summary>
    public class Bill : IUnique
    {
        #region 属性

        public string ID { get; set; }

        public string ClientID { get; set; }

        public Currency Currency { get; set; }

        public bool IsInvoice { get; set; }

        public string AdminID { get; set; }

        public GeneralStatus Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ModifyDate { get; set; }

        public string Summary { get; set; }

        #endregion

        #region 扩展属性

        public Yahv.Services.Models.Admin Creater { get; set; }
        public WsClient Client { get; set; }

        public IEnumerable<BillItem> Items { get; set; }

        public string Price
        {
            get
            {
                if (this.Currency == Currency.CNY)
                {
                    return this.Items.Sum(t => t.CnyStatistics.Sum(k => Math.Round(k.LeftPrice, 2))).ToString("f2");
                }
                else
                {
                    //港币总金额
                    return this.Items.Sum(t => t.CnyStatistics.Sum(k => Math.Round(k.HKDLeftPrice, 2))).ToString("f2");
                }
            }
        }

        #endregion

        public Bill()
        {
            this.Status = GeneralStatus.Normal;
            this.CreateDate = this.ModifyDate = DateTime.Now;
        }

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (var Reponsitory = new PvWsOrderReponsitory())
            {
                if (!Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.Bills>().Any(item => item.ID == this.ID))
                {
                    this.ID = Guid.NewGuid().ToString();
                    Reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.Bills()
                    {
                        ID = this.ID,
                        ClientID = this.ClientID,
                        Currency = (int)this.Currency,
                        IsInvoice = this.IsInvoice,
                        AdminID = this.AdminID,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        ModifyDate = this.ModifyDate,
                        Summary = this.Summary,
                    });
                }
                else
                {
                    Reponsitory.Update<Layers.Data.Sqls.PvWsOrder.Bills>(new
                    {
                        this.ClientID,
                        Currency = (int)this.Currency,
                        this.IsInvoice,
                        this.AdminID,
                        Status = (int)this.Status,
                        ModifyDate = DateTime.Now,
                        this.Summary,
                    }, t => t.ID == this.ID);
                }
            }
        }

        public string ToPdf()
        {
            BillToPdf pdf = new BillToPdf(this);
            string fileName = DateTime.Now.Ticks + ".pdf";
            FileDirectory dic = new FileDirectory(fileName, FileType.ClientBillFile);
            pdf.SaveAs(dic.DownLoadRoot + fileName);

            string filePath = dic.DownLoadRoot + fileName;
            return filePath;
        }
    }

    public class BillItem : IUnique
    {
        #region 属性

        public string ID { get; set; }

        public string BillID { get; set; }

        public string OrderID { get; set; }

        #endregion

        #region 扩展属性

        /// <summary>
        /// 本位币账单
        /// </summary>
        public IEnumerable<Yahv.Services.Models.VoucherCnyStatistic> CnyStatistics { get; set; }

        public OrderOutput OrderOutput { get; set; }

        /// <summary>
        /// 列账日期
        /// </summary>
        public DateTime LeftDate
        {
            get
            {
                return this.CnyStatistics.Max(t => t.LeftDate);
            }
        }

        #region 本位币金额

        /// <summary>
        /// 应收总金额
        /// </summary>
        public decimal LeftTotalPrice
        {
            get
            {
                return this.CnyStatistics.Sum(t => t.LeftPrice);
            }
        }
        /// <summary>
        /// 实收总金额
        /// </summary>
        public decimal RightTotalPrice
        {
            get
            {
                return this.CnyStatistics.Sum(t => t.RightPrice) ?? 0m;
            }
        }
        /// <summary>
        /// 仓储费
        /// </summary>
        public decimal StockFee
        {
            get
            {
                return this.CnyStatistics.Where(t => t.Subject == "仓储费").Sum(t => t.LeftPrice);
            }
        }
        /// <summary>
        /// 标签费
        /// </summary>
        public decimal LabelFee
        {
            get
            {
                return this.CnyStatistics.Where(t => t.Subject != null && t.Subject.Contains("标签")).Sum(t => t.LeftPrice);
            }
        }
        /// <summary>
        /// 登记费
        /// </summary>
        public decimal RegistrationFee
        {
            get
            {
                return this.CnyStatistics.Where(t => t.Subject == "登记费").Sum(t => t.LeftPrice);
            }
        }
        /// <summary>
        /// 清关费
        /// </summary>
        public decimal CustomClearFee
        {
            get
            {
                return this.CnyStatistics.Where(t => t.Subject == "清关费").Sum(t => t.LeftPrice);
            }
        }
        /// <summary>
        /// 入仓费
        /// </summary>
        public decimal EnterFee
        {
            get
            {
                return this.CnyStatistics.Where(t => t.Subject == "入仓费").Sum(t => t.LeftPrice);
            }
        }
        /// <summary>
        /// 送货费
        /// </summary>
        public decimal DeliveryFee
        {
            get
            {
                return this.CnyStatistics.Where(t => t.Subject != null && t.Subject.Contains("送货费")).Sum(t => t.LeftPrice);
            }
        }
        /// <summary>
        /// 其它费用
        /// </summary>
        public decimal OtherFee
        {
            get
            {
                return this.LeftTotalPrice - this.StockFee - this.LabelFee - this.RegistrationFee - this.CustomClearFee - this.EnterFee - this.DeliveryFee;
            }
        }

        #endregion

        #region 港币金额

        /// <summary>
        /// 应收总金额
        /// </summary>
        public decimal HKDLeftTotalPrice
        {
            get
            {
                return this.CnyStatistics.Sum(t => t.HKDLeftPrice);
            }
        }
        /// <summary>
        /// 实收总金额
        /// </summary>
        public decimal HKDRightTotalPrice
        {
            get
            {
                return this.CnyStatistics.Sum(t => t.HKDRightPrice) ?? 0m;
            }
        }
        /// <summary>
        /// 仓储费
        /// </summary>
        public decimal HKDStockFee
        {
            get
            {
                return this.CnyStatistics.Where(t => t.Subject == "仓储费").Sum(t => t.HKDLeftPrice);
            }
        }
        /// <summary>
        /// 标签费
        /// </summary>
        public decimal HKDLabelFee
        {
            get
            {
                return this.CnyStatistics.Where(t => t.Subject.Contains("标签")).Sum(t => t.HKDLeftPrice);
            }
        }
        /// <summary>
        /// 登记费
        /// </summary>
        public decimal HKDRegistrationFee
        {
            get
            {
                return this.CnyStatistics.Where(t => t.Subject == "登记费").Sum(t => t.HKDLeftPrice);
            }
        }
        /// <summary>
        /// 清关费
        /// </summary>
        public decimal HKDCustomClearFee
        {
            get
            {
                return this.CnyStatistics.Where(t => t.Subject == "清关费").Sum(t => t.HKDLeftPrice);
            }
        }
        /// <summary>
        /// 入仓费
        /// </summary>
        public decimal HKDEnterFee
        {
            get
            {
                return this.CnyStatistics.Where(t => t.Subject == "入仓费").Sum(t => t.HKDLeftPrice);
            }
        }
        /// <summary>
        /// 送货费
        /// </summary>
        public decimal HKDDeliveryFee
        {
            get
            {
                return this.CnyStatistics.Where(t => t.Subject.Contains("送货费")).Sum(t => t.HKDLeftPrice);
            }
        }
        /// <summary>
        /// 其它费用
        /// </summary>
        public decimal HKDOtherFee
        {
            get
            {
                return this.HKDLeftTotalPrice - this.HKDStockFee - this.HKDLabelFee - this.HKDRegistrationFee - this.HKDCustomClearFee - this.HKDEnterFee - this.HKDDeliveryFee;
            }
        }

        #endregion

        /// <summary>
        /// 收货方
        /// </summary>
        public string Consignee
        {
            get
            {
                return this.OrderOutput == null ? "" : this.OrderOutput.Waybill.Consignee.Company ?? this.OrderOutput.Waybill.Consignee.Contact;
            }
        }

        /// <summary>
        /// 出库方式
        /// </summary>
        public string TypeDec
        {
            get
            {
                return this.OrderOutput == null ? "" : this.OrderOutput.Waybill.Type.GetDescription();
            }
        }

        public string Region
        {
            get
            {
                if (this.OrderOutput == null)
                {
                    return string.Empty;
                }
                else
                {
                    var address = this.OrderOutput.Waybill.Consignee.Address;
                    if (address.Contains("中西"))
                    {
                        return "中西区";
                    }
                    else if (address.Contains("湾仔"))
                    {
                        return "湾仔区";
                    }
                    else if (address.Contains("东区"))
                    {
                        return "东区";
                    }
                    else if (address.Contains("南区"))
                    {
                        return "南区";
                    }
                    else if (address.Contains("油尖旺"))
                    {
                        return "油尖旺区";
                    }
                    else if (address.Contains("深水埗"))
                    {
                        return "深水埗区";
                    }
                    else if (address.Contains("九龙"))
                    {
                        return "九龙城区";
                    }
                    else if (address.Contains("黄大仙"))
                    {
                        return "黄大仙区";
                    }
                    else if (address.Contains("观塘"))
                    {
                        return "观塘区";
                    }
                    else if (address.Contains("荃湾"))
                    {
                        return "荃湾区";
                    }
                    else if (address.Contains("屯门"))
                    {
                        return "屯门区";
                    }
                    else if (address.Contains("元朗"))
                    {
                        return "元朗区";
                    }
                    else if (address.Contains("北区"))
                    {
                        return "北区";
                    }
                    else if (address.Contains("大埔"))
                    {
                        return "大埔区";
                    }
                    else if (address.Contains("西贡"))
                    {
                        return "西贡区";
                    }
                    else if (address.Contains("沙田"))
                    {
                        return "沙田区";
                    }
                    else if (address.Contains("葵青"))
                    {
                        return "葵青区";
                    }
                    else if (address.Contains("离岛"))
                    {
                        return "离岛区";
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
        }

        #endregion

        public BillItem()
        {

        }

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (var Reponsitory = new PvWsOrderReponsitory())
            {
                if (!Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.BillItems>().Any(item => item.ID == this.ID))
                {
                    this.ID = Guid.NewGuid().ToString();
                    Reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.BillItems()
                    {
                        ID = this.ID,
                        BillID = this.BillID,
                        OrderID = this.OrderID,
                    });
                }
                else
                {
                    Reponsitory.Update<Layers.Data.Sqls.PvWsOrder.BillItems>(new
                    {
                        this.BillID,
                        this.OrderID,
                    }, t => t.ID == this.ID);
                }
            }
        }
    }

}
