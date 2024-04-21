using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Origins
{
    /// <summary>
    /// 打卡记录视图
    /// </summary>
    internal class Logs_AttendOrigin : UniqueView<Logs_Attend, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        internal Logs_AttendOrigin() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        internal Logs_AttendOrigin(PvbErmReponsitory repository) : base(repository) { }

        protected override IQueryable<Logs_Attend> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Logs_Attend>()
                   select new Logs_Attend()
                   {
                       ID = entity.ID,
                       Date = entity.Date,
                       StaffID = entity.StaffID,
                       CreateDate = entity.CreateDate,
                       IP = entity.IP,
                   };
        }
    }
}
