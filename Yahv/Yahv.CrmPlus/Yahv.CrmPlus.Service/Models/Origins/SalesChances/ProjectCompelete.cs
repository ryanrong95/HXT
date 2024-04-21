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
    /// 竞品
    /// </summary>
    public class ProjectCompelete: Yahv.Linq.IUnique, IDataEntity
    {
        #region  属性
        public string ID { get; set; }
        /// <summary>
        /// 销售机会ID
        /// </summary>

        public string ProjectID { get; set; }
        /// <summary>
        /// 销售机会产品项ID
        /// </summary>
        public string ProjectProductID { get; set; }
        /// <summary>
        /// 标准型号ID
        /// </summary>
        public string SpnID { get; set; }
        /// <summary>
        /// 型号ID
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CreatorID { get; set; }

        public  decimal UnitPrice { get; set; }

        public  DateTime CreateDate { get; set; }

        public DateTime ModifyDate { get; set; }

        public DataStatus DataStatus { get; set; }

        public ProjectCompelete()
        {
            this.CreateDate = this.ModifyDate = DateTime.Now;
            this.DataStatus = DataStatus.Normal;
        }

        #endregion



        #region 持久化
        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            {

                //添加
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.ProjectCompeletes>().Any(item =>item.ProjectID== this.ProjectID &&item.ProjectProductID==this.ProjectProductID &&item.SpnID==this.SpnID))
                {
                    this.ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.Compelete);
                    reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.ProjectCompeletes()
                    {
                        ID = this.ID,
                        ProjectID = this.ProjectID,
                        ProjectProductID=this.ProjectProductID,
                        SpnID = this.SpnID,
                        ProductID=this.ProductID,
                        CreatorID=this.CreatorID,
                        UnitPrice=this.UnitPrice,
                        Status=(int)this.DataStatus,
                        CreateDate=this.CreateDate,
                        ModifyDate=this.ModifyDate
                       

                    });
                }
                //修改
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvdCrm.ProjectCompeletes>(new
                    {
                        ProjectID = this.ProjectID,
                        ProjectProductID = this.ProjectProductID,
                        SpnID = this.SpnID,
                        ProductID = this.ProductID,
                        CreatorID = this.CreatorID,
                        UnitPrice = this.UnitPrice,
                        Status = (int)this.DataStatus,
                        CreateDate = this.CreateDate,
                        ModifyDate = this.ModifyDate
                    },  item => item.ProjectID == this.ProjectID && item.ProjectProductID == this.ProjectProductID && item.SpnID == this.SpnID);
                }

            }
            this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
        }

        public void Abandon()
        {
            using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvdCrm.ProjectCompeletes>(new
                {
                    Status = (int)DataStatus.Closed,
                    ModifyDate=DateTime.Now
                }, item => item.ID == this.ID);
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }



        #endregion

        #region 事件
        public event SuccessHanlder EnterSuccess;

        public event SuccessHanlder AbandonSuccess;
        /// <summary>
        /// EnterError
        /// </summary>

        public event ErrorHanlder EnterError;
        #endregion

      
    }
}
