using System;
using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Erm.Services.Models.Origins
{
    /// <summary>
    /// 岗位
    /// </summary>
    public class Postion : IUnique
    {
        #region 事件

        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// EnterError
        /// </summary>
        public event ErrorHanlder EnterError;

        /// <summary>
        /// AbandonSuccess
        /// </summary>
        public event SuccessHanlder AbandonSuccess;

        #endregion

        #region 属性
        string id;

        /// <summary>
        /// MD5（Name）
        /// </summary>
        public string ID
        {
            get
            {
                return this.id ?? string.Join("", this.Name).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 岗位名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 录入人
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 正常 停用
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 录入人
        /// </summary>
        public string AdminName { get; set; }
        #endregion

        #region 持久化
        /// <summary>
        /// 添加/修改
        /// </summary>
        public void Enter()
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                var entity = repository.ReadTable<Postions>().FirstOrDefault(t => t.Name == this.Name);

                //判断是否存在
                if (entity != null && !string.IsNullOrWhiteSpace(entity.ID))
                {
                    if (entity.Status == (int)Status.Delete)
                    {
                        repository.Update<Postions>(new
                        {
                            Status = (int)Status.Normal
                        }, item => item.ID == entity.ID);
                        if (this != null && EnterSuccess != null)
                            this.EnterSuccess(this, new SuccessEventArgs(this));
                        return;
                    }

                    if (EnterError != null)
                    {
                        this.EnterError(this, new ErrorEventArgs("该岗位已存在!"));
                        return;
                    }
                }

                repository.Insert(new Postions()
                {
                    ID = this.ID,
                    Status = (int)Status.Normal,
                    Name = this.Name,
                    CreateDate = DateTime.Now,
                    AdminID = this.AdminID,
                });


                //操作成功
                if (this != null && EnterSuccess != null)
                    this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        /// <summary>
        /// 废弃
        /// </summary>
        public void Abandon()
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvbErm.Postions>(new
                {
                    Status = Status.Delete
                }, item => item.ID == this.ID);

                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }

        #endregion
    }
}