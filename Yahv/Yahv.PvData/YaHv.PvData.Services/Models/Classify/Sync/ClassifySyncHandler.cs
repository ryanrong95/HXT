using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.PvData.Services.Extends;

namespace YaHv.PvData.Services.Models
{
    class AdminMap
    {
        public string ID;
        public string OriginID;
    }

    /// <summary>
    /// 归类数据同步
    /// </summary>
    public sealed class ClassifySyncHandler : SyncHandlerBase
    {
        #region 扩展属性

        AdminMap[] adminMaps;

        private AdminMap[] AdminMaps
        {
            get
            {
                if (this.adminMaps == null)
                {
                    using (var reponsitory = new PvbErmReponsitory())
                    {
                        this.adminMaps = reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Admins>().Select(item => new AdminMap
                        {
                            ID = item.ID,
                            OriginID = item.OriginID
                        }).ToArray();
                    }
                }

                return this.adminMaps;
            }
        }

        #endregion

        #region 扩展方法

        /// <summary>
        /// 是否是自动归类
        /// </summary>
        private bool IsAuto(string creatorID) => ConstConfig.XdtNpc.Equals(creatorID);

        /// <summary>
        /// 将芯达通AdminID转换为代仓储AdminID
        /// </summary>
        private string Map(string creatorID)
        {
            var admin = this.AdminMaps.FirstOrDefault(a => a.OriginID == creatorID);
            if (admin == null)
            {
                throw new Exception($"芯达通Admin【{creatorID}】未在PvbErm中配置！");
            }

            return admin.ID;
        }

        #endregion

        public ClassifySyncHandler() : base()
        {

        }

        public override void DoSync()
        {
            if (this.ops == null || this.ops.Count() == 0)
                return;

            base.OnSyncing();

            using (var reponsitory = new PvWsOrderReponsitory())
            {
                foreach (var op in this.ops)
                {
                    var item = reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItems>().SingleOrDefault(oi => oi.ID == op.ID);
                    if (item == null)
                        continue;

                    OrderItemEnter(op, reponsitory);
                    OrderItemChcdEnter(op, reponsitory);
                    OrderItemTermEnter(op, reponsitory);
                }
            }
        }

        private void OrderItemEnter(OrderedProduct op, PvWsOrderReponsitory reponsitory)
        {
            string productId = op.GenOtherID();
            var item = reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItems>().SingleOrDefault(oi => oi.ID == op.ID);
            if (item != null && item.ProductID != productId)
            {
                reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItems>(new
                {
                    ProductID = productId,
                    ModifyDate = DateTime.Now,
                }, oi => oi.ID == op.ID);

                //原来的数据插入历史日志表
                reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.Logs_OrderItems
                {
                    ID = Guid.NewGuid().ToString(),
                    OrderItemID = item.ID,
                    OrderID = item.OrderID,
                    TinyOrderID = item.TinyOrderID,
                    InputID = item.InputID,
                    OutputID = item.OutputID,
                    ProductID = item.ProductID,
                    CustomName = item.CustomName,
                    Origin = item.Origin,
                    DateCode = item.DateCode,
                    Quantity = item.Quantity,
                    Currency = item.Currency,
                    UnitPrice = item.UnitPrice,
                    Unit = item.Unit,
                    TotalPrice = item.TotalPrice,
                    CreateDate = item.CreateDate,
                    ModifyDate = item.ModifyDate,
                    GrossWeight = item.GrossWeight,
                    Volume = item.Volume,
                    Conditions = item.Conditions,
                    Status = item.Status,
                    IsAuto = item.IsAuto,
                    WayBillID = item.WayBillID
                });
            }
        }

        private void OrderItemChcdEnter(OrderedProduct op, PvWsOrderReponsitory reponsitory)
        {
            string cpnID = op.GenID();
            string adminID = Map(op.CreatorID);
            var itemChcd = reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItemsChcd>().SingleOrDefault(oic => oic.ID == op.ID);

            if (itemChcd == null)
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.OrderItemsChcd()
                {
                    ID = op.ID,
                    SecondAdminID = adminID,
                    SecondHSCodeID = cpnID,
                    SecondDate = DateTime.Now,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                });
            }
            else
            {
                reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItemsChcd>(new
                {
                    SecondAdminID = adminID,
                    SecondHSCodeID = cpnID,
                    SecondDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                }, oic => oic.ID == op.ID);

                //原来的数据插入历史日志表
                reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.Logs_OrderItemsChcd
                {
                    ID = Guid.NewGuid().ToString(),
                    OrderItemID = itemChcd.ID,
                    AutoHSCodeID = itemChcd.AutoHSCodeID,
                    AutoDate = itemChcd.AutoDate,
                    FirstAdminID = itemChcd.FirstAdminID,
                    FirstHSCodeID = itemChcd.FirstHSCodeID,
                    FirstDate = itemChcd.FirstDate,
                    SecondAdminID = itemChcd.SecondAdminID,
                    SecondHSCodeID = itemChcd.SecondHSCodeID,
                    SecondDate = itemChcd.SecondDate,
                    CustomHSCodeID = itemChcd.CustomHSCodeID,
                    CustomTaxCode = itemChcd.CustomTaxCode,
                    SysPriceID = itemChcd.SysPriceID,
                    CustomsPriceID = itemChcd.CustomsPriceID,
                    VATaxedPriceID = itemChcd.VATaxedPriceID,
                    CreateDate = itemChcd.CreateDate,
                    ModifyDate = itemChcd.ModifyDate,
                });
            }
        }

        private void OrderItemTermEnter(OrderedProduct op, PvWsOrderReponsitory reponsitory)
        {
            var itemTerm = reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItemsTerm>().SingleOrDefault(oit => oit.ID == op.ID);
            if (itemTerm == null)
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.OrderItemsTerm()
                {
                    ID = op.ID,
                    OriginRate = op.OriginATRate,
                    FVARate = ConstConfig.FVARate,
                    Ccc = op.Ccc,
                    Embargo = op.Embargo,
                    HkControl = op.HkControl,
                    Coo = op.Coo,
                    CIQ = op.CIQ,
                    CIQprice = op.CIQprice,
                    IsHighPrice = op.IsHighPrice,
                    IsDisinfected = op.IsDisinfected,
                });
            }
            else
            {
                reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItemsTerm>(new
                {
                    OriginRate = op.OriginATRate,
                    FVARate = ConstConfig.FVARate,
                    Ccc = op.Ccc,
                    Embargo = op.Embargo,
                    HkControl = op.HkControl,
                    Coo = op.Coo,
                    CIQ = op.CIQ,
                    CIQprice = op.CIQprice,
                    IsHighPrice = op.IsHighPrice,
                    IsDisinfected = op.IsDisinfected,
                }, oit => oit.ID == op.ID);

                //原来的数据插入历史日志表
                reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.Logs_OrderItemsTerm
                {
                    ID = Guid.NewGuid().ToString(),
                    OrderItemID = itemTerm.ID,
                    OriginRate = itemTerm.OriginRate,
                    FVARate = itemTerm.FVARate,
                    Ccc = itemTerm.Ccc,
                    Embargo = itemTerm.Embargo,
                    HkControl = itemTerm.HkControl,
                    Coo = itemTerm.Coo,
                    CIQ = itemTerm.CIQ,
                    CIQprice = itemTerm.CIQprice,
                    IsHighPrice = itemTerm.IsHighPrice,
                    IsDisinfected = itemTerm.IsDisinfected,
                });
            }
        }
    }
}
