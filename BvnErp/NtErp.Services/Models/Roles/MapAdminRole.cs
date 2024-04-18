using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Linq;

namespace NtErp.Services.Models
{
    /// <summary>
    /// 角色管理员关系
    /// </summary>
    public class MapAdminRole : IMapAdminRole
    {
        public MapAdminRole()
        {

        }
        public string RoleID { get; set; }

        public string AdminID { get; set; }

        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;

        public void Abandon()
        {
            try
            {
                using (var repository = new Layer.Data.Sqls.BvnErpReponsitory())
                {
                    repository.Delete<Layer.Data.Sqls.BvnErp.MapsAdminRole>(item => item.RoleID == this.RoleID && item.AdminID == this.AdminID);
                }

                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }

            }
            catch (Exception ex)
            {

                if (this != null && this.AbandonError != null)
                {
                    this.AbandonError(this, new ErrorEventArgs($"Abandon Fail !!,{ex.Message}"));
                }

            }
        }

        public void Enter()
        {
            try
            {
                using (var repository = new Layer.Data.Sqls.BvnErpReponsitory())
                {
                    if (!repository.ReadTable<Layer.Data.Sqls.BvnErp.MapsAdminRole>().Any(item => item.RoleID == this.RoleID && item.AdminID == this.AdminID))
                    {
                        repository.Insert(new Layer.Data.Sqls.BvnErp.MapsAdminRole
                        {
                            RoleID = this.RoleID,
                            AdminID = this.AdminID
                        });
                    }
                }

                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
            catch (Exception ex)
            {
                if (this != null && this.EnterError != null)
                {
                    this.EnterError(this, new ErrorEventArgs($"Enter Fail !!,{ex.Message}", ErrorType.Customer));
                }
            }

        }
    }
}
