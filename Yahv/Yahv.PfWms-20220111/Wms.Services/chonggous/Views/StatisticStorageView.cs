using Layers.Data.Sqls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Wms.Services.chonggous.Views
{
    public class StatisticStorageView : QueryView<object, PvWmsRepository>
    {
        public StatisticStorageView()
        {
        }

        public StatisticStorageView(PvWmsRepository repository) : base(repository)
        {
        }

        public StatisticStorageView(PvWmsRepository repository, IQueryable<object> iQueryable) : base(repository, iQueryable)
        {
        }

        protected override IQueryable<object> GetIQueryable()
        {
            //throw new NotImplementedException();
            var statisticStorageView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.StatisticsStorage>();

            var linqs = from storage in statisticStorageView
                        join client in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ClientsTopView>()
                        on storage.EnterCode equals client.EnterCode
                        orderby storage.CreateDate descending
                        select new StatisticStorage
                        {
                            EnterCode = storage.EnterCode,
                            Name = client.Name,
                            DateIndex = storage.DateIndex.ToString(),
                            CreateDate = storage.CreateDate,
                            Quantity = storage.Quantity,
                            Box1Quantity = storage.Box1Quantity,
                            Box2Quantity = storage.Box2Quantity,
                            Box3Quantity = storage.Box3Quantity,
                            AdminID = storage.AdminID,
                            WarehouseID = storage.WarehouseID,
                            ID = storage.ID,
                        };
            return linqs;

        }

        /// <summary>
        /// 获取分页及详情信息
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            var iquery = this.IQueryable.Cast<StatisticStorage>();
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            var ienum_storage = iquery.ToArray();

            string todayDateIndex = DateTime.Now.ToString("yyyyMMdd");

            var result = ienum_storage.Select(storage => new
            {
                EnterCode = storage.EnterCode,
                Name = storage.Name,
                DateIndex = storage.DateIndex.ToString(),
                CreateDate = storage.CreateDate,
                Quantity = storage.Quantity,
                Box1Quantity = storage.Box1Quantity,
                Box2Quantity = storage.Box2Quantity,
                Box3Quantity = storage.Box3Quantity,
                AdminID = storage.AdminID,
                WarehouseID = storage.WarehouseID,
                ID = storage.ID,                
                _disabled = (storage.DateIndex == todayDateIndex) ? false : true,
            }).ToArray();

            return new
            {
                Total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                Data = result,
            };

        }

        /// <summary>
        /// 根据用户提交过来的数据修改,大中小箱的个数
        /// </summary>
        /// <param name="jobjects"></param>
        public void Modify(JToken jobjects)
        {
            var statisticStorageView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.StatisticsStorage>();

            foreach (var jobject in jobjects)
            {
                var box1Quantity = jobject["Box1Quantity"].Value<int>();
                var box2Quantity = jobject["Box2Quantity"].Value<int>();
                var box3Quantity = jobject["Box3Quantity"].Value<int>();
                var quantity = jobject["Quantity"].Value<int>();
                var adminID = jobject["AdminID"].Value<string>();
                var id = jobject["ID"].Value<string>();
                if (statisticStorageView.Any(item => item.ID == id))
                {
                    this.Reponsitory.Update<Layers.Data.Sqls.PvWms.StatisticsStorage>(new
                    {
                        Box1Quantity = box1Quantity,
                        Box2Quantity = box2Quantity,
                        Box3Quantity = box3Quantity,
                        Quantity = quantity,
                    }, item => item.ID == id);
                }
            }
            
        }

        #region 搜索方法
        public StatisticStorageView SearchByClientName(string clientName)
        {
            var storageView = IQueryable.Cast<StatisticStorage>();
            var linq = from storage in storageView
                       where storage.Name.Contains(clientName)
                       select storage;

            var view = new StatisticStorageView(this.Reponsitory, linq)
            {
            };
            return view;
        }


        public StatisticStorageView SearchByEnterCode(string enterCode)
        {
            var storageView = IQueryable.Cast<StatisticStorage>();
            var linq = from storage in storageView
                       where storage.EnterCode == enterCode
                       select storage;

            var view = new StatisticStorageView(this.Reponsitory, linq)
            {
            };
            return view;
        }

        public StatisticStorageView SearchByCreateDate(DateTime start, DateTime end)
        {
            var storageView = IQueryable.Cast<StatisticStorage>();
            var linq = from storage in storageView
                       where storage.CreateDate >= start && storage.CreateDate < end.AddDays(1)
                       select storage;

            var view = new StatisticStorageView(this.Reponsitory, linq)
            {
            };
            return view;
        }

        #endregion

        #region Helper Class
        private class StatisticStorage
        {
            public string EnterCode { get; set; }
            public string DateIndex { get; set; }
            public string AdminID { get; set; }

            public int? Quantity { get; set; }

            public int Box1Quantity { get; set; }

            public int Box2Quantity { get; set; }

            public int Box3Quantity { get; set; }

            public DateTime CreateDate { get; set; }

            /// <summary>
            /// 客户名称
            /// </summary>
            public string Name { get; set; }

            public string WarehouseID { get; set; }

            /// <summary>
            /// StatisticStorage ID
            /// </summary>
            public string ID { get; set; }
        }
        #endregion
    }
}
