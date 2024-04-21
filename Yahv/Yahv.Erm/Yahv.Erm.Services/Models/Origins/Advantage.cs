using Layers.Data.Sqls;
using Layers.Linq;
using System.Linq;
using Yahv.Usually;

namespace Yahv.Erm.Services.Models.Origins
{
    public class Advantage
    {
        #region 属性
        string id;
        public string AdminID
        {
            get
            {
                return this.Admin.ID;

            }
            set
            {
                this.id = value;
            }
        }
        public string Manufacturers { set; get; }
        public string PartNumbers { set; get; }
        public Admin Admin { set; get; }

        #endregion

        #region 事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                if (repository.GetTable<Layers.Data.Sqls.PvbErm.Advantages>().Any(item => item.AdminID == this.AdminID))
                {
                    repository.Update<Layers.Data.Sqls.PvbErm.Advantages>(new
                    {
                        Manufacturers = this.Manufacturers,
                        PartNumbers = this.PartNumbers
                    }, item => item.AdminID == this.AdminID);
                }
                else
                {
                    repository.Insert<Layers.Data.Sqls.PvbErm.Advantages>(new Layers.Data.Sqls.PvbErm.Advantages
                    {
                        AdminID = this.AdminID,
                        Manufacturers = this.Manufacturers,
                        PartNumbers = this.PartNumbers
                    });
                }
                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
        }

        #endregion


    }

}
