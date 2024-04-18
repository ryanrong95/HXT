using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using System;
using System.Linq;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 订单的附件
    /// </summary>
    public class OrderFile : ModelBase<Layer.Data.Sqls.ScCustoms.OrderFiles, ScCustomsReponsitory>, IUnique, IPersist
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 订单ItemID
        /// </summary>
        public string OrderItemID { get; set; }

        /// <summary>
        /// 订单费用ID
        /// </summary>
        public string OrderPremiumID { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public Enums.FileType FileType { get; set; }

        /// <summary>
        /// 文件格式
        /// </summary>
        public string FileFormat { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 上传人
        /// </summary>
        public Admin Admin { get; set; }

        /// <summary>
        /// 上传人
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// 文件状态
        /// </summary>
        public Enums.OrderFileStatus FileStatus { get; set; }

        public override void Enter()
        {
            int count = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderFiles>().Count(item => item.ID == this.ID);

            if (count == 0)
            {
                //主键ID（OrderFile +8位年月日+6位流水号）
                this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderFile);
                this.Reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderFiles
                {
                    ID = this.ID,
                    OrderID = this.OrderID,
                    OrderItemID = this.OrderItemID,
                    OrderPremiumID = this.OrderPremiumID,
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
                this.Reponsitory.Update(new Layer.Data.Sqls.ScCustoms.OrderFiles
                {
                    ID = this.ID,
                    OrderID = this.OrderID,
                    OrderItemID = this.OrderItemID,
                    OrderPremiumID = this.OrderPremiumID,
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

        public override void Abandon()
        {
            base.Abandon();
        }
    }
}