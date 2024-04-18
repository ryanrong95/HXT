using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models.PveCrm;
using Yahv.Underly;

namespace Yahv.Services.Views.PveCrm
{
    /// <summary>
    /// 客户
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class ClientsAll<TReponsitory> : UniqueView<Client, TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public ClientsAll()
        {

        }
        public ClientsAll(TReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Client> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PveCrm.ClientsTopView>()
                   join area in this.Reponsitory.ReadTable<Layers.Data.Sqls.PveCrm.EnumsDictionariesTopView>() on entity.AreaType equals area.ID
                   select new Client
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Nature = (Underly.CrmPlus.ClientType)entity.Nature,//客户性质
                       //AreaType = entity.AreaType,//客户地区类型
                       //AreaTypeDes = area.Description,
                       Grade = (ClientGrade)entity.Grade,
                       Status = (AuditStatus)entity.Status,
                       TaxperNumber = entity.TaxperNumber,
                       District = entity.District,
                       Vip = (VIPLevel)entity.Vip,
                       DyjCode = entity.DyjCode,
                       IsNew = entity.IsNew
                   };
        }
    }
}
