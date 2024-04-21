using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Extends;
using Yahv.Linq;

namespace Yahv.CrmPlus.Service.Models.Origins
{
    /// <summary>
    /// 管控
    /// </summary>
    public class Control : IUnique
    {
        public Control()
        {
            this.CreateDate = DateTime.Now;
        }
        #region 属性
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { set; get; }
        /// <summary>
        /// 品牌或型号的ID
        /// </summary>
        public string MainID { set; get; }
        /// <summary>
        /// 类型：品牌，型号
        /// </summary>
        public Yahv.Underly.Enums.ControlType Type { set; get; }
        /// <summary>
        /// 管控内容
        /// </summary>
        public string Context { set; get; }
        /// <summary>
        /// 状态：黑名单，白名单
        /// </summary>
        public Yahv.Underly.Enums.ControlStatus Status { set; get; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorID { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { set; get; }
        #endregion

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            {
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    this.ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.Control);
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvdCrm.Controls>(new
                    {
                        Context = this.Context
                    }, item => item.ID == this.ID);
                }
            }
        }

    }
}
