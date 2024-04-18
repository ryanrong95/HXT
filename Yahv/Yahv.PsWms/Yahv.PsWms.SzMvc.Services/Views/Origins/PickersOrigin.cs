using Layers.Data.Sqls;
using System.Linq;
using Yahv.Linq;
using Yahv.PsWms.SzMvc.Services.Enums;

namespace Yahv.PsWms.SzMvc.Services.Views.Origins
{

    public class PickersOrigin : UniqueView<Models.Origin.Picker, PsOrderRepository>
    {
        #region 构造函数
        public PickersOrigin()
        {
        }

        public PickersOrigin(PsOrderRepository reponsitory) : base(reponsitory)
        {
        }

        #endregion

        protected override IQueryable<Models.Origin.Picker> GetIQueryable()
        {
            var view = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PsOrder.Pickers>()
                       select new Models.Origin.Picker
                       {
                           ID = entity.ID,
                           ClientID = entity.ClientID,
                           IDType = (IDType)entity.IDType,
                           IDCode = entity.IDCode,
                           Contact = entity.Contact,
                           Phone = entity.Phone,
                           Address = entity.Address,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                           Status = (Underly.GeneralStatus)entity.Status,
                       };
            return view;
        }
    }
}
