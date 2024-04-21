using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Origins;
using Yahv.Linq;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Configuration;
using Yahv.Erm.Services.Models.Rolls;

namespace Yahv.Erm.Services.Views.Rolls
{
    /// <summary>
    /// 员工
    /// </summary>
    public class StaffsRoll : UniqueView<Staff, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public StaffsRoll()
        {
        }

        public StaffsRoll(PvbErmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Staff> GetIQueryable()
        {
            var staffView = new StaffsOrigin(this.Reponsitory);

            return from staff in staffView
                   join per in Reponsitory.ReadTable<Personals>() on staff.ID equals per.ID
                   join lab in Reponsitory.ReadTable<Labours>() on staff.ID equals lab.ID
                   select new Staff()
                   {
                       ID = staff.ID,
                       Status = (StaffStatus)staff.Status,
                       CreateDate = staff.CreateDate,
                       SelCode = staff.SelCode,
                       UpdateDate = staff.UpdateDate,
                       Name = staff.Name,
                       Code = staff.Code,
                       Gender = staff.Gender,
                       AdminID = staff.AdminID,
                       LeagueID = staff.LeagueID,
                       DyjDepartmentCode = staff.DyjDepartmentCode,
                       AssessmentTime = staff.AssessmentTime,
                       AssessmentMethod = staff.AssessmentMethod,
                       DyjCompanyCode = staff.DyjCompanyCode,
                       WorkCity = staff.WorkCity,
                       EnterpriseID = lab.EnterpriseID,
                       PostionID = staff.PostionID,
                       IDCard = per.IDCard,
                       EntryDate = lab.EntryDate,
                       LeaveDate = lab.LeaveDate,
                       DyjCode = staff.DyjCode,

                       DepartmentCode = staff.DepartmentCode,
                       PostionCode = staff.PostionCode,

                       RegionID = staff.RegionID,
                       SchedulingID = staff.SchedulingID,
                   };
        }

        /// <summary>
        /// 获取需要同步的dayjIds
        /// </summary>
        /// <param name="dyjIds"></param>
        /// <returns></returns>
        public string[] GetDyjIdsNotExist(string[] dyjIds)
        {
            //离职、废弃、注销
            var statusIds = new int[]
            {
                300,400,500
            };

            var dyjCodes = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Staffs>()
                .Where(item => !statusIds.Contains(item.Status)).Select(item => item.DyjCode).Distinct().ToArray();

            //not in 
            return dyjIds.Except(dyjCodes).ToArray();
        }

        /// <summary>
        /// 更新芯达通员工的大赢家Id（大赢家Id为空的）
        /// </summary>
        /// <param name="array"></param>
        public void UpdateDyjCodeToXdt(StaffContactDto[] array)
        {
            //离职、废弃、注销
            var statusIds = new int[]
            {
                300,400,500
            };

            var companies = ConfigurationManager.AppSettings["XdtCompany"]
                .Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            //没有大赢家编码的员工
            var staffs = (from linq in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Staffs>()
                          join l in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Labours>() on linq.ID equals l.ID
                          where !statusIds.Contains(linq.Status) && linq.DyjCode == null && companies.Contains(l.ID)
                          select linq).ToList();

            if (staffs != null && staffs.Any())
            {
                StaffContactDto dto;
                foreach (var staff in staffs)
                {
                    //根据名字更新；大赢家名字是唯一的
                    dto = array.FirstOrDefault(item => item.姓名 == staff.Name);

                    if (dto != null && dto.ID > 0)
                    {
                        this.Reponsitory.Update<Layers.Data.Sqls.PvbErm.Staffs>(new
                        {
                            DyjCode = dto.ID.ToString()
                        }, item => item.ID == staff.ID);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 历史员工
    /// </summary>
    public class StaffsBackgroundRoll : UniqueView<Staff, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public StaffsBackgroundRoll()
        {
        }

        public StaffsBackgroundRoll(PvbErmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Staff> GetIQueryable()
        {
            var personalView = new PersonalsOrigin(this.Reponsitory);
            var labourView = new LaboursOrigin(this.Reponsitory);
            var positionView = new PostionsOrigin(this.Reponsitory);

            var linq = from staff in Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Staffs>()
                       join personal in personalView on staff.ID equals personal.ID
                       join labour in labourView on staff.ID equals labour.ID into labours
                       from labour in labours.DefaultIfEmpty()
                       join position in positionView on staff.PostionID equals position.ID into positions
                       from position in positions.DefaultIfEmpty()
                       where staff.Status != (int)StaffStatus.Delete
                       select new Staff
                       {
                           ID = staff.ID,
                           Name = staff.Name,
                           Code = staff.Code,
                           SelCode = staff.SelCode,
                           Gender = (Gender)staff.Gender,
                           DyjCompanyCode = staff.DyjCompanyCode,
                           DyjDepartmentCode = staff.DyjDepartmentCode,
                           DyjCode = staff.DyjCode,
                           WorkCity = staff.WorkCity,
                           LeagueID = staff.LeagueID,
                           PostionID = staff.PostionID,
                           DepartmentCode = staff.DepartmentCode,
                           PostionCode = staff.PostionCode,
                           AssessmentMethod = staff.AssessmentMethod,
                           AssessmentTime = staff.AssessmentTime,
                           AdminID = staff.AdminID,
                           UpdateDate = staff.UpdateDate,
                           CreateDate = staff.CreateDate,
                           Status = (StaffStatus)staff.Status,
                           RegionID = staff.RegionID,
                           SchedulingID = staff.SchedulingID,

                           Personal = personal,
                           Labour = labour,
                           Postion = position
                       };
            return linq;
        }

        /// <summary>
        /// 获取员工的背景调查情况
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="expressions"></param>
        /// <returns></returns>
        public IQueryable<Staff> GetStaffsBackground(Expression<Func<Staff, bool>> expression, params LambdaExpression[] expressions)
        {
            //过滤考勤结果
            var staffs = this.GetIQueryable();
            foreach (var predicate in expressions)
            {
                staffs = staffs.Where(predicate as Expression<Func<Staff, bool>>);
            }
            staffs = staffs.Where(expression);
            return staffs;
        }
    }
}