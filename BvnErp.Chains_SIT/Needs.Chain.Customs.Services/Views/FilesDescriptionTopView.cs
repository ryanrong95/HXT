using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class FilesDescriptionTopView
    {
        private ScCustomsReponsitory Reponsitory { get; set; }

        public FilesDescriptionTopView()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public FilesDescriptionTopView(ScCustomsReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        public List<FilesDescriptionTopViewModel> GetResults(LambdaExpression[] expressions)
        {
            var filesDescriptionTopView = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.FilesDescriptionTopView>();

            var results = from file in filesDescriptionTopView
                          select new FilesDescriptionTopViewModel
                          {
                              ID = file.ID,
                              WsOrderID = file.WsOrderID,
                              //ApplicationID = file.ApplicationID,
                              LsOrderID = file.LsOrderID,
                              WaybillID = file.WaybillID,
                              NoticeID = file.NoticeID,
                              StorageID = file.StorageID,
                              InputID = file.InputID,
                              AdminID = file.AdminID,
                              ClientID = file.ClientID,
                              CustomName = file.CustomName,
                              PayID = file.PayID,
                              Type = file.Type,
                              Url = file.Url,
                              CreateDate = file.CreateDate,
                              Status = file.Status,
                              //StaffID = file.StaffID,
                              //ErmApplicationID = file.ErmApplicationID,
                              ShipID = file.ShipID,
                          };

            foreach (var expression in expressions)
            {
                results = results.Where(expression as Expression<Func<FilesDescriptionTopViewModel, bool>>);
            }

            return results.ToList();
        }
    }

    public class FilesDescriptionTopViewModel
    {
        public string ID { get; set; } = string.Empty;
        public string WsOrderID { get; set; } = string.Empty;
        //public string ApplicationID { get; set; } = string.Empty;
        public string LsOrderID { get; set; } = string.Empty;
        public string WaybillID { get; set; } = string.Empty;
        public string NoticeID { get; set; } = string.Empty;
        public string StorageID { get; set; } = string.Empty;
        public string InputID { get; set; } = string.Empty;
        public string AdminID { get; set; } = string.Empty;
        public string ClientID { get; set; } = string.Empty;
        public string CustomName { get; set; } = string.Empty;
        public string PayID { get; set; } = string.Empty;
        public int Type { get; set; }
        public string Url { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; }
        public int Status { get; set; }
        //public string StaffID { get; set; } = string.Empty;
        //public string ErmApplicationID { get; set; } = string.Empty;
        public string ShipID { get; set; }

    }
}
