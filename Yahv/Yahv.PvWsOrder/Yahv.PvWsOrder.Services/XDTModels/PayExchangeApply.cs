using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.ClientViews;
using Yahv.PvWsOrder.Services.Extends;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Converters;
using Yahv.Utils.Serializers;

namespace Yahv.PvWsOrder.Services.XDTModels
{
    /// <summary>
    /// 付汇申请
    /// </summary>
    public class PayExchangeApply : IUnique
    {
        #region  属性

        public string ID { get; set; }

        /// <summary>
        /// 供应商中文名
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        ///供应商英文名
        /// </summary>
        public string SupplierEnglishName { get; set; }

        /// <summary>
        /// 供应商地址
        /// </summary>
        public string SupplierAddress { get; set; }

        /// <summary>
        /// 供应商银行账号
        /// </summary>
        public string BankAccount { get; set; }

        /// <summary>
        /// 银行地址
        /// </summary>
        public string BankAddress { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 银行代号
        /// </summary>
        public string SwiftCode { get; set; }

        /// <summary>
        /// ABA（付美国必填）
        /// </summary>
        public string ABA { get; set; }

        /// <summary>
        /// IBAN（付欧盟必填）
        /// </summary>
        public string IBAN { get; set; }

        /// <summary>
        /// 是否垫款(0-垫款, 1-不垫款)
        /// </summary>
        public int IsAdvanceMoney { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 税率类型
        /// </summary>
        public string ExchangeRateType { get; set; }

        public decimal ExchangeRate { get; set; }

        /// <summary>
        /// 支付类型
        /// </summary>
        public string PaymentType { get; set; }

        /// <summary>
        /// 期望付汇日期
        /// </summary>
        public DateTime? ExpectPayDate { get; set; }

        /// <summary>
        /// 结算日期
        /// </summary>
        public string SettlemenDate { get; set; }

        /// <summary>
        ///   其他相关资料
        /// </summary>
        public string OtherInfo { get; set; }

        /// <summary>
        /// 订单项
        /// </summary>
        public UnPayExchangeOrderItem[] UnPayExchangeOrders { get; set; }

        /// <summary>
        /// 文件
        /// </summary>
        public CenterFileDescription[] PayExchangeApplyFiles { get; set; }

        public string ClientID { get; set; }

        public string UserID { get; set; }

        public string Summary { get; set; }

        public JMessage ResponseData { get; set; }


        /// <summary>
        /// 代付款手续费类型
        /// </summary>
        public string HandlingFeePayerType { get; set; }

        /// <summary>
        /// 手续费（美元）
        /// </summary>
        public decimal HandlingFee { get; set; }

        /// <summary>
        /// 美元实时汇率
        /// </summary>
        public decimal USDRate { get; set; }

        #endregion

        /// <summary>
        /// 提交付汇申请
        /// </summary>
        /// <returns></returns>
        public void SubmitApply()
        {
            this.ResponseData = this.XDTPayExchange().JsonTo<JMessage>();
            if (this.ResponseData.success)
            {
                using (Layers.Data.Sqls.PvCenterReponsitory reponsitory = new Layers.Data.Sqls.PvCenterReponsitory())
                {
                    if (this.PayExchangeApplyFiles != null)
                    {
                        //上传文件
                        new CenterFilesView().Upload(this.PayExchangeApplyFiles.Where(item => item.ID == null).Select(item => new CenterFileDescription
                        {
                            CustomName = item.CustomName,
                            Url = item.Url,
                            AdminID = item.AdminID,
                            ApplicationID = this.ResponseData.data,
                            Type = item.Type,
                            ClientID = item.ClientID
                        }).ToArray());
                        var originFiles = this.PayExchangeApplyFiles.Where(item => item.ID != null).ToArray();
                        foreach (var file in originFiles)
                        {
                            reponsitory.Insert(new Layers.Data.Sqls.PvCenter.FilesDescription
                            {
                                ID = "F" + Guid.NewGuid().ToString(),
                                ApplicationID = this.ResponseData.data,
                                CustomName = file.CustomName,
                                Type = file.Type,
                                Url = file.Url,
                                CreateDate = DateTime.Now,
                                ClientID = file.ClientID,
                                AdminID = file.AdminID,
                                Status = (int)FileDescriptionStatus.Normal,
                            });
                        }
                    }
                    foreach (var item in this.UnPayExchangeOrders)
                    {
                        var payStatus = PayExchangeStatus.All;
                        var leftAmount = item.DeclarePrice - item.PaidExchangeAmount; //剩余未支付金额
                        if (leftAmount > item.CurrentPaidAmount)
                        {
                            payStatus = PayExchangeStatus.Partial;
                        }
                        #region 付汇状态
                        //删除状态
                        reponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
                        {
                            IsCurrent = false,
                        }, c => c.MainID == item.OrderID && c.Type == (int)OrderStatusType.RemittanceStatus);
                        //新增状态
                        reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder()
                        {
                            ID = Guid.NewGuid().ToString(),
                            MainID = item.OrderID,
                            Type = (int)OrderStatusType.RemittanceStatus,
                            Status = (int)payStatus,
                            CreateDate = DateTime.Now,
                            CreatorID = this.UserID,
                            IsCurrent = true,
                        });
                        #endregion
                    }
                }
            }
        }
    }

    /// <summary>
    /// 芯达通文件对象
    /// </summary>
    public class XDTFile
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件后缀
        /// </summary>
        public string FileFormat { get; set; }

        /// <summary>
        /// URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string Type { get; set; }
    }

    /// <summary>
    /// 订单项
    /// </summary>
    public class UnPayExchangeOrderItem
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 报关金额
        /// </summary>
        public decimal DeclarePrice { get; set; }

        /// <summary>
        /// 已付汇金额
        /// </summary>
        public decimal PaidExchangeAmount { get; set; }

        /// <summary>
        /// 当前付汇金额
        /// </summary>
        public decimal CurrentPaidAmount { get; set; }
    }
}
