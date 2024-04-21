using System;
using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Usually;

namespace Yahv.Erm.Services.Models.Origins
{
    /// <summary>
    /// 联系方式
    /// </summary>
    public class Contact : IUnique
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

        #endregion

        #region 属性
        /// <summary>
        /// StaffID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 固定电话
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// QQ
        /// </summary>
        public string QQ { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 微信
        /// </summary>
        public string Wx { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime UpdateDate { get; set; }
        #endregion

        #region 持久化
        /// <summary>
        /// 添加/修改
        /// </summary>
        public void Enter()
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                //添加
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    //判断是否存在
                    if (repository.ReadTable<Contacts>().Any(a => a.ID == this.ID))
                    {
                        if (EnterError != null)
                            this.EnterError(this, new ErrorEventArgs("该联系方式已存在!"));

                        return;
                    }

                    repository.Insert(new Contacts()
                    {
                        ID = this.ID,       //StaffID
                        CreateDate = DateTime.Now,
                        Email = this.Email,
                        Mobile = this.Mobile,
                        QQ = this.QQ,
                        Tel = this.Tel,
                        Wx = this.Wx,
                    });
                }
                //修改
                else
                {
                    //判断是否存在
                    if (!repository.ReadTable<Contacts>().Any(a => a.ID == this.ID))
                    {
                        if (EnterError != null)
                            this.EnterError(this, new ErrorEventArgs("该信息不存在!"));

                        return;
                    }

                    repository.Update<Contacts>(new
                    {
                        Email = this.Email,
                        Mobile = this.Mobile,
                        QQ = this.QQ,
                        Tel = this.Tel,
                        Wx = this.Wx,
                        UpdateDate = DateTime.Now,
                    }, t => t.ID == this.ID);
                }

                //操作成功
                if (this != null && EnterSuccess != null)
                    this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }
        
        #endregion
    }
}
