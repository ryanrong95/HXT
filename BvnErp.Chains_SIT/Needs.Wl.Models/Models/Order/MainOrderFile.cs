using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using System;
using System.Linq;

namespace Needs.Wl.Models
{
    public class MainOrderFile : OrderFile
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public string MainOrderID { get; set; }

       
        public override void Enter()
        {
            int count = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.MainOrderFiles>().Count(item => item.ID == this.ID);

            if (count == 0)
            {
                //主键ID（OrderFile +8位年月日+6位流水号）
                this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderFile);
                this.Reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.MainOrderFiles
                {
                    ID = this.ID,
                    MainOrderID = this.MainOrderID,                    
                    AdminID = this.Admin?.ID,
                    UserID = this.User?.ID,
                    Name = this.Name,
                    FileType = (int)this.FileType,
                    FileFormat = this.FileFormat,
                    Url = this.Url,
                    FileStatus = (int)this.FileStatus,
                    Status = (int)this.Status,
                    CreateDate = DateTime.Now,
                    Summary = this.Summary
                });
            }
            else
            {
                this.Reponsitory.Update(new Layer.Data.Sqls.ScCustoms.MainOrderFiles
                {
                    ID = this.ID,
                    MainOrderID = this.MainOrderID,                   
                    AdminID = this.Admin?.ID,
                    UserID = this.User?.ID,
                    Name = this.Name,
                    FileType = (int)this.FileType,
                    FileFormat = this.FileFormat,
                    Url = this.Url,
                    FileStatus = (int)this.FileStatus,
                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    Summary = this.Summary
                }, item => item.ID == this.ID);
            }
        }

      

        /// <summary>
        /// 删除
        /// </summary>
        public override void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.MainOrderFiles>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            }

            //this.OnAbandonSuccess();
        }

        public void Delete()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.MainOrderFiles>(item => item.ID == this.ID);
            }

            //this.OnDeletedSuccess();
        }
    }
}