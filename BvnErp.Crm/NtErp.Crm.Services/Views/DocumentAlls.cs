using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Linq;
using Layer.Data.Sqls;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;

namespace NtErp.Crm.Services.Views
{
    public class DocumentAlls : UniqueView<Document, BvCrmReponsitory>, Needs.Underly.IFkoView<Document>
    {
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public DocumentAlls()
        {

        }

        /// <summary>
        /// 返回结果集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Document> GetIQueryable()
        {
            AdminTopView adminTops = new AdminTopView(this.Reponsitory);

            return from document in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Documents>()
                   join admin in adminTops on document.AdminID equals admin.ID
                   where document.Status == (int)Status.Normal
                   select new Document
                   {
                       ID = document.ID,
                       DirectoryID = document.DirectoryID,
                       Title = document.Title,
                       Name = document.Name,
                       Url = document.Url,
                       Size = document.Size,
                       Status = (Status)document.Status,
                       Admin = admin,
                       CreateDate = document.CreateDate,
                       UpdateDate = document.UpdateDate,
                       Summary = document.Summary,
                   };
        }
    }
}
