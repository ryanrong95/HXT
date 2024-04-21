using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;
using YaHv.Csrm.Services.Extends;

namespace YaHv.Csrm.Services.Models.Origins
{
    public class Driver : Yahv.Linq.IUnique
    {
        public Driver()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = GeneralStatus.Normal;
        }
        #region 属性
        string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Join("", this.Enterprise.ID, this.Name, this.IDCard, this.Mobile).MD5();

            }
            set
            {
                this.id = value;
            }
        }
        public Enterprise Enterprise { set; get; }
        /// <summary>
        /// 司机姓名
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string IDCard { set; get; }
        /// <summary>
        /// 手机号1，大陆手机号
        /// </summary>
        public string Mobile { set; get; }
        /// <summary>
        /// 香港手机号
        /// </summary>
        //public string HKMobile { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { set; get; }
        /// <summary>
        /// 编辑时间
        /// </summary>
        public DateTime UpdateDate { set; get; }
        public string CreatorID { set; get; }
        /// <summary>
        /// 添加人
        /// </summary>
        public Admin Creator { internal set; get; }
        /// <summary>
        /// 其他手机号：香港或其他地区
        /// </summary>

        public string Mobile2 { set; get; }
        /// <summary>
        /// 海关编码
        /// </summary>

        public string CustomsCode { set; get; }
        /// <summary>
        /// 司机卡号
        /// </summary>

        public string CardCode { set; get; }
        /// <summary>
        /// 口岸电子编号
        /// </summary>

        public string PortCode { set; get; }
        /// <summary>
        /// 寮步密码
        /// </summary>

        public string LBPassword { set; get; }
        /// <summary>
        /// 是否中港贸易
        /// </summary>
        public bool IsChcd { set; get; }
        #endregion


        #region 事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;
        /// <summary>
        /// AbandonSuccess
        /// </summary>
        public event SuccessHanlder AbandonSuccess;
        /// <summary>
        /// 已存在
        /// </summary>
       // public event ErrorHanlder NameReapt;
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.Drivers>().Any(item => item.ID == this.ID))
                {
                    repository.Update(this.ToLinq(), item => item.ID == this.ID);
                }
                else
                {
                    repository.Insert(this.ToLinq());
                }
                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        public void Abandon()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.Drivers>().Any(item => item.ID == this.ID))
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.Drivers>(new
                    {
                        Status = GeneralStatus.Deleted
                    }, item => item.ID == this.ID);
                }
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        #endregion
    }
}
