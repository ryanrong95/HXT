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
    /// 暂存文件View
    /// </summary>
    public class OrderWhesPremiumFileView : UniqueView<Models.OrderWhesPremiumFile, ScCustomsReponsitory>
    {
        public OrderWhesPremiumFileView()
        {
        }

        internal OrderWhesPremiumFileView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.OrderWhesPremiumFile> GetIQueryable()
        {
            return from orderWhesPremiumFile in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWhesPremiumFiles>()
                   where orderWhesPremiumFile.Status == (int)Enums.Status.Normal
                   select new Models.OrderWhesPremiumFile
                   {
                       ID = orderWhesPremiumFile.ID,
                       OrderWhesPremiumID = orderWhesPremiumFile.OrderWhesPremiumID,
                       AdminID = orderWhesPremiumFile.AdminID,
                       Name = orderWhesPremiumFile.Name,
                       FileType = (Enums.FileType)orderWhesPremiumFile.FileType,
                       FileFormat = orderWhesPremiumFile.FileFormat,
                       URL = orderWhesPremiumFile.URL,
                       Status = (Enums.Status)orderWhesPremiumFile.Status,
                       CreateDate = orderWhesPremiumFile.CreateDate,
                       Summary = orderWhesPremiumFile.Summary,
                   };
        }
    }
}
