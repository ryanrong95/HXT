using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    /// <summary>
    /// 订单的报关单附件
    /// </summary>
    public class OrderDecHeadFilesView : View<Models.DecHeadFile, ScCustomsReponsitory>
    {
        private string OrderID;

        public OrderDecHeadFilesView(string orderID)
        {
            this.OrderID = orderID;
            this.AllowPaging = false;
        }

        protected override IQueryable<Models.DecHeadFile> GetIQueryable()
        {
            return from file in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeadFiles>()
                   join decHead in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>() on file.DecHeadID equals decHead.ID
                   where decHead.OrderID == this.OrderID && file.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                   select new Models.DecHeadFile
                   {
                       ID = file.ID,
                       Name = file.Name,
                       FileType = (Enums.FileType)file.FileType,
                       FileFormat = file.FileFormat,
                       Url = file.Url,
                       AdminID = file.AdminID,
                       Status = file.Status,
                       CreateDate = file.CreateDate,
                       Summary = file.Summary
                   };
        }

        /// <summary>
        /// 根据附件类型查找
        /// </summary>
        /// <returns></returns>
        public Models.DecHeadFile FindByType(Enums.FileType type)
        {
            return this.GetIQueryable().Where(s => s.FileType == type).FirstOrDefault();
        }
    }
}