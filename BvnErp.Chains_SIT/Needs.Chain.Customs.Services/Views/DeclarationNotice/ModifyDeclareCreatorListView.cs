using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 调整制单人列表视图
    /// </summary>
    public class ModifyDeclareCreatorListView : QueryView<ModifyDeclareCreatorListViewModel, ScCustomsReponsitory>
    {
        public ModifyDeclareCreatorListView()
        {
        }

        protected ModifyDeclareCreatorListView(ScCustomsReponsitory reponsitory, IQueryable<ModifyDeclareCreatorListViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<ModifyDeclareCreatorListViewModel> GetIQueryable()
        {
            var declarationNotices = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarationNotices>();
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();

            var iQuery = from declarationNotice in declarationNotices
                         join order in orders on declarationNotice.OrderID equals order.ID
                         join client in clients on order.ClientID equals client.ID
                         join company in companies on client.CompanyID equals company.ID
                         where ((declarationNotice.Status == (int)Enums.DeclareNoticeStatus.UnDec && order.OrderStatus == (int)Enums.OrderStatus.QuoteConfirmed)
                                || declarationNotice.Status == (int)Enums.DeclareNoticeStatus.AllDec)
                             && order.Status == (int)Enums.Status.Normal
                             && client.Status == (int)Enums.Status.Normal
                             && company.Status == (int)Enums.Status.Normal
                         select new ModifyDeclareCreatorListViewModel
                         {
                             DeclarationNoticeID = declarationNotice.ID,
                             OrderID = order.ID,
                             CreateDate = declarationNotice.CreateDate,
                             Currency = order.Currency,
                             ClientID = client.ID,
                             ClientName = company.Name,
                             DecNoticeStatus = (Enums.DeclareNoticeStatus)declarationNotice.Status,
                             CreateDeclareAdminID = declarationNotice.CreateDeclareAdminID,
                         };
            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<ModifyDeclareCreatorListViewModel> iquery = this.IQueryable.Cast<ModifyDeclareCreatorListViewModel>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myDecNotices = iquery.ToArray();

            //获取报关通知的ID
            var decNoticesID = ienum_myDecNotices.Select(item => item.DeclarationNoticeID);

            //获取订单的ID
            var ordersID = ienum_myDecNotices.Select(item => item.OrderID);

            //客户ID
            var clientID = ienum_myDecNotices.Select(item => item.ClientID);

            var decNoticeVoyagesView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecNoticeVoyages>();
            var voyagesView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Voyages>();

            #region 获取运输批次

            var linq_dnVoyage = from dnVoyage in decNoticeVoyagesView
                                join voyage in voyagesView on dnVoyage.VoyageID equals voyage.ID
                                where dnVoyage.Status == (int)Enums.Status.Normal && decNoticesID.Contains(dnVoyage.DecNoticeID)
                                select new
                                {
                                    dnVoyage.DecNoticeID,
                                    VoyageID = voyage.ID,
                                    VoyageType = (Enums.VoyageType)voyage.Type
                                };

            var ienums_dnVoyage = linq_dnVoyage.ToArray();

            #endregion

            #region icgoo

            var icgooOrderView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.IcgooOrderMap>();

            var linq_icgoo = from map in icgooOrderView
                             where ordersID.Contains(map.OrderID)
                             select new
                             {
                                 map.OrderID,
                                 map.IcgooOrder,
                             };
            var ienums_icgoo = linq_icgoo.ToArray();

            #endregion

            #region 订单项目价格

            var orderItemsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();

            var linq_orderItems = from item in orderItemsView
                                  where item.Status == (int)Enums.Status.Normal && ordersID.Contains(item.OrderID)
                                  select new
                                  {
                                      OrderItemID = item.ID,
                                      item.Model,
                                      item.OrderID,
                                      item.Quantity,
                                      item.TotalPrice,
                                      item.GrossWeight
                                  };

            var ienums_orderItems = linq_orderItems.ToArray();
            var groups_orderItems = from item in ienums_orderItems
                                    group item by item.OrderID into groups
                                    select new
                                    {
                                        OrderID = groups.Key,
                                        TotalDeclarePrice = groups.Sum(t => t.TotalPrice),
                                        //TotalQty = groups.Sum(t => t.Quantity),
                                    };

            #endregion

            #region 跟单员

            var clientAdminsView = new ClientAdminsView(this.Reponsitory);

            var linq_declarant = from map in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAdmins>()
                                 join admin in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>() on map.AdminID equals admin.ID
                                 where clientID.Contains(map.ClientID)
                                    && map.Type == (int)Enums.ClientAdminType.Merchandiser
                                    && map.Status == (int)Enums.Status.Normal
                                 select new
                                 {
                                     map.ClientID,
                                     RealName = admin.RealName,
                                 };
            var ienums_declarant = linq_declarant.ToArray();

            #endregion

            #region 制单人

            var adminsTopView2 = new AdminsTopView2(this.Reponsitory);

            var createDeclareAdminIDs = ienum_myDecNotices.Where(item => item.CreateDeclareAdminID != null).Select(item => item.CreateDeclareAdminID);

            var linq_createDeclareAdmin = from adminsTop in adminsTopView2
                                          where createDeclareAdminIDs.Contains(adminsTop.OriginID)
                                             //&& adminsTop.Status == (int)Enums.Status.Normal
                                          group adminsTop by new { adminsTop.OriginID } into g
                                          select new
                                          {
                                              CreateDeclareAdminID = g.Key.OriginID,
                                              CreateDeclareAdminName = g.FirstOrDefault().RealName,
                                          };
            var ienums_createDeclareAdmin = linq_createDeclareAdmin.ToArray();

            #endregion

            var ienums_linq = from declare in ienum_myDecNotices
                              join _voyage in ienums_dnVoyage on declare.DeclarationNoticeID equals _voyage.DecNoticeID into voyages
                              from voyage in voyages.DefaultIfEmpty()
                              join _createDeclareAdmin in ienums_createDeclareAdmin on declare.CreateDeclareAdminID equals _createDeclareAdmin.CreateDeclareAdminID
                              into ienums_createDeclareAdmin2
                              from createDeclareAdmin in ienums_createDeclareAdmin2.DefaultIfEmpty()
                              let ogroups_orderItems = groups_orderItems.Single(item => item.OrderID == declare.OrderID)
                              let icgoo = ienums_icgoo.SingleOrDefault(item => item.OrderID == declare.OrderID)
                              let declarant = ienums_declarant.SingleOrDefault(item => item.ClientID == declare.ClientID)
                              select new ModifyDeclareCreatorListViewModel
                              {
                                  DeclarationNoticeID = declare.DeclarationNoticeID,
                                  OrderID = declare.OrderID,
                                  CreateDate = declare.CreateDate,
                                  ClientName = declare.ClientName,
                                  Currency = declare.Currency,
                                  DecNoticeStatus = declare.DecNoticeStatus,

                                  //运输相关
                                  VoyageID = voyage?.VoyageID,
                                  VoyageType = (int)(voyage?.VoyageType ?? Enums.VoyageType.Error),
                                  VoyageTypeName = (voyage?.VoyageType ?? Enums.VoyageType.Error).GetDescription(),

                                  TotalDeclarePrice = (ogroups_orderItems.TotalDeclarePrice).ToRound(4), //* ConstConfig.TransPremiumInsurance
                                  IcgooOrder = icgoo?.IcgooOrder,
                                  DeclarantName = declarant?.RealName,

                                  CreateDeclareAdminID = createDeclareAdmin != null ? createDeclareAdmin.CreateDeclareAdminID : "",
                                  CreateDeclareAdminName = createDeclareAdmin != null ? createDeclareAdmin.CreateDeclareAdminName : "",
                              };

            var results = ienums_linq;

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            Func<ModifyDeclareCreatorListViewModel, object> convert = declareNotice => new
            {
                DecNoticeID = declareNotice.DeclarationNoticeID,
                OrderID = declareNotice.OrderID,
                ClientName = declareNotice.ClientName,
                ClientID = declareNotice.ClientID,
                CreateDate = declareNotice.CreateDate.ToShortDateString(),
                DeclarantName = declareNotice.DeclarantName,
                //OrderSpecialTypeName = CalcOrderSpecialTypeName(declareNotice),
                VoyageID = declareNotice.VoyageID,
                VoyageTypeName = declareNotice.VoyageTypeName,
                //PackNo = declareNotice.PackNo,
                TotalDeclarePriceDisplay = (declareNotice.TotalDeclarePrice * ConstConfig.TransPremiumInsurance).ToRound(4) + " (" + declareNotice.Currency + ")",
                TotalDeclarePrice = (declareNotice.TotalDeclarePrice * ConstConfig.TransPremiumInsurance).ToRound(4),
                //TotalQty = declareNotice.TotalQty,
                //TotalModelQty = declareNotice.TotalModelQty,
                //TotalGrossWeight = declareNotice.TotalGrossWeight,
                //CompanyID = declareNotice.CompanyID,
                IcgooOrder = declareNotice.IcgooOrder,
                DecNoticeStatusDisplay = declareNotice.DecNoticeStatus.GetDescription(),
                CreateDeclareAdminID = declareNotice.CreateDeclareAdminID,
                CreateDeclareAdminName = declareNotice.CreateDeclareAdminName,
            };

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }

        public ModifyDeclareCreatorListView SearchByOrderID(string orderID)
        {
            var linq = from query in this.IQueryable
                       where query.OrderID.Contains(orderID)
                       select query;

            var view = new ModifyDeclareCreatorListView(this.Reponsitory, linq);
            return view;
        }

        public ModifyDeclareCreatorListView SearchByClientName(string name)
        {
            var linq = from query in this.IQueryable
                       where query.ClientName.Contains(name)
                       select query;

            var view = new ModifyDeclareCreatorListView(this.Reponsitory, linq);
            return view;
        }

        public ModifyDeclareCreatorListView SearchByVoyageID(string id)
        {
            var decNoticeVoyagesView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecNoticeVoyages>();
            var voyagesView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Voyages>();
            var myQuery = this.IQueryable;

            var linqs_dnVoyagesID = from dnVoyage in decNoticeVoyagesView
                                    join voyage in voyagesView on dnVoyage.VoyageID equals voyage.ID
                                    where dnVoyage.Status == (int)Enums.Status.Normal && voyage.ID.Contains(id)
                                    select dnVoyage.DecNoticeID;

            var distincts = linqs_dnVoyagesID.Distinct();

            var linq = from query in myQuery
                       join item in distincts on query.DeclarationNoticeID equals item
                       select query;

            var view = new ModifyDeclareCreatorListView(this.Reponsitory, linq);
            return view;
        }

        public ModifyDeclareCreatorListView SearchByDeclareCreatorID(string adminID)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDeclareAdminID == adminID
                       select query;

            var view = new ModifyDeclareCreatorListView(this.Reponsitory, linq);
            return view;
        }

    }

    public class ModifyDeclareCreatorListViewModel
    {
        /// <summary>
        /// 报关通知ID
        /// </summary>
        public string DeclarationNoticeID { get; set; } = string.Empty;

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; } = string.Empty;

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; } = string.Empty;

        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; } = string.Empty;

        /// <summary>
        /// 运输批次号
        /// </summary>
        //[Description("运输批次号")]
        public string VoyageID { get; set; } = string.Empty;

        /// <summary>
        /// 运输类型
        /// </summary>
        //[Description("运输类型")]
        public int VoyageType { get; set; }

        /// <summary>
        /// 运输类型名称
        /// </summary>
        public string VoyageTypeName { get; set; } = string.Empty;

        /// <summary>
        /// 报关总价
        /// </summary>
        public decimal TotalDeclarePrice { get; set; }

        /// <summary>
        /// 报关员姓名
        /// </summary>
        public string DeclarantName { get; set; } = string.Empty;

        /// <summary>
        /// Icgoo/大赢家订单号
        /// </summary>
        public string IcgooOrder { get; set; } = string.Empty;

        /// <summary>
        /// 报关通知状态
        /// </summary>
        public Enums.DeclareNoticeStatus DecNoticeStatus { get; set; }

        /// <summary>
        /// 制单人ID
        /// </summary>
        public string CreateDeclareAdminID { get; set; } = string.Empty;

        /// <summary>
        /// 制单人姓名
        /// </summary>
        public string CreateDeclareAdminName { get; set; } = string.Empty;
    }
}
