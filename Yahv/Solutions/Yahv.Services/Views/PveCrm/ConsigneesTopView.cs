using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models.PveCrm;

namespace Yahv.Services.Views.PveCrm
{
    /// <summary>
    /// 地址通用视图
    /// </summary>
    public class ConsigneesTopView<TReponsitory> : UniqueView<Consignee, TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ConsigneesTopView()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public ConsigneesTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Consignee> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PveCrm.ConsigneesTopView>()
                   join area in this.Reponsitory.ReadTable<Layers.Data.Sqls.PveCrm.EnumsDictionariesTopView>() on entity.District equals area.ID
                   select new Consignee
                   {
                       ID = entity.ID,
                       EnterpriseID = entity.EnterpriseID,
                       Title = entity.Title,
                       DyjCode = entity.DyjCode,
                       District = entity.District,
                       DistrictDesc = area.Description,
                       Address = entity.Address,
                       Postzip = entity.PostZip,
                       Status = (Underly.AuditStatus)entity.Status,
                       Name = entity.Name,
                       Tel = entity.Tel,
                       Mobile = entity.Mobile,
                       Email = entity.Email,
                       //Place = entity.Place,
                       Enterprise = new Enterprise
                       {
                           ID = entity.EnterpriseID,
                           Name = entity.Name,
                           RegAddress = entity.RegAddress,
                           Uscc = entity.Uscc,
                           Corporation = entity.Corperation,
                           Place = entity.Place
                       }
                   };
        }
    }
}
