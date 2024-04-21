using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Usually;

namespace Yahv.CrmPlus.Service.Models.Origins
{
    public class MapsTopN : IUnique
    {
        public MapsTopN()
        {

        }
        #region 属性
        public string ID { set; get; }
        /// <summary>
        /// TOP10
        /// </summary>
        public int TopOrder { set; get; }

        /// <summary>
        /// 所有人 （AdminID）
        /// </summary>
        public string OwnerID { set; get; }
        /// <summary>
        /// 审批人
        /// </summary>
        public string ClientID { set; get; }

        #endregion

        #region 事件
        public virtual event SuccessHanlder EnterSuccess;
        public virtual event AbandonHanlder AbandonSuccess;
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            using (var tran = reponsitory.OpenTransaction())
            {
                {
                    var exist = reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.MapsTopN>().FirstOrDefault(item => item.ClientID == this.ClientID
                    && item.OwnerID == this.OwnerID);
                    if (exist == null)
                    {
                        this.ID = Guid.NewGuid().ToString();
                        reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.MapsTopN
                        {
                            ID = this.ID,
                            ClientID = this.ClientID,
                            TopOrder = this.TopOrder,
                            OwnerID = this.OwnerID
                        });
                    }
                    else
                    {

                        reponsitory.Update<Layers.Data.Sqls.PvdCrm.MapsTopN>(new
                        {

                            ID = this.ID,
                            ClientID = this.ClientID,
                            TopOrder = this.TopOrder,
                            OwnerID = this.OwnerID

                        }, item => item.ClientID == this.ClientID && item.OwnerID == this.OwnerID);
                    }

                }
                tran.Commit();
            }
            this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
        }


        public void CancelTop10()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            {
                reponsitory.Delete<Layers.Data.Sqls.PvdCrm.MapsTopN>(item => item.ClientID == this.ClientID &&item.OwnerID==this.OwnerID);
                this.AbandonSuccess?.Invoke(this, new AbandonedEventArgs(this));
            }
        }
        #endregion
    }
}
