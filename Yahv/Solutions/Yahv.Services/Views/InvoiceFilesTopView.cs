using Layers.Data.Sqls.PvbCrm;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Yahv.Services.Views
{
    /// <summary>
    ///海关发票文件视图
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class InvoiceFilesTopView<TReponsitory> : QueryView<InvoiceFile, TReponsitory>
        where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public InvoiceFilesTopView()
        {

        }

        public InvoiceFilesTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }
        protected override IQueryable<InvoiceFile> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.InvoiceFilesTopView>()
                   select new InvoiceFile()
                   {
                         OrderID = entity.OrderID,
                         Name=entity.Name,
                         Url=entity.Url,
                         FileFormat=entity.FileFormat,
                         FileType=entity.FileType,
                         Status=entity.Status
                   };
        }
    }
}
