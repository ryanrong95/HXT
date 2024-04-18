using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 暂存
    /// </summary>
    public class Temporary : IUnique, IPersist, IFulError, IFulSuccess
    {
        #region 属性

        public string ID { get; set; }

        /// <summary>
        /// 制单人
        /// </summary>
        public Admin Admin { get; set; }

        /// <summary>
        /// 入仓号
        /// </summary>
        public string EntryNumber { get; set; }

        /// <summary>
        /// 客户公司名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 库位号
        /// </summary>
        public string ShelveNumber { get; set; }

        /// <summary>
        /// 入库日期
        /// </summary>
        public DateTime EntryDate { get; set; }

        /// <summary>
        /// 运单号
        /// </summary>
        public string WaybillCode { get; set; }

        /// <summary>
        /// 包装类型
        /// </summary>
        public Enums.BaseWrapType WrapType { get; set; }

        /// <summary>
        /// 件数
        /// </summary>
        public int PackNo { get; set; }

        public Enums.TemporaryStatus TemporaryStatus { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        #endregion

        //操作人
        protected Admin Operator { get; set; }

        public void SetOperator(Admin admin)
        {
            this.Operator = admin;
        }

        /// <summary>
        /// 暂存文件
        /// </summary>
        TemporaryFiles files;
        public TemporaryFiles Files
        {
            get
            {
                if (files == null)
                {
                    using (var view = new Views.TemporaryFileView())
                    {
                        var query = view.Where(item => item.TemporaryID == this.ID);
                        this.files = new TemporaryFiles(query);
                    }
                }
                return this.files;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.files = new TemporaryFiles(value, new Action<TemporaryFile>(delegate (TemporaryFile item)
                {
                    item.TemporaryID = this.ID;
                }));
            }
        }

        public Temporary()
        {
            this.TemporaryStatus = Enums.TemporaryStatus.Untreated;
            this.Status = Enums.Status.Normal;
            this.CreateDate = this.UpdateDate = DateTime.Now;

            this.EnterSuccess += Temporary_EnterSuccess;
        }

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Temporarys>().Count(item => item.ID == this.ID);
                    if (count == 0)
                    {
                        this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.Temporary);
                        reponsitory.Insert(this.ToLinq());
                    }
                    else
                    {
                        this.UpdateDate = DateTime.Now;
                        reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                    }
                }
                this.OnEnterSuccess();
            }
            catch (Exception ex)
            {
                this.EnterError(this, new ErrorEventArgs(ex.Message));
            }
        }

        /// <summary>
        /// 去持久化
        /// </summary>
        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Temporarys>(
                    new
                    {
                        UpdateDate = DateTime.Now,
                        Status = Enums.Status.Delete
                    }, item => item.ID == this.ID);
            }
            this.OnAbandonSuccess();
        }

        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this));
            }
        }

        private void Temporary_EnterSuccess(object sender, SuccessEventArgs e)
        {
            var temporary = (Temporary)e.Object;
            if (temporary == null)
            {
                return;
            }
            var admin = temporary?.Operator;
            if (admin != null)
            {
                if (temporary.CreateDate == temporary.UpdateDate)
                {
                    temporary.Log(admin, "库房管理员[" + admin.RealName + "]新增了暂存信息，等待跟单处理");
                }
                else
                {
                    if (temporary.TemporaryStatus == TemporaryStatus.Untreated)
                    {
                        temporary.Log(admin, "库房管理员[" + admin.RealName + "]编辑了暂存信息，等待跟单处理");
                    }
                    if (temporary.TemporaryStatus == TemporaryStatus.Treated)
                    {
                        temporary.Log(admin, "跟单员[" + admin.RealName + "]处理了暂存信息，等待库房装箱");
                    }
                    if (temporary.TemporaryStatus == TemporaryStatus.Complete)
                    {
                        temporary.Log(admin, "库房管理员[" + admin.RealName + "]完成了装箱");
                    }
                }
            }
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //保存附件
                var temporaryFiles = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.TemporaryFiles>()
                    .Where(item => item.TemporaryID == temporary.ID && item.FileType == (int)FileType.TemporaryPicture);
                var FileIDs = temporary.Files.Select(item => item.ID);
                var dbFileIDs = temporaryFiles.Select(item => item.ID);
                foreach (var ID in dbFileIDs)
                {
                    if (!FileIDs.Contains(ID))
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.TemporaryFiles>(
                            new { Status = Status.Delete }, item => item.ID == ID);
                    }
                }
                foreach (var file in temporary.Files)
                {
                    if (string.IsNullOrEmpty(file.ID))
                    {
                        file.ID = Needs.Overall.PKeySigner.Pick(PKeyType.TemporaryFile);
                        file.TemporaryID = this.ID;
                        file.AdminID = admin.ID;
                        file.FileType = FileType.PIFiles;
                        file.CreateDate = DateTime.Now;
                        file.Status = Status.Normal;
                        reponsitory.Insert(file.ToLinq());
                    }
                }
            }
        }
    }
}