using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class VoyageDetailsViewNew : QueryView<Models.VoyageDetailViewModel, ScCustomsReponsitory>
    {
        public VoyageDetailsViewNew()
        {
        }

        internal VoyageDetailsViewNew(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected VoyageDetailsViewNew(ScCustomsReponsitory reponsitory, IQueryable<Models.VoyageDetailViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Models.VoyageDetailViewModel> GetIQueryable()
        {

            var packings = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Packings>();
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var decHeadsView = new Views.DecHeadsView(this.Reponsitory).Where(dc => dc.CusDecStatus != "04");

            var result = from entity in packings
                         join order in orders on entity.OrderID equals order.ID
                         join client in clients on order.ClientID equals client.ID
                         join decHead in decHeadsView on order.ID equals decHead.OrderID
                         orderby entity.OrderID, entity.BoxIndex
                         where entity.Status==(int)Enums.Status.Normal
                         select new Models.VoyageDetailViewModel
                         {
                             ID = entity.ID,
                             VoyageNo = decHead.VoyNo,
                             OrderID = order.ID,
                             ClientID = order.ClientID,
                             ClientCode = client.ClientCode,
                             ClientType = (Enums.ClientType)client.ClientType,
                             CompanyID = client.CompanyID,
                             PackingDate = entity.PackingDate,
                             BoxIndex = entity.BoxIndex,

                         };

            return result;
        }

        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Models.VoyageDetailViewModel> iquery = this.IQueryable.Cast<Models.VoyageDetailViewModel>().OrderByDescending(item => item.PackingDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            var ienum_EntryNotices = iquery.ToArray();

            var orderIDs = ienum_EntryNotices.Select(t => t.OrderID).Distinct();
            var orderItemsCount = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()
                                  where c.Status == (int)Enums.Status.Normal && orderIDs.Contains(c.OrderID)
                                  group c by c.OrderID into d
                                  select new
                                  {
                                      OrderID = d.Key,
                                      ItemsCount = d.Count()
                                  };
            var ienmus_orderItemsCount = orderItemsCount.ToArray();

            var companyIDs = ienum_EntryNotices.Select(t => t.CompanyID).Distinct();
            var companyInfo = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>()
                                 where companyIDs.Contains(c.ID)
                                 select new
                                 {
                                     ID = c.ID,
                                     Name = c.Name
                                 };
            var ienums_companyInfo = companyInfo.ToArray();

            var results = from entity in ienum_EntryNotices
                          join orderItem in ienmus_orderItemsCount on entity.OrderID equals orderItem.OrderID
                          join company in ienums_companyInfo on entity.CompanyID equals company.ID
                          select new Models.VoyageDetailViewModel
                          {
                              ID = entity.ID,
                              VoyageNo = entity.VoyageNo,
                              OrderID = entity.OrderID,
                              ClientID = entity.ClientID,
                              ClientCode = entity.ClientCode,
                              ClientType = entity.ClientType,
                              CompanyID = entity.CompanyID,
                              PackingDate = entity.PackingDate,
                              BoxIndex = entity.BoxIndex,
                              ClientName = company.Name,
                              ItemCount = orderItem.ItemsCount,
                          };

            List<Models.GroupVoyageDetailViewModel> groupedResult = new List<Models.GroupVoyageDetailViewModel>();
            foreach(var item in orderItemsCount)
            {
                var summaryInfo = results.Where(t => t.OrderID == item.OrderID).FirstOrDefault();

                Models.GroupVoyageDetailViewModel groupInfo = new Models.GroupVoyageDetailViewModel();
                groupInfo.BoxInfo = new List<Models.GroupBox>();
                groupInfo.ID = summaryInfo.ID;
                groupInfo.VoyageNo = summaryInfo.VoyageNo;
                groupInfo.OrderID = item.OrderID;
                groupInfo.ClientID = summaryInfo.ClientID;
                groupInfo.ClientCode = summaryInfo.ClientCode;
                groupInfo.ClientType = summaryInfo.ClientType;
                groupInfo.ClientName = summaryInfo.ClientName;
                groupInfo.ItemCount = item.ItemsCount;

                var boxes = results.Where(t => t.OrderID == item.OrderID).ToList();
                foreach(var box in boxes)
                {
                    Models.GroupBox groupBox = new Models.GroupBox();
                    groupBox.BoxIndex = box.BoxIndex;
                    groupBox.PackingDate = box.PackingDate.ToString("yyyy-MM-dd");
                    groupInfo.BoxInfo.Add(groupBox);
                }
                groupedResult.Add(groupInfo);
            }

         

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = groupedResult.ToArray(),
            };
        }

        public VoyageDetailsViewNew SearchByVoyageNo(string voyageNo)
        {
            var linq = from query in this.IQueryable
                       where query.VoyageNo == voyageNo
                       select query;

            var view = new VoyageDetailsViewNew(this.Reponsitory, linq);
            return view;
        }

        public VoyageDetailsViewNew SearchByClientType(Enums.ClientType clientType)
        {
            var linq = from query in this.IQueryable
                       where query.ClientType == clientType
                       select query;

            var view = new VoyageDetailsViewNew(this.Reponsitory, linq);
            return view;
        }
    }
}
