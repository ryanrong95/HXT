using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 报关通知的视图
    /// </summary>
    public class DeclarationNoticesView : UniqueView<Models.DeclarationNotice, ScCustomsReponsitory>
    {
        public DeclarationNoticesView()
        {
        }

        internal DeclarationNoticesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Models.DeclarationNotice> GetIQueryable()
        {
            var adminsView = new AdminsTopView(this.Reponsitory);
            //var companiesView = new CompaniesTopView(this.Reponsitory);
            var orderView = new OrdersView(this.Reponsitory);

            return from notice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarationNotices>()
                   join admin in adminsView on notice.AdminID equals admin.ID into admins
                   from admin in admins.DefaultIfEmpty()
                       //join agent in companiesView on notice.AgentID equals agent.ID
                   join order in orderView on notice.OrderID equals order.ID
                   select new Models.DeclarationNotice
                   {
                       ID = notice.ID,
                       OrderID = notice.OrderID,
                       order = order,
                       Admin = admin,
                       //Consignor = consignor,
                       //Consignee = consignee,
                       //Agent = agent,
                       Status = (Needs.Ccs.Services.Enums.DeclareNoticeStatus)notice.Status,
                       CreateDate = notice.CreateDate,
                       UpdateDate = notice.UpdateDate,
                       Summary = notice.Summary
                   };
        }
    }

    /// <summary>
    /// 报关通知的视图-待制单
    /// </summary>
    public class DeclarationNoticesWaitView : UniqueView<Models.DeclarationNotice, ScCustomsReponsitory>
    {
        public DeclarationNoticesWaitView()
        {
        }

        internal DeclarationNoticesWaitView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Models.DeclarationNotice> GetIQueryable()
        {
            var adminsView = new AdminsTopView(this.Reponsitory);
            //var companiesView = new CompaniesTopView(this.Reponsitory);
            var orderView = new OrdersView(this.Reponsitory);
            var orderVoyages = new OrderVoyagesOriginView(this.Reponsitory);

            var orderVoyagesCharterBus = orderVoyages.Where(t => t.Type == Enums.OrderSpecialType.CharterBus);
            var orderVoyagesHighValue = orderVoyages.Where(t => t.Type == Enums.OrderSpecialType.HighValue);
            var orderVoyagesInspection = orderVoyages.Where(t => t.Type == Enums.OrderSpecialType.Inspection);
            var orderVoyagesQuarantine = orderVoyages.Where(t => t.Type == Enums.OrderSpecialType.Quarantine);

            var decNoticeVoyagesView = new DecNoticeVoyagesView(this.Reponsitory);

            var headView = new DecHeadsView(this.Reponsitory);

            return from notice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarationNotices>()
                   join admin in adminsView on notice.AdminID equals admin.ID into admins
                   from admin in admins.DefaultIfEmpty()
                       //join agent in companiesView on notice.AgentID equals agent.ID
                   join order in orderView on notice.OrderID equals order.ID
                   where notice.Status == (int)Enums.DeclareNoticeStatus.UnDec

                   join orderVoyage in orderVoyagesCharterBus on order.ID equals orderVoyage.Order.ID into orderVoyages2
                   from orderVoyageCharterBus in orderVoyages2.DefaultIfEmpty()
                   join orderVoyage in orderVoyagesHighValue on order.ID equals orderVoyage.Order.ID into orderVoyages3
                   from orderVoyageHighValue in orderVoyages3.DefaultIfEmpty()

                   join orderVoyage in orderVoyagesInspection on order.ID equals orderVoyage.Order.ID into orderVoyages4
                   from orderVoyageInspection in orderVoyages4.DefaultIfEmpty()
                   join orderVoyage in orderVoyagesQuarantine on order.ID equals orderVoyage.Order.ID into orderVoyages5
                   from orderVoyageQuarantine in orderVoyages5.DefaultIfEmpty()

                   join decNoticeVoyage in decNoticeVoyagesView on notice.ID equals decNoticeVoyage.DeclarationNotice.ID into decNoticeVoyagesView2
                   from decNoticeVoyage in decNoticeVoyagesView2.DefaultIfEmpty()

                   join dechead in headView on notice.ID equals dechead.DeclarationNoticeID into decheads
                   from head in decheads.DefaultIfEmpty()
                   select new Models.DeclarationNotice
                   {
                       ID = notice.ID,
                       OrderID = notice.OrderID,
                       order = order,
                       Admin = admin,
                       //Consignor = consignor,
                       //Consignee = consignee,
                       //Agent = agent,
                       Status = (Needs.Ccs.Services.Enums.DeclareNoticeStatus)notice.Status,
                       CreateDate = notice.CreateDate,
                       UpdateDate = notice.UpdateDate,
                       Summary = notice.Summary,

                       OrderVoyageCharterBus = orderVoyageCharterBus,
                       OrderVoyageHighValue = orderVoyageHighValue,
                       OrderVoyageInspection = orderVoyageInspection,
                       OrderVoyageQuarantine = orderVoyageQuarantine,

                       DecNoticeVoyage = decNoticeVoyage,
                       DecHead = head,
                   };
        }
    }

    /*
    /// <summary>
    /// 报关通知的视图-已制单
    /// </summary>
    public class DeclarationNoticesMakedView : UniqueView<Models.DeclarationNotice, ScCustomsReponsitory>
    {
        public DeclarationNoticesMakedView()
        {
        }

        internal DeclarationNoticesMakedView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Models.DeclarationNotice> GetIQueryable()
        {
            var adminsView = new AdminsTopView(this.Reponsitory);
            //var companiesView = new CompaniesTopView(this.Reponsitory);
            var orderView = new OrdersView(this.Reponsitory);
            var headView = new DecHeadsView(this.Reponsitory);

            var decHeadSpecialTypes = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeadSpecialTypes>();
            var decNoticeVoyages = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecNoticeVoyages>();
            var voyages = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Voyages>();

            return from notice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarationNotices>()
                   join dechead in headView on notice.ID equals dechead.DeclarationNoticeID into decheads
                   from head in decheads.DefaultIfEmpty()
                   join admin in adminsView on notice.AdminID equals admin.ID into admins
                   from admin in admins.DefaultIfEmpty()
                       //join agent in companiesView on notice.AgentID equals agent.ID
                   join order in orderView on notice.OrderID equals order.ID


                   join decHeadSpecialType in decHeadSpecialTypes
                        on new { DecHeadID = head.ID, DecHeadSpecialTypeStatus = (int)Enums.Status.Normal, DecHeadSpecialType = (int)Enums.DecHeadSpecialTypeEnum.CharterBus }
                        equals new { DecHeadID = decHeadSpecialType.DecHeadID, DecHeadSpecialTypeStatus = decHeadSpecialType.Status, DecHeadSpecialType = decHeadSpecialType.Type }
                        into decHeadSpecialTypesCharterBus
                   from decHeadSpecialTypeCharterBus in decHeadSpecialTypesCharterBus.DefaultIfEmpty()
                   join decHeadSpecialType in decHeadSpecialTypes
                        on new { DecHeadID = head.ID, DecHeadSpecialTypeStatus = (int)Enums.Status.Normal, DecHeadSpecialType = (int)Enums.DecHeadSpecialTypeEnum.HighValue }
                        equals new { DecHeadID = decHeadSpecialType.DecHeadID, DecHeadSpecialTypeStatus = decHeadSpecialType.Status, DecHeadSpecialType = decHeadSpecialType.Type }
                        into decHeadSpecialTypesHighValue
                   from decHeadSpecialTypeHighValue in decHeadSpecialTypesHighValue.DefaultIfEmpty()
                   join decHeadSpecialType in decHeadSpecialTypes
                        on new { DecHeadID = head.ID, DecHeadSpecialTypeStatus = (int)Enums.Status.Normal, DecHeadSpecialType = (int)Enums.DecHeadSpecialTypeEnum.Inspection }
                        equals new { DecHeadID = decHeadSpecialType.DecHeadID, DecHeadSpecialTypeStatus = decHeadSpecialType.Status, DecHeadSpecialType = decHeadSpecialType.Type }
                        into decHeadSpecialTypesInspection
                   from decHeadSpecialTypeInspection in decHeadSpecialTypesInspection.DefaultIfEmpty()
                   join decHeadSpecialType in decHeadSpecialTypes
                        on new { DecHeadID = head.ID, DecHeadSpecialTypeStatus = (int)Enums.Status.Normal, DecHeadSpecialType = (int)Enums.DecHeadSpecialTypeEnum.Quarantine }
                        equals new { DecHeadID = decHeadSpecialType.DecHeadID, DecHeadSpecialTypeStatus = decHeadSpecialType.Status, DecHeadSpecialType = decHeadSpecialType.Type }
                        into decHeadSpecialTypesQuarantine
                   from decHeadSpecialTypeQuarantine in decHeadSpecialTypesQuarantine.DefaultIfEmpty()

                   join decNoticeVoyage in decNoticeVoyages
                        on new { DecNoticeID = notice.ID, DecNoticeVoyageStatus = (int)Enums.Status.Normal }
                        equals new { DecNoticeID = decNoticeVoyage.DecNoticeID, DecNoticeVoyageStatus = decNoticeVoyage.Status }
                        into decNoticeVoyages2
                   from decNoticeVoyage in decNoticeVoyages2.DefaultIfEmpty()

                   join voyage in voyages
                        on new { VoyageID = decNoticeVoyage.VoyageID, VoyageStatus = (int)Enums.Status.Normal }
                        equals new { VoyageID = voyage.ID, VoyageStatus = voyage.Status }
                        into voyages2
                   from voyage in voyages2.DefaultIfEmpty()


                   where notice.Status == (int)Enums.DeclareNoticeStatus.AllDec
                   select new Models.DeclarationNotice
                   {
                       ID = notice.ID,
                       OrderID = notice.OrderID,
                       order = order,
                       Admin = admin,
                       Status = (Needs.Ccs.Services.Enums.DeclareNoticeStatus)notice.Status,
                       CreateDate = notice.CreateDate,
                       UpdateDate = notice.UpdateDate,
                       Summary = notice.Summary,
                       DecHead = head,
                       IsCharterBus = (decHeadSpecialTypeCharterBus != null),
                       IsHighValue = (decHeadSpecialTypeHighValue != null),
                       IsInspection = (decHeadSpecialTypeInspection != null),
                       IsQuarantine = (decHeadSpecialTypeQuarantine != null),
                       VoyageID = decNoticeVoyage.VoyageID,
                       VoyageType = (Enums.VoyageType)(voyage.Type != null ? voyage.Type : 0),
                   };
        }
    }
    */

    /// <summary>
    /// 报关通知的视图-已制单
    /// </summary>
    public class DeclarationNoticesMakedView : Needs.Linq.Generic.Unique1Classics<Models.DeclarationNotice, ScCustomsReponsitory>
    {
        public DeclarationNoticesMakedView()
        {

        }

        internal DeclarationNoticesMakedView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.DeclarationNotice> GetIQueryable(Expression<Func<Models.DeclarationNotice, bool>> expression, params LambdaExpression[] expressions)
        {
            var decHeadsView = new DecHeadsView(this.Reponsitory);
            var decNoticeVoyagesView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecNoticeVoyages>().Where(dnv => dnv.Status == (int)Enums.Status.Normal);

            var linq = from notice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarationNotices>()
                       join dechead in decHeadsView on notice.ID equals dechead.DeclarationNoticeID into decheads
                       from dechead in decheads.DefaultIfEmpty()
                       join decNoticeVoyage in decNoticeVoyagesView on notice.ID equals decNoticeVoyage.DecNoticeID into decNoticeVoyages
                       from decNoticeVoyage in decNoticeVoyages.DefaultIfEmpty()
                       where notice.Status == (int)Enums.DeclareNoticeStatus.AllDec
                       orderby notice.CreateDate descending
                       select new Models.DeclarationNotice()
                       {
                           ID = notice.ID,
                           OrderID = notice.OrderID,
                           Status = (Enums.DeclareNoticeStatus)notice.Status,
                           CreateDate = notice.CreateDate,
                           UpdateDate = notice.UpdateDate,
                           Summary = notice.Summary,
                           DecHeadID = dechead != null ? dechead.ID : null,
                           DecHead = dechead,
                           VoyageID = decNoticeVoyage.VoyageID,
                           AdminID = decNoticeVoyage.AdminID
                       };

            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<Models.DeclarationNotice, bool>>);
            }

            return linq.Where(expression);
        }

        protected override IEnumerable<Models.DeclarationNotice> OnReadShips(Models.DeclarationNotice[] results)
        {
            var adminsArr = new AdminsTopView(this.Reponsitory).Where(a => results.Select(r => r.AdminID).ToArray().Contains(a.ID)).ToArray();
            var ordersArr = new OrdersView(this.Reponsitory).Where(o => results.Select(r => r.OrderID).ToArray().Contains(o.ID)).ToArray();

            var specialTypeIds = results.Select(r => r.DecHead.ID).ToArray();
            var decHeadSpecialTypesArr = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeadSpecialTypes>()
                                         .Where(st => specialTypeIds.Contains(st.DecHeadID) && st.Status == (int)Enums.Status.Normal).ToArray();

            var voyageIds = results.Select(r => r.VoyageID).ToArray();
            var voyagesArr = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Voyages>()
                             .Where(v => voyageIds.Contains(v.ID) && v.Status == (int)Enums.Status.Normal).ToArray();

            var decHeadIds = results.Select(r => r.DecHeadID).ToArray();
            var decListsArr = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>().Where(dl => decHeadIds.Contains(dl.DeclarationID)).ToArray();

            return from notice in results
                   join admin in adminsArr on notice.AdminID equals admin.ID into admins
                   from admin in admins.DefaultIfEmpty()
                   join order in ordersArr on notice.OrderID equals order.ID
                   join decHeadSpecialType in decHeadSpecialTypesArr on notice.DecHeadID equals decHeadSpecialType.DecHeadID into decHeadSpecialTypes
                   join voyage in voyagesArr on notice.VoyageID equals voyage.ID into voyages
                   from voyage in voyages.DefaultIfEmpty()
                   join decList in decListsArr on notice.DecHeadID equals decList.DeclarationID into decLists
                   select new Models.DeclarationNotice
                   {
                       ID = notice.ID,
                       OrderID = notice.OrderID,
                       order = order,
                       AdminID = notice.AdminID,
                       Admin = admin,
                       Status = notice.Status,
                       CreateDate = notice.CreateDate,
                       UpdateDate = notice.UpdateDate,
                       Summary = notice.Summary,
                       DecHeadID = notice.DecHeadID,
                       DecHead = notice.DecHead,
                       IsCharterBus = decHeadSpecialTypes.Where(st => st.Type == (int)Enums.DecHeadSpecialTypeEnum.CharterBus).Count() > 0,
                       IsHighValue = decHeadSpecialTypes.Where(st => st.Type == (int)Enums.DecHeadSpecialTypeEnum.HighValue).Count() > 0,
                       IsInspection = decHeadSpecialTypes.Where(st => st.Type == (int)Enums.DecHeadSpecialTypeEnum.Inspection).Count() > 0,
                       IsQuarantine = decHeadSpecialTypes.Where(st => st.Type == (int)Enums.DecHeadSpecialTypeEnum.Quarantine).Count() > 0,
                       IsCCC = decHeadSpecialTypes.Where(st => st.Type == (int)Enums.DecHeadSpecialTypeEnum.CCC).Count() > 0,
                       VoyageID = notice.VoyageID,
                       VoyageType = (Enums.VoyageType)(voyage != null ? voyage.Type : 0),

                       TotalQty = decLists.Sum(dl => dl.GQty),
                       TotalModelQty = decLists.Count()
                   };
        }
    }
}
