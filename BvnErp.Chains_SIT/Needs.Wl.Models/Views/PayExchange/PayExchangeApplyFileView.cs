using Layer.Data.Sqls;
using Needs.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    public class PayExchangeApplyFileView : View<Models.PayExchangeApplyFile, ScCustomsReponsitory>
    {
        private string PayExchangeApplyID;

        public PayExchangeApplyFileView(string payExchangeApplyID)
        {
            this.PayExchangeApplyID = payExchangeApplyID;
        }

        internal PayExchangeApplyFileView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.PayExchangeApplyFile> GetIQueryable()
        {
            return from payApplyFile in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyFiles>()
                   where payApplyFile.Status == (int)Enums.Status.Normal
                   && payApplyFile.PayExchangeApplyID == this.PayExchangeApplyID
                   select new Models.PayExchangeApplyFile
                   {
                       ID = payApplyFile.ID,
                       PayExchangeApplyID = payApplyFile.PayExchangeApplyID,
                       UserID = payApplyFile.UserID,
                       AdminID = payApplyFile.AdminID,
                       Name = payApplyFile.Name,
                       FileFormat = payApplyFile.FileFormat,
                       FileType = (Enums.FileType)payApplyFile.FileType,
                       Url = payApplyFile.Url,
                       Status = payApplyFile.Status,
                       CreateDate = payApplyFile.CreateDate,
                       Summary = payApplyFile.Summary
                   };
        }

        /// <summary>
        /// 付汇委托书
        /// </summary>
        /// <returns></returns>
        public Models.PayExchangeApplyFile FindPayExchange()
        {
            return this.GetIQueryable().Where(s => s.FileType == Needs.Wl.Models.Enums.FileType.PayExchange).FirstOrDefault();
        }

        /// <summary>
        /// PI文件
        /// </summary>
        /// <returns></returns>
        public IList<Models.PayExchangeApplyFile> FindPIFiles()
        {
            var query = from payApplyFile in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyFiles>()
                        where payApplyFile.Status == (int)Enums.Status.Normal
                        && payApplyFile.PayExchangeApplyID == this.PayExchangeApplyID
                        && payApplyFile.FileType == (int)Needs.Wl.Models.Enums.FileType.PIFiles
                        select new Models.PayExchangeApplyFile
                        {
                            ID = payApplyFile.ID,
                            PayExchangeApplyID = payApplyFile.PayExchangeApplyID,
                            UserID = payApplyFile.UserID,
                            AdminID = payApplyFile.AdminID,
                            Name = payApplyFile.Name,
                            FileFormat = payApplyFile.FileFormat,
                            FileType = (Enums.FileType)payApplyFile.FileType,
                            Url = payApplyFile.Url,
                            Status = payApplyFile.Status,
                            CreateDate = payApplyFile.CreateDate,
                            Summary = payApplyFile.Summary
                        };

            return query.ToList();
        }
    }
}