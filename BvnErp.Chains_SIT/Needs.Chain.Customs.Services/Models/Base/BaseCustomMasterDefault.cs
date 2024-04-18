using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class BaseCustomMasterDefault : IUnique
    {
        #region 属性
        public string ID { get; set; }

        /// <summary>
        /// 申报地海关编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 申报地海关名称
        /// </summary>
        public string CodeName { get; set; }

        /// <summary>
        /// 进境关别
        /// </summary>
        public string IEPortCode { get; set; }

        /// <summary>
        /// 进境关别
        /// </summary>
        public string IEPortCodeName { get; set; }

        /// <summary>
        /// 入境口岸
        /// </summary>
        public string EntyPortCode { get; set; }

        /// <summary>
        /// 入境口岸
        /// </summary>
        public string EntyPortCodeName { get; set; }

        /// <summary>
        /// 检验检疫受理机关
        /// </summary>
        public string OrgCode { get; set; }

        /// <summary>
        /// 检验检疫受理机关
        /// </summary>
        public string OrgCodeName { get; set; }

        /// <summary>
        /// 领证机关
        /// </summary>

        public string VsaOrgCode { get; set; }

        /// <summary>
        /// 领证机关
        /// </summary>

        public string VsaOrgCodeName { get; set; }

        /// <summary>
        /// 口岸检验检疫机关
        /// </summary>
        public string InspOrgCode { get; set; }

        /// <summary>
        /// 口岸检验检疫机关
        /// </summary>
        public string InspOrgCodeName { get; set; }

        /// <summary>
        /// 目的地检验检疫机关
        /// </summary>
        public string PurpOrgCode { get; set; }

        /// <summary>
        /// 目的地检验检疫机关
        /// </summary>
        public string PurpOrgCodeName { get; set; }

        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault { get; set; }

        #endregion

        public BaseCustomMasterDefault()
        {

        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseCustomMasterDefault>().Where(item => item.ID == this.ID);

                //默认设置
                if (this.IsDefault)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.BaseCustomMasterDefault>(
                        new
                        {
                            IsDefault = false
                        }, item => item.IsDefault);
                }

                if (count.Count() == 0)
                {
                    this.ID = Guid.NewGuid().ToString("N");
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.BaseCustomMasterDefault
                    {
                        ID = this.ID,
                        Code = this.Code,
                        IEPortCode = this.IEPortCode,
                        EntyPortCode = this.EntyPortCode,
                        OrgCode = this.OrgCode,
                        VsaOrgCode = this.VsaOrgCode,
                        InspOrgCode = this.InspOrgCode,
                        PurpOrgCode = this.PurpOrgCode,
                        IsDefault = this.IsDefault
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.BaseCustomMasterDefault
                    {
                        ID = this.ID,
                        Code = this.Code,
                        IEPortCode = this.IEPortCode,
                        EntyPortCode = this.EntyPortCode,
                        OrgCode = this.OrgCode,
                        VsaOrgCode = this.VsaOrgCode,
                        InspOrgCode = this.InspOrgCode,
                        PurpOrgCode = this.PurpOrgCode,
                        IsDefault = this.IsDefault
                    }, item => item.ID == this.ID);
                }
            }

            this.OnEnter();
        }

        virtual public void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}
