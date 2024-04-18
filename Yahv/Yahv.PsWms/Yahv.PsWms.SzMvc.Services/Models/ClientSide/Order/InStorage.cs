using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PsWms.SzMvc.Services.Enums;
using Yahv.PsWms.SzMvc.Services.Notice;
using Yahv.Underly;

namespace Yahv.PsWms.SzMvc.Services.Models
{
    public class InStorage
    {
        /// <summary>
        /// 订单
        /// </summary>
        public Layers.Data.Sqls.PsOrder.Orders Order { get; set; }

        /// <summary>
        /// 订单项
        /// </summary>
        public Layers.Data.Sqls.PsOrder.OrderItems[] OrderItems { get; set; }

        /// <summary>
        /// 产品信息
        /// </summary>
        public Layers.Data.Sqls.PsOrder.Products[] Products { get; set; }

        /// <summary>
        /// 货运信息
        /// </summary>
        public Layers.Data.Sqls.PsOrder.OrderTransports OrderTransport { get; set; }

        /// <summary>
        /// 特殊要求
        /// </summary>
        public Layers.Data.Sqls.PsOrder.Requires[] Requires { get; set; }

        /// <summary>
        /// 提货单文件、装箱单文件
        /// </summary>
        public Layers.Data.Sqls.PsOrder.PcFiles[] Files { get; set; }

        /// <summary>
        /// 新增
        /// </summary>
        public void Insert(string trackerID)
        {
            using (PsOrderRepository repository = new PsOrderRepository(false))
            {
                repository.Insert(this.Order);
                repository.Insert(this.OrderItems);
                repository.Insert(this.Products);
                repository.Insert(this.OrderTransport);
                if (this.Requires != null && this.Requires.Length > 0)
                {
                    repository.Insert(this.Requires);
                }
                if (this.Files != null && this.Files.Length > 0)
                {
                    repository.Insert(this.Files);
                }

                repository.Submit();
            }

            Task.Run(() =>
            {
                try
                {
                    StorageInNoticeService noticeService = new StorageInNoticeService();
                    noticeService.Order = this.Order;
                    noticeService.OrderItems = this.OrderItems;
                    noticeService.Products = this.Products;
                    noticeService.OrderTransport = this.OrderTransport;
                    noticeService.Requires = this.Requires;
                    noticeService.Files = this.Files;
                    //noticeService.GenerateJson(trackerID);
                    noticeService.GenerateJsonNew(trackerID);
                    noticeService.SendNotice(this.Order.ID);
                    noticeService.GenerateJsonFile();
                    noticeService.SendFileInfo(this.Order.ID);
                }
                catch (Exception ex)
                {
                    using (PsOrderRepository repository = new PsOrderRepository())
                    {
                        repository.Insert(new Layers.Data.Sqls.PsOrder.Logs
                        {
                            ID = Guid.NewGuid().ToString("N"),
                            ActionName = LogAction.NewStorageInRevNotice.GetDescription(),
                            MainID = this.Order.ID,
                            Content = ex.Message,
                            CreateDate = DateTime.Now,
                        });
                    }
                }
            });
        }
    }
}
