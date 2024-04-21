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
    public class vBrand : IUnique
    {
        public string ID { set; get; }
        public string BrandID { set; get; }
        public string RoleID { set; get; }
        public string RoleName { set; get; }
        public string AdminID { set; get; }
        public string RealName { internal set; get; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        public event SuccessHanlder EnterSuccess;

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            {
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.vBrands>().Any(item => item.BrandID == this.BrandID && item.RoleID == this.RoleID && item.AdminID == this.AdminID))
                {
                    this.ID = Guid.NewGuid().ToString("N");
                    reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.vBrands
                    {
                        ID = this.ID,
                        BrandID = this.BrandID,
                        RoleID = this.RoleID,
                        AdminID = this.AdminID
                    });
                }
                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }
        public void Abandon()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            {
                reponsitory.Delete<Layers.Data.Sqls.PvdCrm.vBrands>(item => item.ID == this.ID);
            }
        }
    }
}
