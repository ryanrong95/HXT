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

namespace Yahv.CrmPlus.Service.Models.Origins
{
    /// <summary>
    /// 业务类型
    /// </summary>
    public class Conduct: IUnique
    {
        #region  属性
        public string ID { get; set; }
        /// <summary>
        /// 企业ID
        /// </summary>

        public string EnterpriseID { get; set; }
       /// <summary>
       /// 业务类型
       /// </summary>
        public ConductType ConductType { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        public ConductGrade Grade { get; set; }

        /// <summary>
        /// 是否公海
        /// </summary>
        public bool IsPublic { get; set; }

        #endregion 

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            {
                if (!reponsitory.GetTable<Layers.Data.Sqls.PvdCrm.Conducts>().Any(x => x.EnterpriseID == this.EnterpriseID &&x.Type==(int)this.ConductType))
                {
                    this.ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.Conducts);
                    reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.Conducts()
                    {
                        ID = this.ID,
                        EnterpriseID=this.EnterpriseID,
                        Type=(int)this.ConductType,
                        Grade=(int)this.Grade,
                        IsPublic=this.IsPublic
                    });
                }
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvdCrm.Conducts>(new
                    {
                        EnterpriseID = this.EnterpriseID,
                        Type = (int)this.ConductType,
                        Grade = (int)this.Grade,
                        IsPublic = this.IsPublic
                    },  x => x.EnterpriseID == this.EnterpriseID && x.Type == (int)this.ConductType);

                }

                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }

        }
        #endregion


        #region  事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// EnterError
        /// </summary>
        public event ErrorHanlder EnterError;


        public event SuccessHanlder AbandonSuccess;

        #endregion


    }



}
