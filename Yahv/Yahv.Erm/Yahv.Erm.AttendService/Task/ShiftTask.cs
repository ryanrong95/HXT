using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Erm.Services;
using Yahv.Utils.Serializers;

namespace Yahv.Erm.AttendService
{
    /// <summary>
    /// 员工假期初始化任务
    /// </summary>
    public class ShiftTask
    {
        public ShiftTask()
        {
        }

        public static void Shift()
        {
            var shifts = new Erm.Services.Views.Alls.ShiftStaffsAll();

            using (var repository = new PvbErmReponsitory())
            {
                foreach (var shift in shifts)
                {
                    //更新员工班别
                    repository.Update<Layers.Data.Sqls.PvbErm.Staffs>(new
                    {
                        SchedulingID = shift.NextSchedulingID,
                    }, item => item.ID == shift.ID);

                    //更新员工下次倒班数据
                    var schedulings = shift.ShiftSchedule.Split(',');
                    foreach (var sche in schedulings)
                    {
                        if (sche != shift.NextSchedulingID && !string.IsNullOrEmpty(sche))
                        {
                            repository.Update<Layers.Data.Sqls.PvbErm.ShiftStaffs>(new
                            {
                                NextSchedulingID = sche,
                                UpdateDate = DateTime.Now,
                            }, item => item.ID == shift.ID);
                            break;
                        }
                    }
                }
            }
        }

        //所有换班的员工周一都设置为A班
        public static void SetScheduleA()
        {
            var shifts = new Erm.Services.Views.Alls.ShiftStaffsAll();
            using (var repository = new PvbErmReponsitory())
            {
                foreach (var shift in shifts)
                {
                    repository.Update<Layers.Data.Sqls.PvbErm.Staffs>(new
                    {
                        SchedulingID = "Scheing00011",
                    }, item => item.ID == shift.ID);
                }
            }
        }
    }
}
