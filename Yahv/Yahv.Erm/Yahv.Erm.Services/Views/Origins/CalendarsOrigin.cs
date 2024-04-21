using System.Linq;
using Layers.Data.Sqls;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Origins
{
    /// <summary>
    /// 自然日视图
    /// </summary>
    public class CalendarsOrigin : QueryView<Calendar, PvbErmReponsitory>
    {
        public CalendarsOrigin()
        {

        }

        public CalendarsOrigin(PvbErmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Calendar> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Calendars>()
                   select new Calendar()
                   {
                       ID = entity.ID,
                       Day = entity.Day,
                       Month = entity.Month,
                       Week = entity.Week,
                       Year = entity.Year,
                   };
        }
    }
}