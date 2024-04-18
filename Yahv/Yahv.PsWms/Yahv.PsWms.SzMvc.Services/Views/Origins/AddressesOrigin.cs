using Layers.Data.Sqls;
using System.Linq;
using Yahv.Linq;
using Yahv.PsWms.SzMvc.Services.Enums;

namespace Yahv.PsWms.SzMvc.Services.Views.Origins
{

    public class AddressesOrigin : UniqueView<Models.Origin.Address, PsOrderRepository>
    {
        #region 构造函数
        public AddressesOrigin()
        {
        }

        public AddressesOrigin(PsOrderRepository reponsitory) : base(reponsitory)
        {
        }

        #endregion

        protected override IQueryable<Models.Origin.Address> GetIQueryable()
        {
            var view = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PsOrder.Addresses>()
                       select new Models.Origin.Address
                       {
                           ID = entity.ID,
                           Type = (AddressType)entity.Type,
                           ClientID = entity.ClientID,
                           Title = entity.Title,
                           Contact = entity.Contact,
                           ClientAddress = entity.Address,
                           Phone = entity.Phone,
                           Email = entity.Email,
                           Status = (Underly.GeneralStatus)entity.Status,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                       };
            return view;
        }
    }
}
