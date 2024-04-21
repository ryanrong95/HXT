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
using YaHv.VcCsrm.Service.Extends;

namespace YaHv.VcCsrm.Service.Models
{
    public class Consignee : Yahv.Linq.IUnique
    {
        #region 属性
        string id;
        /// <summary>
        /// 
        /// </summary>
        virtual public string ID
        {
            get
            {
                return this.id ?? string.Join("",
                    this.EnterpriseID,
                    this.DyjCode,
                    this.District,
                    this.Address,
                    this.Postzip
                    ).MD5();
            }
            set { this.id = value; }
        }
        public string EnterpriseID { set; get; }
        /// <summary>
        /// 名称，主要用于库房的子库房名称
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 大赢家Code
        /// </summary>
        public string DyjCode { set; get; }
        /// <summary>
        /// 地区
        /// </summary>
        public District District { set; get; }
        /// <summary>
        /// 省
        /// </summary>
        public string Province { set; get; }
        /// <summary>
        /// 市
        /// </summary>
        public string City { set; get; }
        /// <summary>
        /// 地
        /// </summary>
        public string Land { set; get; }
        /// <summary>
        /// 收货地址
        /// </summary>
        public string Address { set; get; }
        /// <summary>
        /// 邮编
        /// </summary>
        public string Postzip { set; get; }
        /// <summary>
        /// 联系人类型
        /// </summary>
       // public ContactType Type { get; set; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 联系人手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 联系人邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; internal set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate { get; internal set; }
        /// <summary>
        /// 公司基本信息
        /// </summary>
        public Enterprise Enterprise { set; get; }
        public string CreatorID { set; get; }
        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault { set; get; }
        /// <summary>
        /// 门牌编码，库房使用
        /// </summary>
        public string PlateCode { set; get; }
        #endregion
        #region 事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        virtual public event SuccessHanlder EnterSuccess;
        /// <summary>
        /// AbandonSuccess
        /// </summary>
        virtual public event SuccessHanlder AbandonSuccess;
        #endregion

        #region 持久化
        virtual public void Enter()
        {
            using (var repository = LinqFactory<PvcCrmReponsitory>.Create())
            {
                //设置默认
                if (this.IsDefault)
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.Consignees>(new
                    {
                        IsDefault = false
                    }, item => item.EnterpriseID == this.EnterpriseID);
                }
                //Consignee
                if (repository.GetTable<Layers.Data.Sqls.PvbCrm.Consignees>().Any(item => item.ID == this.ID))
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
        virtual public void Abandon()
        {
            using (var repository = LinqFactory<PvcCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvcCrm.Consignees>(new
                {
                    Status = GeneralStatus.Deleted
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
