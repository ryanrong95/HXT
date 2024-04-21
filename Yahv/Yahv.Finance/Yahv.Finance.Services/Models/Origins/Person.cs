using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data;
using Yahv.Finance.Services.Enums;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;

namespace Yahv.Finance.Services.Models.Origins
{
    public class Person : IUnique
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
        public string RealName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public GenderType? Gender { get; set; }

        /// <summary>
        /// 身份证信息
        /// </summary>
        public string IDCard { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 创建人
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

        #endregion

        #region 拓展属性
        /// <summary>
        /// 创建人名称
        /// </summary>
        public string CreatorName { get; set; }
        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                //添加
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.Persons>().Any(item => item.ID == this.ID))
                {
                    this.ID = this.ID ?? PKeySigner.Pick(Yahv.Finance.Services.PKeyType.Persons);

                    reponsitory.Insert(new Layers.Data.Sqls.PvFinance.Persons()
                    {
                        ID = this.ID,
                        RealName = this.RealName,
                        Gender = (int)this.Gender,
                        IDCard = this.IDCard,
                        Mobile = this.Mobile,
                        CreatorID = this.CreatorID,
                        ModifierID = this.CreatorID,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                        Status = (int)GeneralStatus.Normal,
                    });
                }
                //修改
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvFinance.Persons>(new
                    {
                        RealName = this.RealName,
                        Gender = (int)this.Gender,
                        IDCard = this.IDCard,
                        Mobile = this.Mobile,
                        ModifierID = this.ModifierID,
                        ModifyDate = DateTime.Now,
                    }, item => item.ID == this.ID);
                }

                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }

        #endregion

    }
}
