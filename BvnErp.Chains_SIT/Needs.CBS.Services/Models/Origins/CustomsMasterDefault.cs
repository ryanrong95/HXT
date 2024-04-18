using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Cbs.Services.Models.Origins
{
    /// <summary>
    /// 海关申报地默认关联
    /// </summary>
    public class CustomsMasterDefault : IUnique
    {
        #region 属性

        string id;
        public string ID
        {
            get
            {
                //主键编码规则：Code.MD5
                return this.id ?? this.Code.MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 申报地海关代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 进境关别
        /// </summary>
        public string IEPortCode { get; set; }

        /// <summary>
        /// 入境口岸
        /// </summary>
        public string EntyPortCode { get; set; }

        /// <summary>
        /// 检验检疫受理机关
        /// </summary>
        public string OrgCode { get; set; }

        /// <summary>
        /// 领证机关
        /// </summary>
        public string VsaOrgCode { get; set; }

        /// <summary>
        /// 口岸检验检疫机关
        /// </summary>
        public string InspOrgCode { get; set; }

        /// <summary>
        /// 目的地检验检疫机关
        /// </summary>
        public string PurpOrgCode { get; set; }

        #endregion

        public CustomsMasterDefault()
        {

        }

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.Customs.CustomsMasterDefaults>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.Customs.CustomsMasterDefaults
                    {
                        ID = this.ID,
                        Code = this.Code,
                        IEPortCode = this.IEPortCode,
                        EntyPortCode = this.EntyPortCode,
                        OrgCode = this.OrgCode,
                        VsaOrgCode = this.VsaOrgCode,
                        InspOrgCode = this.InspOrgCode,
                        PurpOrgCode = this.PurpOrgCode
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.Customs.CustomsMasterDefaults>(new
                    {
                        Code = this.Code,
                        IEPortCode = this.IEPortCode,
                        EntyPortCode = this.EntyPortCode,
                        OrgCode = this.OrgCode,
                        VsaOrgCode = this.VsaOrgCode,
                        InspOrgCode = this.InspOrgCode,
                        PurpOrgCode = this.PurpOrgCode
                    }, item => item.ID == this.ID);
                }
            }

            this.OnEnterSuccess();
        }

        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.CustomsTariffs>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            }

            this.OnAbandonSuccess();
        }

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}
