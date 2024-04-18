using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yahv.Services;
using Yahv.Services.Enums;
using Yahv.Underly;
using Yahv.XdtData.Import.Extends;
using Yahv.XdtData.Import.Models;

namespace Yahv.XdtData.Import.Services
{
    /// <summary>
    /// 数据更新
    /// </summary>
    public class UpdateService
    {
        static object locker = new object();
        static UpdateService current;

        Queue<string> xdtMainOrderIDsQueue;
        Queue<CgDelcareSZPrice> szPricesQueue;

        private UpdateService()
        {
            this.xdtMainOrderIDsQueue = new Queue<string>();
            this.szPricesQueue = new Queue<CgDelcareSZPrice>();
        }

        public static UpdateService Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new UpdateService();
                        }
                    }
                }

                return current;
            }
        }

        public void AddMainOrderIDs(params string[] mainOrderIDs)
        {
            lock (this)
            {
                foreach (var id in mainOrderIDs)
                {
                    if (!this.xdtMainOrderIDsQueue.Contains(id))
                    {
                        this.xdtMainOrderIDsQueue.Enqueue(id);
                    }
                }
            }
        }

        public void Start()
        {
            Task.Factory.StartNew(() =>
            {
                Thread.CurrentThread.IsBackground = false;
                this.Query();
            });

            Task.Factory.StartNew(() =>
            {
                Thread.CurrentThread.IsBackground = false;
                this.Update();
            });
        }

        private void Query()
        {
            while (true)
            {
                if (this.xdtMainOrderIDsQueue.Count > 0)
                {
                    var xdtMainOrderID = this.xdtMainOrderIDsQueue.Dequeue();
                    var szPrice = SzPriceQuery(xdtMainOrderID);
                    this.szPricesQueue.Enqueue(szPrice);
                }

                Thread.Sleep(5000);
            }
        }

        private void Update()
        {
            while (true)
            {
                if (this.szPricesQueue.Count > 0)
                {
                    var szPrice = this.szPricesQueue.Dequeue();
                    SzPriceUpdate(szPrice);
                }

                Thread.Sleep(5000);
            }
        }

        /// <summary>
        /// 查询计算深圳库房商品账
        /// </summary>
        /// <param name="xdtMainOrderID">芯达通主订单ID</param>
        /// <returns></returns>
        private CgDelcareSZPrice SzPriceQuery(string xdtMainOrderID)
        {
            Console.WriteLine($"查询计算深圳库房商品账, 芯达通主订单ID: {xdtMainOrderID}");

            using (var reponsitory = new PvWmsRepository())
            {
                var szPrice = new CgDelcareSZPrice();
                szPrice.Items = new List<CgDelcareSZPriceItem>();

                var szPriceItems = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CgPvWmsDeclareSzPricesTopView>()
                    .Where(item => item.MainOrderId == xdtMainOrderID).ToArray();

                szPrice.Items.AddRange(szPriceItems.Select(item => new CgDelcareSZPriceItem()
                {
                    OrderID = item.MainOrderId,
                    TinyOrderID = item.TinyOrderID,
                    OrderItemID = item.OrderItemID,
                    InUnitPrice = (((item.DeclTotal * item.CustomsExchangeRate.Value) + ((item.DeclTotal * item.CustomsExchangeRate.Value).ToRound(0) * item.ReceiptRate)) / item.Qty).ToRound(7),
                    OutUnitPrice = (((item.DeclTotal * item.CustomsExchangeRate.Value) + ((item.DeclTotal * item.CustomsExchangeRate.Value).ToRound(0) * item.ReceiptRate)) / item.Qty).ToRound(7)
                }).ToList());

                return szPrice;
            }
        }

        /// <summary>
        /// 更新深圳库房商品账
        /// </summary>
        /// <param name="szPrice"></param>
        private void SzPriceUpdate(CgDelcareSZPrice szPrice)
        {
            Console.WriteLine($"更新深圳库房商品账");

            var tmpInputList = new List<TmpInput>();
            var tmpOutputList = new List<TmpOutput>();

            using (var reponsitory = new PvWmsRepository())
            {
                var inputs = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>();
                var notices = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>();
                var outputs = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>();

                var updateInputs = from input in inputs
                                   join notice in notices on input.ID equals notice.InputID
                                   where (notice.Type == (int)CgNoticeType.Enter && notice.WareHouseID.StartsWith(WhSettings.SZ.ID))
                                   select new
                                   {
                                       ID = input.ID,
                                       OrderID = input.OrderID,
                                       TinyOrderID = input.TinyOrderID,
                                       ItemID = input.ItemID,
                                   };
                var updateOutputs = from output in outputs
                                    join notice in notices on output.ID equals notice.OutputID
                                    where (notice.Type == (int)CgNoticeType.Out && notice.WareHouseID.StartsWith(WhSettings.SZ.ID))
                                    select new
                                    {
                                        ID = output.ID,
                                        OrderID = output.OrderID,
                                        TinyOrderID = output.TinyOrderID,
                                        ItemID = output.ItemID,
                                    };

                var orderItemIDs = szPrice.Items.Select(item => item.OrderItemID).ToArray();
                var updateInputIDs = updateInputs.Where(t => orderItemIDs.Contains(t.ItemID)).Select(t => new { t.ID, t.ItemID }).ToArray();
                var updateOutputIDs = updateOutputs.Where(t => orderItemIDs.Contains(t.ItemID)).Select(t => new { t.ID, t.ItemID }).ToArray();

                foreach (var item in szPrice.Items)
                {
                    tmpInputList.AddRange(updateInputIDs.Where(t => t.ItemID == item.OrderItemID).Select(t => new TmpInput()
                    {
                        ID = t.ID,
                        UnitPrice = item.InUnitPrice,
                        Currency = (int)Currency.CNY,
                    }));

                    tmpOutputList.AddRange(updateOutputIDs.Where(t => t.ItemID == item.OrderItemID).Select(t => new TmpOutput()
                    {
                        ID = t.ID,
                        Price = item.OutUnitPrice,
                        Currency = (int)Currency.CNY,
                    }));
                }
            }

            using (var conn = Connections.ConnManager.Current.PvWms)
            {
                conn.Open();
                conn.BulkInsertWithTempTable(tmpInputList);
                conn.BulkInsertWithTempTable(tmpOutputList);
            }
        }
    }

    public class CgDelcareSZPrice
    {
        public List<CgDelcareSZPriceItem> Items { get; set; }
    }

    public class CgDelcareSZPriceItem
    {
        public string OrderID { get; set; }

        public string TinyOrderID { get; set; }

        public string OrderItemID { get; set; }

        public decimal InUnitPrice { get; set; }

        public decimal OutUnitPrice { get; set; }
    }
}
