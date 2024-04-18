using Layers.Data.Sqls.PvbCrm;
using System;
using System.Linq;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 代收付货款申请的收付款凭证文件
    /// </summary>
    public class ApplicationFilesTopView<TReponsitory> : UniqueView<ApplicationFile, TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ApplicationFilesTopView()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ApplicationFilesTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<ApplicationFile> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.ApplicationFilesTopView>()
                   select new ApplicationFile
                   {
                       ID = entity.ID,
                       ApplicationID = entity.ApplicationID,
                       Payer = entity.Payer,
                       Payee = entity.Payee,
                       CustomName = entity.CustomName,
                       Type = entity.Type,
                       Url = entity.Url,
                       CreateDate = entity.CreateDate,
                       Status = (FileDescriptionStatus)entity.Status,
                   };
        }
    }
}
