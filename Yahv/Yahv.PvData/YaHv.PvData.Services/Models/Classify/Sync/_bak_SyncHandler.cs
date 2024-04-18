using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.PvData.Services.Extends;

namespace YaHv.PvData.Services.Models
{
    [Obsolete]
    public class SyncHandler
    {
        #region 扩展属性

        /// <summary>
        /// 是否是自动归类
        /// </summary>
        private bool IsAuto
        {
            get
            {
                return ConstConfig.XdtNpc.Equals(this.op.CreatorID);
            }
        }

        string adminID;

        /// <summary>
        /// 将芯达通AdminID转换为代仓储AdminID
        /// </summary>
        private string AdminID
        {
            get
            {
                if (this.adminID == null)
                {
                    using (var reponsitory = new PvbErmReponsitory())
                    {
                        var admin = reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Admins>().FirstOrDefault(a => a.OriginID == this.op.CreatorID);
                        if (admin == null)
                        {
                            throw new Exception($"芯达通Admin【{this.op.CreatorID}】未在PvbErm中配置！");
                        }
                        this.adminID = admin.ID;
                    }
                }

                return this.adminID;
            }
        }

        string cpnID;

        /// <summary>
        /// ClassifiedPartNumber ID
        /// </summary>
        private string CpnID
        {
            get
            {
                if (this.cpnID == null)
                {
                    this.cpnID = this.op.GenID();
                }

                return this.cpnID;
            }
        }

        #endregion

        #region 构造器

        private OrderedProduct op;

        internal SyncHandler(OrderedProduct op)
        {
            this.op = op;
        }

        #endregion

        /// <summary>
        /// 同步归类信息到代仓储订单
        /// </summary>
        public void SyncClassified()
        {
            using (var reponsitory = new PvWsOrderReponsitory())
            {
                var item = reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItems>().SingleOrDefault(oi => oi.ID == this.op.ID);
                if (item == null)
                    return;

                ClassifiedPartNumberEnter(op);

                #region OrderItemChcd 持久化

                var itemChcd = reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItemsChcd>().SingleOrDefault(oic => oic.ID == this.op.ID);
                if (itemChcd == null)
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.OrderItemsChcd()
                    {
                        ID = op.ID,
                        SecondAdminID = this.AdminID,
                        SecondHSCodeID = this.CpnID,
                        SecondDate = DateTime.Now,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                    });
                }
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItemsChcd>(new
                    {
                        SecondAdminID = this.AdminID,
                        SecondHSCodeID = this.CpnID,
                        SecondDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                    }, oic => oic.ID == op.ID);

                    OrderItemChcdLogEnter(itemChcd, reponsitory);
                }

                #endregion

                #region OrderItemTerm 持久化

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

                    OrderItemTermLogEnter(itemTerm, reponsitory);
                }

                #endregion
            }
        }

        /// <summary>
        /// 同步税费变更到代仓储订单
        /// </summary>
        public void SyncTaxChanged()
        {
            using (var reponsitory = new PvWsOrderReponsitory())
            {
                var item = reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItems>().SingleOrDefault(oi => oi.ID == this.op.ID);
                if (item == null)
                    return;

                ClassifiedPartNumberEnter(op);

                #region OrderItemChcd 持久化

                var itemChcd = reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItemsChcd>().SingleOrDefault(oic => oic.ID == this.op.ID);
                if (itemChcd != null)
                {
                    reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItemsChcd>(new
                    {
                        SecondHSCodeID = this.CpnID,
                        ModifyDate = DateTime.Now,
                    }, oic => oic.ID == op.ID);

                    OrderItemChcdLogEnter(itemChcd, reponsitory);
                }

                #endregion

                #region OrderItemTerm 持久化

                var itemTerm = reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItemsTerm>().SingleOrDefault(oit => oit.ID == op.ID);
                if (itemTerm != null)
                {
                    reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItemsTerm>(new
                    {
                        OriginRate = op.OriginATRate,
                    }, oit => oit.ID == op.ID);

                    OrderItemTermLogEnter(itemTerm, reponsitory);
                }

                #endregion
            }
        }

        private void ClassifiedPartNumberEnter(OrderedProduct op)
        {
            using (var reponsitory = new PvDataReponsitory())
            {
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvData.ClassifiedPartNumbers>().Any(t => t.ID == this.CpnID))
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvData.ClassifiedPartNumbers()
                    {
                        ID = this.CpnID,
                        PartNumber = op.PartNumber,
                        Manufacturer = op.Manufacturer,
                        HSCode = op.HSCode,
                        Name = op.TariffName,
                        LegalUnit1 = op.LegalUnit1,
                        LegalUnit2 = op.LegalUnit2,
                        VATRate = op.VATRate,
                        ImportPreferentialTaxRate = op.ImportPreferentialTaxRate,
                        ExciseTaxRate = op.ExciseTaxRate,
                        Elements = op.Elements,
                        CIQCode = op.CIQCode,
                        TaxCode = op.TaxCode,
                        TaxName = op.TaxName,
                        CreateDate = op.CreateDate,
                        OrderDate = op.UpdateDate
                    });
                }
            }
        }

        private void OrderItemChcdLogEnter(Layers.Data.Sqls.PvWsOrder.OrderItemsChcd itemChcd, PvWsOrderReponsitory reponsitory)
        {
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

        private void OrderItemTermLogEnter(Layers.Data.Sqls.PvWsOrder.OrderItemsTerm itemTerm, PvWsOrderReponsitory reponsitory)
        {
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
