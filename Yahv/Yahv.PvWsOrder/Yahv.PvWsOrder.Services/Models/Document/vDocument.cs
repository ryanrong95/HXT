using System;
using System.Linq;
using Layers.Data;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Usually;

namespace Yahv.PvWsOrder.Services.Models
{
    /// <summary>
    /// 文档
    /// </summary>
    public class vDocument : IUnique
    {
        #region 事件

        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// AbandonSuccess
        /// </summary>
        public event SuccessHanlder AbandonSuccess;
        #endregion

        #region 属性
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 分类ID
        /// </summary>
        public string CatalogID { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// 创建ID
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }
        #endregion

        #region 扩展属性
        /// <summary>
        /// 分类名称
        /// </summary>
        public string CatalogName { get; set; }
        /// <summary>
        /// 创建人姓名
        /// </summary>
        public string CreatorName { get; set; }
        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = new PvWsOrderReponsitory())
            {
                //新增
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.vDocuments>().Any(item => item.ID == this.ID))
                {
                    this.ID = Layers.Data.PKeySigner.Pick(Yahv.Underly.PKeyType.vDocuments);

                    reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.vDocuments()
                    {
                        ID = this.ID,
                        Title = this.Title,
                        CatalogID = this.CatalogID,
                        Context = this.Context,
                        CreatorID = this.CreatorID,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                    });
                }
                //修改
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvWsOrder.vDocuments>(new
                    {
                        ModifyDate = DateTime.Now,
                        Title = this.Title,
                        CatalogID = this.CatalogID,
                        Context = this.Context,
                        CreatorID = this.CreatorID,
                    }, item => item.ID == this.ID);
                }

                if (this != null && EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
        }

        /// <summary>
        /// 废弃
        /// </summary>
        public void Abandon()
        {
            using (var reponsitory = new PvWsOrderReponsitory())
            {
                reponsitory.Delete<Layers.Data.Sqls.PvWsOrder.vDocuments>(item => item.ID == this.ID);

                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        #endregion
    }
}