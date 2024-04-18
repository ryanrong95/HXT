using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// DecHeadSpecialTypes 维护相关视图
    /// </summary>
    public class DecHeadSpecialTypesView
    {
        private ScCustomsReponsitory _reponsitory;

        public DecHeadSpecialTypesView(ScCustomsReponsitory reponsitory)
        {
            _reponsitory = reponsitory;
        }

        public class OrderVoyageModel
        {
            /// <summary>
            /// 订单特殊类型
            /// </summary>
            public Enums.OrderSpecialType Type { get; set; }
        }

        public IEnumerable<OrderVoyageModel> GetOrderVoyageModel(string decNoticeID)
        {
            var orderVoyages = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderVoyages>();
            var declarationNotices = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarationNotices>();

            return from orderVoyage in orderVoyages
                   join declarationNotice in declarationNotices
                        on new { OrderID = orderVoyage.OrderID, OrderVoyageStatus = orderVoyage.Status, DecNoticeID = decNoticeID }
                        equals new { OrderID = declarationNotice.OrderID, OrderVoyageStatus = (int)Enums.Status.Normal, DecNoticeID = declarationNotice.ID }
                   where orderVoyage.Type != (int)Enums.OrderSpecialType.CharterBus
                   select new OrderVoyageModel
                   {
                       Type = (Enums.OrderSpecialType)orderVoyage.Type,
                   };
        }


        public class DecNoticeVoyageModel
        {
            public Enums.VoyageType Type { get; set; }
        }

        public IEnumerable<DecNoticeVoyageModel> GetDecNoticeVoyageModel(string decNoticeID)
        {
            var decNoticeVoyages = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecNoticeVoyages>();
            var voyages = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Voyages>();

            return from decNoticeVoyage in decNoticeVoyages
                   join voyage in voyages
                        on new
                        {
                            VoyageID = decNoticeVoyage.VoyageID,
                            DecNoticeVoyageStatus = decNoticeVoyage.Status,
                            VoyageStatus = (int)Enums.Status.Normal,
                            DecNoticeID = decNoticeVoyage.DecNoticeID,
                        }
                        equals new
                        {
                            VoyageID = voyage.ID,
                            DecNoticeVoyageStatus = (int)Enums.Status.Normal,
                            VoyageStatus = voyage.Status,
                            DecNoticeID = decNoticeID
                        }
                   select new DecNoticeVoyageModel
                   {
                       Type = (Enums.VoyageType)voyage.Type,
                   };
        }
    }

    public class DecHeadSpecialTypesRoleView : UniqueView<Models.DecHeadSpecialType, ScCustomsReponsitory>
    {
        protected override IQueryable<DecHeadSpecialType> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeadSpecialTypes>()
                   where entity.Status == (int)Enums.Status.Normal
                   select new DecHeadSpecialType
                   {
                       ID = entity.ID,
                       DecHeadID = entity.DecHeadID,
                       Type = (Enums.DecHeadSpecialTypeEnum)entity.Type,
                       Status = (Enums.Status)entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Summary = entity.Summary,
                   };
        }
    }
}
