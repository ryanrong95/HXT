using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 调整发单人列表视图
    /// </summary>
    public class ModifyCustomSubmiterListView : QueryView<ModifyCustomSubmiterListViewModel, ScCustomsReponsitory>
    {
        public ModifyCustomSubmiterListView()
        {
        }

        protected ModifyCustomSubmiterListView(ScCustomsReponsitory reponsitory, IQueryable<ModifyCustomSubmiterListViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<ModifyCustomSubmiterListViewModel> GetIQueryable()
        {
            var decHeadsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var ordersView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clientsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();

            var iQuery = from dechead in decHeadsView
                         join order in ordersView on dechead.OrderID equals order.ID
                         join client in clientsView on order.ClientID equals client.ID
                         where order.Status == (int)Enums.Status.Normal
                         orderby dechead.ContrNo descending
                         select new ModifyCustomSubmiterListViewModel
                         {
                             DecHeadID = dechead.ID,
                             ContrNo = dechead.ContrNo,
                             BillNo = dechead.BillNo,
                             OrderID = dechead.OrderID,
                             ClientID = order.ClientID,
                             ClientCode = client.ClientCode,
                             ClientName = dechead.OwnerName,
                             //VoyageID = dechead.VoyNo,
                             //PackNo = dechead.PackNo,
                             //CreateTime = dechead.CreateTime,
                             //InputerID = dechead.InputerID,
                             //Status = dechead.CusDecStatus,
                             //SeqNo = dechead.SeqNo,
                             //GrossWt = dechead.GrossWt,
                             DDate = dechead.DDate.Value,
                             //EntryId = dechead.EntryId,
                             DeclarationNoticeID = dechead.DeclarationNoticeID,
                             CustomSubmiterID = dechead.SubmitCustomAdminID,
                             DoubleCheckerID = dechead.DoubleCheckerAdminID,
                             CusDecStatus = dechead.CusDecStatus
                         };
            return iQuery;
        }

        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<ModifyCustomSubmiterListViewModel> iquery = this.IQueryable.Cast<ModifyCustomSubmiterListViewModel>().OrderByDescending(item => item.DecHeadID);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myDeclares = iquery.ToArray();

            #region 制单人

            var declarationNoticesView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarationNotices>();
            var adminsTopView2 = new AdminsTopView2(this.Reponsitory);

            var declarationNoticeIDs = ienum_myDeclares.Select(item => item.DeclarationNoticeID);

            var linq_createDeclareAdmin = from adminsTop in adminsTopView2
                                          join declarationNotice in declarationNoticesView on adminsTop.OriginID equals declarationNotice.CreateDeclareAdminID
                                          where declarationNoticeIDs.Contains(declarationNotice.ID)
                                          group new { adminsTop, declarationNotice } by new { DeclarationNoticeID = declarationNotice.ID } into g
                                          select new
                                          {
                                              DeclarationNoticeID = g.Key.DeclarationNoticeID,
                                              CreateDeclareAdminName = g.FirstOrDefault().adminsTop.RealName,
                                          };
            var ienums_createDeclareAdmin = linq_createDeclareAdmin.ToArray();

            #endregion

            #region 发单人

            var customSubmiterIDs = ienum_myDeclares.Where(item => item.CustomSubmiterID != null).Select(item => item.CustomSubmiterID);

            var linq_customSubmiterAdmin = from adminsTop in adminsTopView2
                                          where customSubmiterIDs.Contains(adminsTop.OriginID)
                                          //&& adminsTop.Status == (int)Enums.Status.Normal
                                          group adminsTop by new { adminsTop.OriginID } into g
                                          select new
                                          {
                                              CustomSubmiterAdminID = g.Key.OriginID,
                                              CustomSubmiterAdminName = g.FirstOrDefault().RealName,
                                          };
            var ienums_customSubmiterAdmin = linq_customSubmiterAdmin.ToArray();

            #endregion

            #region 复核人
            var doubleCheckerIDs = ienum_myDeclares.Where(item => item.DoubleCheckerID != null).Select(item => item.DoubleCheckerID);

            var linq_doubleCheckerAdmin = from adminsTop in adminsTopView2
                                           where doubleCheckerIDs.Contains(adminsTop.OriginID)
                                           //&& adminsTop.Status == (int)Enums.Status.Normal
                                           group adminsTop by new { adminsTop.OriginID } into g
                                           select new
                                           {
                                               DoubleCheckerAdminID = g.Key.OriginID,
                                               DoubleCheckerAdminName = g.FirstOrDefault().RealName,
                                           };
            var ienums_doubleCheckerAdmin = linq_doubleCheckerAdmin.ToArray();
            #endregion

            var ienums_linq = from dechead in ienum_myDeclares
                              join _createDeclareAdmin in ienums_createDeclareAdmin on dechead.DeclarationNoticeID equals _createDeclareAdmin.DeclarationNoticeID
                              into ienums_createDeclareAdmin2
                              from createDeclareAdmin in ienums_createDeclareAdmin2.DefaultIfEmpty()

                              join _customSubmiterAdmin in ienums_customSubmiterAdmin on dechead.CustomSubmiterID equals _customSubmiterAdmin.CustomSubmiterAdminID
                              into ienums_customSubmiterAdmin2
                              from customSubmiterAdmin in ienums_customSubmiterAdmin2.DefaultIfEmpty()

                              join _doubleCheckerAdmin in ienums_doubleCheckerAdmin on dechead.DoubleCheckerID equals _doubleCheckerAdmin.DoubleCheckerAdminID
                             into ienums_doubleCheckerAdmin2
                              from doubleCheckerAdmin in ienums_doubleCheckerAdmin2.DefaultIfEmpty()

                              select new ModifyCustomSubmiterListViewModel
                              {
                                  DecHeadID = dechead.DecHeadID,
                                  ContrNo = dechead.ContrNo,
                                  BillNo = dechead.BillNo,
                                  OrderID = dechead.OrderID,
                                  ClientID = dechead.ClientID,
                                  ClientCode = dechead.ClientCode,
                                  ClientName = dechead.ClientName,
                                  DDate = dechead.DDate,
                                  CusDecStatus = dechead.CusDecStatus,

                                  CreateDeclareAdminName = createDeclareAdmin != null ? createDeclareAdmin.CreateDeclareAdminName : "",
                                  CustomSubmiterName = customSubmiterAdmin != null ? customSubmiterAdmin.CustomSubmiterAdminName : "",
                                  DoubleCheckerName = doubleCheckerAdmin != null ? doubleCheckerAdmin.DoubleCheckerAdminName : "",
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

            Func<ModifyCustomSubmiterListViewModel, object> convert = head => new
            {
                DecHeadID = head.DecHeadID,
                ContrNo = head.ContrNo,
                OrderID = head.OrderID,
                BillNo = head.BillNo,               
                DDate = head.DDate.ToString("yyyy-MM-dd HH:mm"),
                ClientCode = head.ClientCode,
                ClientName = head.ClientName,
                CreateDeclareAdminName = head.CreateDeclareAdminName,
                CustomSubmiterName = head.CustomSubmiterName,
                DoubleCheckerName = head.DoubleCheckerName,
                StatusName = head.StatusName
            };


            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }


        /// <summary>
        /// 查询合同号
        /// </summary>
        /// <param name="orderID">订单ID</param>
        /// <returns>视图</returns>
        public ModifyCustomSubmiterListView SearchByContractID(string contractID)
        {
            var linq = from query in this.IQueryable
                       where query.ContrNo.Contains(contractID)
                       select query;

            var view = new ModifyCustomSubmiterListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 查询订单号
        /// </summary>
        /// <param name="orderID">订单ID</param>
        /// <returns>视图</returns>
        public ModifyCustomSubmiterListView SearchByOrderID(string orderID)
        {
            var linq = from query in this.IQueryable
                       where query.OrderID.Contains(orderID)
                       select query;

            var view = new ModifyCustomSubmiterListView(this.Reponsitory, linq);
            return view;
        }

        public ModifyCustomSubmiterListView SearchByFrom(DateTime fromtime)
        {
            var linq = from query in this.IQueryable
                       where query.DDate >= fromtime
                       select query;

            var view = new ModifyCustomSubmiterListView(this.Reponsitory, linq);
            return view;
        }

        public ModifyCustomSubmiterListView SearchByTo(DateTime totime)
        {
            var linq = from query in this.IQueryable
                       where query.DDate <= totime
                       select query;

            var view = new ModifyCustomSubmiterListView(this.Reponsitory, linq);
            return view;
        }

        //public ModifyCustomSubmiterListView SearchByCustomSubmitor(string adminName)
        //{
        //    var linq = from query in this.IQueryable
        //               where query. <= totime
        //               select query;

        //    var view = new ModifyCustomSubmiterListView(this.Reponsitory, linq);
        //    return view;
        //}


    }

    public class ModifyCustomSubmiterListViewModel
    {
        /// <summary>
        /// 报关单ID
        /// </summary>
        public string DecHeadID { get; set; } = string.Empty;

        /// <summary>
        /// 合同号
        /// </summary>
        public string ContrNo { get; set; } = string.Empty;

        /// <summary>
        /// 提(运)单号
        /// </summary>
        public string BillNo { get; set; } = string.Empty;

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; } = string.Empty;

        /// <summary>
        /// 客户编号
        /// </summary>
        public string ClientID { get; set; } = string.Empty;

        /// <summary>
        /// 客户编号
        /// </summary>
        public string ClientCode { get; set; } = string.Empty;

        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; } = string.Empty;

        /// <summary>
        /// 发单日期
        /// </summary>
        public DateTime DDate { get; set; }

        /// <summary>
        /// 报关通知ID
        /// </summary>
        public string DeclarationNoticeID { get; set; }

        /// <summary>
        /// 制单人姓名
        /// </summary>
        public string CreateDeclareAdminName { get; set; } = string.Empty;

        /// <summary>
        /// 发单人ID
        /// </summary>
        public string CustomSubmiterID { get; set; } = string.Empty;

        /// <summary>
        /// 发单人姓名
        /// </summary>
        public string CustomSubmiterName { get; set; } = string.Empty;
        /// <summary>
        /// 复核人ID
        /// </summary>
        public string DoubleCheckerID { get; set; } = string.Empty;
        /// <summary>
        /// 复核人姓名
        /// </summary>
        public string DoubleCheckerName { get; set; } = string.Empty;

        /// <summary>
        /// 状态
        /// </summary>
        public string CusDecStatus { get; set; }

        /// <summary>
        /// 单据状态名字
        /// </summary>
        public string StatusName
        {
            get
            {
                return MultiEnumUtils.ToText<Enums.CusDecStatus>(this.CusDecStatus);
            }
        }
    }
}
