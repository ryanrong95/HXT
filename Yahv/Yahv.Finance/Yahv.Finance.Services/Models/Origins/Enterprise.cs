using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;

namespace Yahv.Finance.Services.Models.Origins
{
    public class Enterprise : IUnique
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

        #region 数据库属性

        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public Yahv.Finance.Services.Enums.EnterpriseAccountType Type { get; set; }

        /// <summary>
        /// 地区
        /// </summary>
        public string District { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public string ModifierID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Summary { get; set; }

        #endregion

        #region 其它属性

        /// <summary>
        /// 创建人姓名
        /// </summary>
        public string CreatorName { get; set; }

        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                //添加
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.Enterprises>().Any(item => item.ID == this.ID))
                {
                    this.ID = this.ID ?? PKeySigner.Pick(PKeyType.Enterprise);

                    reponsitory.Insert(new Layers.Data.Sqls.PvFinance.Enterprises()
                    {
                        ID = this.ID,
                        Name = this.Name,
                        Type = (int)this.Type,
                        District = this.District,
                        CreatorID = this.CreatorID,
                        ModifierID = this.CreatorID,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                        Status = (int)GeneralStatus.Normal,
                        Summary = this.Summary,
                    });
                }
                //修改
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvFinance.Enterprises>(new
                    {
                        Name = this.Name,
                        Type = (int)this.Type,
                        District = this.District,
                        ModifierID = this.ModifierID,
                        ModifyDate = DateTime.Now,
                        Summary = this.Summary,
                    }, item => item.ID == this.ID);
                }

                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }

        #endregion

    }
}
