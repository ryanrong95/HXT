using Layers.Data.Sqls;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.UI.WebControls;
using Yahv.Linq;
using Yahv.Linq.Generic;
using Yahv.PvWsOrder.Services.ClientViews;
using Yahv.PvWsOrder.Services.Extends;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Underly.Attributes;
using Yahv.Utils.Serializers;

namespace Yahv.PvWsOrder.Services.XDTClientView
{
    public class MyPayExchangeApplies : UniqueView<UserPayExchangeApply, ScCustomReponsitory>
    {
        IUser user;

        private MyPayExchangeApplies()
        {

        }

        /// <summary>
        /// 带参数的构造函数
        /// </summary>
        /// <param name="User">用户</param>
        public MyPayExchangeApplies(IUser User)
        {
            this.user = User;
        }


        public MyPayExchangeApplies(IUser User, ScCustomReponsitory reponsitory, IQueryable<UserPayExchangeApply> IQuery) : base(reponsitory, IQuery)
        {
            this.user = User;
        }

        /// <summary>
        /// 获取结果集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<UserPayExchangeApply> GetIQueryable()
        {
            var payapplies = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.PayExchangeApplies>().Where(item => item.Status == (int)GeneralStatus.Normal).Where(item => item.ClientID == this.user.XDTClientID);
            //if (this.user.IsMain)
            //{
            //    payapplies = payapplies.Where(item => item.ClientID == this.user.XDTClientID);
            //}
            //else
            //{
            //    payapplies = payapplies.Where(item => item.UserID == this.user.ID);
            //}
            return from payApply in payapplies
                   select new UserPayExchangeApply
                   {
                       ID = payApply.ID,
                       ClientID = payApply.ClientID,
                       UserID = payApply.UserID,
                       SupplierName = payApply.SupplierName,
                       SupplierEnglishName = payApply.SupplierEnglishName,
                       SupplierAddress = payApply.SupplierAddress,
                       BankName = payApply.BankName,
                       BankAccount = payApply.BankAccount,
                       BankAddress = payApply.BankAddress,
                       SwiftCode = payApply.SwiftCode,
                       ABA = payApply.ABA,
                       IBAN = payApply.IBAN,
                       Currency = payApply.Currency,
                       ExchangeRate = payApply.ExchangeRate,
                       ExchangeRateType = (ExchangeRateType)payApply.ExchangeRateType,
                       ExpectPayDate = payApply.ExpectPayDate,
                       SettlemenDate = payApply.SettlemenDate,
                       PayExchangeApplyStatus = (PayExchangeApplyStatus)payApply.PayExchangeApplyStatus,
                       PaymentType = (PaymentType)payApply.PaymentType,
                       OtherInfo = payApply.OtherInfo,
                       Status = payApply.Status,
                       CreateDate = payApply.CreateDate,
                       UpdateDate = payApply.UpdateDate,
                       Summary = payApply.Summary,
                       FatherID = payApply.FatherID,
                       HandlingFeePayerType = payApply.HandlingFeePayerType,
                       HandlingFee = payApply.HandlingFee,
                       USDRate = payApply.USDRate,
                   };
        }


        /// <summary>
        /// 根据订单ID查询
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public MyPayExchangeApplies SearchByOrderID(string OrderID)
        {
            var applyitemview = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.PayExchangeApplyItems>().Where(item => item.Status == (int)GeneralStatus.Normal && item.OrderID == OrderID);

            var linq = from apply in this.IQueryable
                       join item in applyitemview on apply.ID equals item.PayExchangeApplyID
                       select apply;

            return new MyPayExchangeApplies(this.user, this.Reponsitory, linq);
        }

        public MyPayExchangeApplies SearchByMultiField(string multiField)
        {
            var applyitemview = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.PayExchangeApplyItems>();

            var linq = from apply in this.IQueryable
                       join applyitem in applyitemview on apply.ID equals applyitem.PayExchangeApplyID
                       where (applyitem.Status == (int)GeneralStatus.Normal && applyitem.OrderID == multiField)
                        || (apply.SupplierName.Contains(multiField) || apply.SupplierEnglishName.Contains(multiField))
                        || (apply.BankAccount.Contains(multiField))
                       select apply;

            linq = linq.Distinct();

            return new MyPayExchangeApplies(this.user, this.Reponsitory, linq);
        }


        /// <summary>
        /// 根据传入参数获取订单数据
        /// </summary>
        /// <param name="expressions"></param>
        /// <returns></returns>
        virtual protected IQueryable<UserPayExchangeApply> GetData(LambdaExpression[] expressions)
        {
            var Applies = this.IQueryable;
            if (expressions != null)
            {
                foreach (var expression in expressions)
                {
                    Applies = Applies.Where(expression as Expression<Func<UserPayExchangeApply, bool>>);
                }
            }
            return Applies;
        }

        /// <summary>
        /// 根据传入参数和查询条件获取数据
        /// </summary>
        /// <param name="expressions"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        public PageList<UserPayExchangeApplyExtends> GetPageListOrders(LambdaExpression[] expressions, int PageSize, int PageIndex)
        {
            var Applies = this.GetData(expressions).OrderByDescending(item => item.CreateDate);
            int total = Applies.Count();
            var linq = Applies.Skip(PageSize * (PageIndex - 1)).Take(PageSize).ToArray();

            var items = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.PayExchangeApplyItems>().Where(item => item.Status == (int)GeneralStatus.Normal).
                Where(item => linq.Select(a => a.ID).Contains(item.PayExchangeApplyID)).ToArray();
            var applyusers = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Users>().Where(item => linq.Select(a => a.UserID).Contains(item.ID)).ToArray();
            var CompetedLog = new PayExchangeApplyLogsView(this.Reponsitory).Where(item => linq.Select(a => a.UserID).Contains(item.PayExchangeApplyID)).
                FirstOrDefault(item => item.PayExchangeApplyStatus == PayExchangeApplyStatus.Completed);


            var result = from payApply in linq
                         join item in items on payApply.ID equals item.PayExchangeApplyID into _items
                         join userTable in applyusers on payApply.UserID equals userTable.ID into users
                         from user in users.DefaultIfEmpty()
                         select new UserPayExchangeApplyExtends
                         {
                             Apply = payApply,
                             User = user == null ? null : new ApplyUser
                             {
                                 ID = user.ID,
                                 Name = user.Name,
                                 RealName = user.RealName
                             },
                             Items = _items.Select(item => new PayExchangeApplyItem
                             {
                                 ID = item.ID,
                                 PayExchangeApplyID = item.PayExchangeApplyID,
                                 OrderID = item.OrderID,
                                 Amount = item.Amount,
                                 Status = item.Status,
                                 CreateDate = item.CreateDate,
                                 UpdateDate = item.UpdateDate,
                                 Summary = item.Summary
                             }).ToArray(),
                             CompetedLog = CompetedLog,
                         };

            return new PageList<UserPayExchangeApplyExtends>(PageIndex, PageSize, result, total);
        }

        /// <summary>
        /// 获取详情数据
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public UserPayExchangeApplyExtends GetDetailDataByID(string ID)
        {
            var ApplyFiles = new CenterFilesView().Where(item => item.ApplicationID == ID).ToArray();

            var extends = new UserPayExchangeApplyExtends();
            extends.Apply = this[ID];
            extends.Items = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.PayExchangeApplyItems>().
                Where(item => item.Status == (int)GeneralStatus.Normal && item.PayExchangeApplyID == ID).Select(item => new PayExchangeApplyItem
                {
                    ID = item.ID,
                    PayExchangeApplyID = item.PayExchangeApplyID,
                    OrderID = item.OrderID,
                    Amount = item.Amount,
                    Status = item.Status,
                    CreateDate = item.CreateDate,
                    UpdateDate = item.UpdateDate,
                    Summary = item.Summary
                }).ToArray();
            extends.PayExchangeFile = ApplyFiles.FirstOrDefault(item => item.Type == (int)FileType.PayExchange);
            extends.PIFiles = ApplyFiles.Where(item => item.Type == (int)FileType.PIFiles).ToArray();
            extends.CompetedLog = new PayExchangeApplyLogsView(this.Reponsitory).Where(item => item.PayExchangeApplyID == ID).
                FirstOrDefault(item => item.PayExchangeApplyStatus == PayExchangeApplyStatus.Completed);

            return extends;
        }

        /// <summary>
        /// 根据订单查付汇申请, 并且只显示 PayExchangeApply 中 FatherID 为 NULL 的
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public PayExchangeApplyItemExtend[] GetAppliesByOrderID(string orderID)
        {
            return (from items in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.PayExchangeApplyItems>().
                Where(item => item.Status == (int)GeneralStatus.Normal && item.OrderID == orderID)
                    join apply in this.GetIQueryable() on items.PayExchangeApplyID equals apply.ID
                    where apply.FatherID == null
                    select new PayExchangeApplyItemExtend
                    {
                        ID = items.ID,
                        PayExchangeApplyID = items.PayExchangeApplyID,
                        OrderID = items.OrderID,
                        Amount = items.Amount,
                        Status = items.Status,
                        CreateDate = items.CreateDate,
                        UpdateDate = items.UpdateDate,
                        Summary = items.Summary,
                        PayExchangeApplyStatus = apply.PayExchangeApplyStatus,
                        SupplierName = apply.SupplierName
                    }).ToArray();
        }
    }

    /// <summary>
    /// 付汇申请项扩展
    /// </summary>
    public class PayExchangeApplyItemExtend : PayExchangeApplyItem
    {
        /// <summary>
        /// 供应商名称
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// 申请状态
        /// </summary>
        public PayExchangeApplyStatus PayExchangeApplyStatus { get; set; }
    }


    public class UserPayExchangeApplyExtends
    {
        public UserPayExchangeApply Apply { get; set; }

        public ApplyUser User { get; set; }

        public PayExchangeApplyItem[] Items { get; set; }

        public CenterFileDescription[] PIFiles { get; set; }

        public CenterFileDescription PayExchangeFile { get; set; }

        public PayExchangeLog CompetedLog { get; set; }



        /// <summary>
        /// 删除付汇申请
        /// </summary>
        /// <returns></returns>
        public void DeleteApply(string userID)
        {
            this.Apply.ResponseData = this.Apply.XDTDeletePayExchange(userID).JsonTo<JMessage>();
            if (this.Apply.ResponseData.success)
            {
                using (Layers.Data.Sqls.PvCenterReponsitory reponsitory = new Layers.Data.Sqls.PvCenterReponsitory())
                {
                    using (Layers.Data.Sqls.ScCustomReponsitory reponsitory1 = new Layers.Data.Sqls.ScCustomReponsitory())
                    {
                        foreach (var item in this.Items)
                        {
                            var order = reponsitory1.ReadTable<Layers.Data.Sqls.ScCustoms.Orders>().Where(c => c.ID == item.OrderID).FirstOrDefault();
                            var payStatus = PayExchangeStatus.UnPay;
                            if (order.PaidExchangeAmount > 0)
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
                                CreatorID = userID, //this.Apply.UserID,
                                IsCurrent = true,
                            });
                            #endregion
                        }

                    }
                }
            }
        }
    }

    public class UserPayExchangeApply : IUnique
    {
        public string ID { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        public string ClientName
        {
            get
            {
                using (ScCustomReponsitory reponsitory = new ScCustomReponsitory())
                {
                    var linq = from client in reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Clients>()
                               join company in reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Companies>() on client.CompanyID equals company.ID
                               where client.ID == this.ClientID
                               select company.Name;
                    return linq.SingleOrDefault();
                }
            }
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// 供应商英文名称
        /// </summary>
        public string SupplierEnglishName { get; set; }

        /// <summary>
        /// 供应商地址
        /// </summary>
        public string SupplierAddress { get; set; }

        /// <summary>
        /// 银行账户
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
        /// 银行国际代码
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
        /// 使用的汇率类型
        /// </summary>
        public ExchangeRateType ExchangeRateType { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 汇率
        /// </summary>
        public decimal ExchangeRate { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        public PaymentType PaymentType { get; set; }

        /// <summary>
        /// 期望付款日期
        /// </summary>
        public DateTime? ExpectPayDate { get; set; }

        /// <summary>
        /// 结算截止日期
        /// </summary>
        public DateTime SettlemenDate { get; set; }

        /// <summary>
        /// 其它资料
        /// </summary>
        public string OtherInfo { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 数据状态
        /// //TODO:需要在框架中定义枚举
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 描述，备注
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// FatherID
        /// </summary>
        public string FatherID { get; set; }


        /// <summary>
        /// 代付款手续费类型
        /// </summary>
        public int? HandlingFeePayerType { get; set; }

        /// <summary>
        /// 手续费（美元）
        /// </summary>
        public decimal? HandlingFee { get; set; }

        /// <summary>
        /// 美元实时汇率
        /// </summary>
        public decimal? USDRate { get; set; }


        public JMessage ResponseData { get; set; }

        /// <summary>
        /// 付汇申请状态
        /// </summary>
        public PayExchangeApplyStatus PayExchangeApplyStatus { get; set; }
    }

    public class PayExchangeApplyItem
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 数据状态
        /// //TODO:需要在框架中定义枚举
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 描述，备注
        /// </summary>
        public string Summary { get; set; }

        public string PayExchangeApplyID { get; set; }

        public string OrderID { get; set; }

        public decimal Amount { get; set; }

        /// <summary>
        /// 报关总价
        /// </summary>
        public decimal DeclarePrice { get; set; }

        /// <summary>
        /// 已申请付汇总价
        /// </summary>
        public decimal PaidExchangeAmount { get; set; }

        /// <summary>
        /// 应收货款总额
        /// </summary>
        public decimal ReceivableAmount { get; set; }

        /// <summary>
        /// 实收货款总额
        /// </summary>
        public decimal ReceivedAmount { get; set; }

        /// <summary>
        /// 申请状态
        /// </summary>
        public ApplyItemStatus ApplyStatus { get; set; }
    }

    public class ApplyUser
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public string RealName { get; set; }
    }


    /// <summary>
    /// 付款方式
    /// </summary>
    public enum PaymentType
    {
        /// <summary>
        /// 支票
        /// </summary>
        [Description("支票")]
        Check = 1,

        /// <summary>
        /// 现金
        /// </summary>
        [Description("现金")]
        Cash = 2,

        /// <summary>
        /// 转账
        /// </summary>
        [Description("转账")]
        TransferAccount = 3,
    }

    /// <summary>
    /// 付汇申请状态
    /// </summary>
    public enum PayExchangeApplyStatus
    {
        /// <summary>
        /// 待审核
        /// </summary>
        [Description("待审核")]
        Auditing = 1,

        /// <summary>
        /// 已审核
        /// </summary>
        [Description("已审核")]
        Audited = 2,

        /// <summary>
        /// 已审批
        /// </summary>
        [Description("已审批")]
        Approvaled = 3,

        /// <summary>
        /// 已取消
        /// </summary>
        [Description("已取消")]
        Cancled = 4,

        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        Completed = 5
    }

    /// <summary>
    /// 付汇申请 Item 的状态
    /// </summary>
    public enum ApplyItemStatus
    {
        /// <summary>
        /// 申请中
        /// </summary>
        Appling = 1,

        /// <summary>
        /// 申请成功
        /// </summary>
        Applied = 2,
    }
}
