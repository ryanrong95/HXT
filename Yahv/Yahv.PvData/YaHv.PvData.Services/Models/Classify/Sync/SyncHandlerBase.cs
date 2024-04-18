using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.PvData.Services.Extends;
using YaHv.PvData.Services.Handlers;

namespace YaHv.PvData.Services.Models
{
    /// <summary>
    /// 数据同步基类
    /// </summary>
    public abstract class SyncHandlerBase
    {
        protected IEnumerable<OrderedProduct> ops;

        protected event SyncingHandler Syncing;

        public SyncHandlerBase()
        {
            this.Syncing += ClassifiedPartNumbersEnter;
            this.Syncing += ProductsEnter;
        }

        public SyncHandlerBase For(params OrderedProduct[] ops)
        {
            this.ops = ops;
            return this;
        }

        /// <summary>
        /// 做数据同步
        /// </summary>
        public abstract void DoSync();

        virtual protected void OnSyncing()
        {
            if (this != null && this.Syncing != null)
            {
                this.Syncing(this, new SyncingEventArgs(this.ops));
            }
        }

        /// <summary>
        /// ClassifiedPartNumbers持久化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ClassifiedPartNumbersEnter(object sender, SyncingEventArgs e)
        {
            using (var reponsitory = new PvDataReponsitory())
            {
                foreach (var op in e.OrderedProducts)
                {
                    string cpnID = op.GenID();
                    if (!reponsitory.ReadTable<Layers.Data.Sqls.PvData.ClassifiedPartNumbers>().Any(t => t.ID == cpnID))
                    {
                        reponsitory.Insert(new Layers.Data.Sqls.PvData.ClassifiedPartNumbers()
                        {
                            ID = cpnID,
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
        }

        /// <summary>
        /// Products持久化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ProductsEnter(object sender, SyncingEventArgs e)
        {
            using (var reponsitory = new PvDataReponsitory())
            {
                foreach (var op in e.OrderedProducts)
                {
                    string productID = op.GenOtherID();
                    if (!reponsitory.ReadTable<Layers.Data.Sqls.PvData.Products>().Any(t => t.ID == productID))
                    {
                        reponsitory.Insert(new Layers.Data.Sqls.PvData.Products()
                        {
                            ID = productID,
                            PartNumber = op.PartNumber,
                            Manufacturer = op.Manufacturer,
                            CreateDate = op.CreateDate,
                        });
                    }
                }
            }
        }
    }
}
