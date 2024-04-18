using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Linq.Generic;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Underly.Attributes;

namespace Yahv.PvWsOrder.Services.XDTClientView
{
    //public class PayExchangeApplyFilesView : UniqueView<PayExchangeApplyFile, ScCustomReponsitory>
    //{
    //    private PayExchangeApplyFilesView()
    //    {

    //    }

    //    public PayExchangeApplyFilesView(ScCustomReponsitory reponsitory) : base(reponsitory)
    //    {

    //    }

    //    protected override IQueryable<PayExchangeApplyFile> GetIQueryable()
    //    {
    //        return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.PayExchangeApplyFiles>()
    //               where entity.Status == (int)GeneralStatus.Normal
    //               orderby entity.CreateDate descending
    //               select new PayExchangeApplyFile
    //               {
    //                   ID = entity.ID,
    //                   PayExchangeApplyID = entity.PayExchangeApplyID,
    //                   UserID = entity.UserID,
    //                   AdminID = entity.AdminID,
    //                   Name = entity.Name,
    //                   FileFormat = entity.FileFormat,
    //                   FileType = (XDTFileType)entity.FileType,
    //                   Url = entity.Url,
    //                   Status = entity.Status,
    //                   CreateDate = entity.CreateDate,
    //                   Summary = entity.Summary
    //               };
    //    }
    //}

    /// <summary>
    /// 付汇文件
    /// </summary>
    public class PayExchangeApplyFile : IUnique
    {
        public string ID { get; set; }

        public string PayExchangeApplyID { get; set; }

        public string Name { get; set; }

        public FileType FileType { get; set; }

        public string FileFormat { get; set; }

        public string Url { get; set; }

        public string ClientID { get; set; }

        public string UserID { get; set; }

        public int Status { get; set; }

        public DateTime CreateDate { get; set; }

        public string Summary { get; set; }


        public void Enter()
        {
            using (Layers.Data.Sqls.PvCenterReponsitory reponsitory = new Layers.Data.Sqls.PvCenterReponsitory())
            {
                reponsitory.Update<Layers.Data.Sqls.PvCenter.FilesDescription>(new
                {
                    Status = 400,
                }, item => item.ApplicationID == this.PayExchangeApplyID && item.Type == (int)this.FileType);
                CenterFileDescription[] files = new CenterFileDescription[] {
                      new CenterFileDescription{
                           CustomName = this.Name,
                           Url = this.Url,
                           AdminID = this.UserID,
                           ApplicationID =  this.PayExchangeApplyID,
                           Type =(int)this.FileType,
                      }
                };
                //上传文件
                new Yahv.PvWsOrder.Services.ClientViews.CenterFilesView().Upload(files);
            }
        }
    }
}
