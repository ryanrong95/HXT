using System;
using System.Linq;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Underly.Attributes;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;
using YaHv.Erm.Services.Views.Rolls;

namespace Yahv.Erm.Services.Models.Origins
{
    /// <summary>
    /// 印章证照
    /// </summary>
    public class SealCertificate : IUnique
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

        /// <summary>
        /// 唯一码:Adm001
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 企业ID
        /// </summary>
        public string EnterpriseID { get; set; }

        /// <summary>
        /// 单证名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 印章证照类型
        /// </summary>
        public SealCertificateType Type { get; set; }

        /// <summary>
        /// 办理日期
        /// </summary>
        public DateTime? ProcessingDate { get; set; }

        /// <summary>
        /// 到期日期
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// 添加人
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 保管人
        /// </summary>
        public string StaffID { get; set; }

        public GeneralStatus Status { get; set; }
        #endregion

        #region 扩展属性

        public Admin Admin { get; set; }

        public Staff Staff { get; set; }

        #endregion

        public SealCertificate()
        {
            this.Status = GeneralStatus.Normal;
            this.Type = SealCertificateType.其它文件;
        }

        #region 持久化

        /// <summary>
        /// 添加/修改
        /// </summary>
        public void Enter()
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                if (!repository.ReadTable<SealCertificates>().Any(t => t.ID == this.ID))
                {
                    this.ID = PKeySigner.Pick(PKeyType.SealCtf);
                    repository.Insert(new SealCertificates()
                    {
                        ID = this.ID,
                        EnterpriseID = this.EnterpriseID,
                        Name = this.Name,
                        Type = (int)this.Type,
                        ProcessingDate = this.ProcessingDate,
                        DueDate = this.DueDate,
                        AdminID = this.AdminID,
                        StaffID = this.StaffID,
                        Status = (int)this.Status,
                    });
                }
                else
                {
                    repository.Update<SealCertificates>(new
                    {
                        EnterpriseID = this.EnterpriseID,
                        Name = this.Name,
                        Type = (int)this.Type,
                        ProcessingDate = this.ProcessingDate,
                        DueDate = this.DueDate,
                        AdminID = this.AdminID,
                        StaffID = this.StaffID,
                    }, a => a.ID == this.ID);
                }

                //操作成功
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
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                repository.Update<SealCertificates>(new
                {
                    Status = GeneralStatus.Closed
                }, item => item.ID == this.ID);

                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// 印章单证类型
    /// </summary>
    public enum SealCertificateType
    {
        [Description("印章")]
        印章 = 1,

        [Description("证照")]
        证照 = 2,

        [Description("IC卡")]
        IC卡 = 3,

        [Description("其它文件")]
        其它文件 = 99,
    }
}
