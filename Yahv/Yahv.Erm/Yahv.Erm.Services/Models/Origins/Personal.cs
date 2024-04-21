using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Usually;
using System;
using System.Collections.Generic;
using Yahv.Underly;
using System.Threading.Tasks;

namespace Yahv.Erm.Services.Models.Origins
{
    /// <summary>
    /// 员工个人信息
    /// </summary>
    public class Personal : IUnique
    {
        #region 事件

        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// EnterError
        /// </summary>
        public event ErrorHanlder EnterError;

        /// <summary>
        /// AbandonSuccess
        /// </summary>
        public event SuccessHanlder AbandonSuccess;

        #endregion

        #region 属性
        /// <summary>
        /// StaffID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IDCard { get; set; }

        /// <summary>
        /// 肖像照
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// 籍贯
        /// </summary>
        public string NativePlace { get; set; }

        /// <summary>
        /// 家庭地址
        /// </summary>
        public string HomeAddress { get; set; }

        /// <summary>
        /// 户口所在地
        /// </summary>
        public string PassAddress { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        public string Volk { get; set; }

        /// <summary>
        /// 政治面貌
        /// </summary>
        public string PoliticalOutlook { get; set; }

        /// <summary>
        /// 身高
        /// </summary>
        public double? Height { get; set; }

        /// <summary>
        /// 体重
        /// </summary>
        public double? Weight { get; set; }

        /// <summary>
        /// 血型
        /// </summary>
        public string Blood { get; set; }

        /// <summary>
        /// 学历
        /// </summary>
        public string Education { get; set; }

        /// <summary>
        /// 毕业院校
        /// </summary>
        public string GraduatInstitutions { get; set; }

        /// <summary>
        /// 所学专业
        /// </summary>
        public string Major { get; set; }

        /// <summary>
        /// 婚否
        /// </summary>
        public bool? IsMarry { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// 参加工作时间
        /// </summary>
        public DateTime? BeginWorkDate { get; set; }

        /// <summary>
        /// 毕业时间
        /// </summary>
        public DateTime? GraduationDate { get; set; }

        /// <summary>
        /// 健康状况
        /// </summary>
        public string Healthy { get; set; }

        /// <summary>
        /// 紧急联系人
        /// </summary>
        public string EmergencyContact { get; set; }

        /// <summary>
        /// 紧急联系人电话
        /// </summary>
        public string EmergencyMobile { get; set; }

        /// <summary>
        /// 外语水平
        /// </summary>
        public string LanguageLevel { get; set; }

        /// <summary>
        /// 计算机水平
        /// </summary>
        public string ComputerLevel { get; set; }

        /// <summary>
        /// 自我评价
        /// </summary>
        public string SelfEvaluation { get; set; }

        /// <summary>
        /// 待遇需求
        /// </summary>
        public string Treatment { get; set; }

        /// <summary>
        /// 应聘职位
        /// </summary>
        public string PositionName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }

        #endregion

        #region 扩展属性
        public int? Age
        {
            get
            {
                var age = DateTime.Now.Year - this.BirthDate?.Year;
                return age;
            }
        }

        /// <summary>
        /// 工作经历
        /// </summary>
        IEnumerable<PersonalWorkExperience> workItems;
        public IEnumerable<PersonalWorkExperience> Workitems
        {
            get
            {
                if (this.workItems == null)
                {
                    this.workItems = new Views.Origins.PersonalWorkExperiencesOrigin().Where(item => item.StaffID == this.ID);
                }
                return this.workItems;
            }
            set
            {
                this.workItems = value;
            }
        }

        /// <summary>
        /// 家庭成员
        /// </summary>
        IEnumerable<PersonalFamilyMember> familyItems;
        public IEnumerable<PersonalFamilyMember> Familyitems
        {
            get
            {
                if (this.familyItems == null)
                {
                    this.familyItems = new Views.Origins.PersonalFamilyMembersOrigin().Where(item => item.StaffID == this.ID);
                }
                return this.familyItems;
            }
            set
            {
                this.familyItems = value;
            }
        }

        #endregion

        #region 持久化

        public void Enter()
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                //添加
                if (!repository.ReadTable<Personals>().Any(t => t.ID == this.ID))
                {
                    repository.Insert(new Personals()
                    {
                        ID = this.ID,       //StaffID
                        Blood = this.Blood,
                        Education = this.Education,
                        GraduatInstitutions = this.GraduatInstitutions,
                        Height = this.Height,
                        HomeAddress = this.HomeAddress,
                        IDCard = this.IDCard,
                        Image = this.Image,
                        IsMarry = this.IsMarry,
                        Major = this.Major,
                        NativePlace = this.NativePlace,
                        PassAddress = this.PassAddress,
                        PoliticalOutlook = this.PoliticalOutlook,
                        Volk = this.Volk,
                        Weight = this.Weight,
                        Mobile = this.Mobile,
                        Email = this.Email,
                        BirthDate = this.BirthDate,
                        BeginWorkDate = this.BeginWorkDate,
                        GraduationDate = this.GraduationDate,
                        Healthy = this.Healthy,
                        EmergencyContact = this.EmergencyContact,
                        EmergencyMobile = this.EmergencyMobile,
                        LanguageLevel = this.LanguageLevel,
                        ComputerLevel = this.ComputerLevel,
                        SelfEvaluation = this.SelfEvaluation,
                        Treatment = this.Treatment,
                        PositionName = this.PositionName,
                        Summary = this.Summary,
                    });
                }
                //修改
                else
                {
                    repository.Update<Personals>(new
                    {
                        Blood = this.Blood,
                        Education = this.Education,
                        GraduatInstitutions = this.GraduatInstitutions,
                        Height = this.Height,
                        HomeAddress = this.HomeAddress,
                        IDCard = this.IDCard,
                        Image = this.Image,
                        IsMarry = this.IsMarry,
                        Major = this.Major,
                        NativePlace = this.NativePlace,
                        PassAddress = this.PassAddress,
                        PoliticalOutlook = this.PoliticalOutlook,
                        Volk = this.Volk,
                        Weight = this.Weight,
                        Mobile = this.Mobile,
                        Email = this.Email,
                        BirthDate = this.BirthDate,
                        BeginWorkDate = this.BeginWorkDate,
                        GraduationDate = this.GraduationDate,
                        Healthy = this.Healthy,
                        EmergencyContact = this.EmergencyContact,
                        EmergencyMobile = this.EmergencyMobile,
                        LanguageLevel = this.LanguageLevel,
                        ComputerLevel = this.ComputerLevel,
                        SelfEvaluation = this.SelfEvaluation,
                        Treatment = this.Treatment,
                        PositionName = this.PositionName,
                        Summary = this.Summary,
                    }, t => t.ID == this.ID);
                }

                //保存工作经历
                var oldWorks = new Views.Origins.PersonalWorkExperiencesOrigin().Where(item => item.StaffID == this.ID).ToList();
                Task t1 = new Task(() =>
                {
                    SaveWorkExperiences(this.workItems, oldWorks);
                });
                t1.Start();
                //保存家庭成员
                var oldFamilys = new Views.Origins.PersonalFamilyMembersOrigin().Where(item => item.StaffID == this.ID).ToList();
                Task t2 = new Task(() =>
                {
                    SaveFamilyMembers(this.familyItems, oldFamilys);
                });
                t2.Start();
                Task.WaitAll(t1, t2);

                //操作成功
                if (this != null && EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
        }

        public void Abandon()
        {

        }

        #endregion

        /// <summary>
        /// 保存工作经历
        /// </summary>
        /// <param name="newFiles"></param>
        /// <param name="oldFiles"></param>
        private void SaveWorkExperiences(IEnumerable<PersonalWorkExperience> news, IEnumerable<PersonalWorkExperience> olds)
        {
            if (news == null)
            {
                return;
            }
            string[] newids = news.Select(item => item.ID).ToArray();
            string[] oldids = olds.Select(item => item.ID).ToArray();
            using (PvbErmReponsitory reponsitory = LinqFactory<PvbErmReponsitory>.Create())
            {
                //相同的ids,不同的ids
                string[] sameids = oldids.Intersect(newids).ToArray();
                string[] diffids = oldids.Where(item => !sameids.Contains(item)).ToArray();

                #region 新增项处理
                var InsertItems = news.Where(item => item.ID == null).Select(item => new Layers.Data.Sqls.PvbErm.PersonalWorkExperiences
                {
                    ID = Layers.Data.PKeySigner.Pick(PKeyType.WorkExp),
                    StaffID = this.ID,
                    StartTime = item.StartTime,
                    EndTime = item.EndTime,
                    Company = item.Company,
                    Position = item.Position,
                    Salary = item.Salary,
                    LeaveReason = item.LeaveReason,
                    Phone = item.Phone,
                });
                reponsitory.Insert(InsertItems.ToArray());
                #endregion

                #region 更新项处理
                foreach (var id in sameids)
                {
                    var newItem = news.Where(t => t.ID == id).FirstOrDefault();
                    reponsitory.Update<Layers.Data.Sqls.PvbErm.PersonalWorkExperiences>(new
                    {
                        StartTime = newItem.StartTime,
                        EndTime = newItem.EndTime,
                        Company = newItem.Company,
                        Position = newItem.Position,
                        Salary = newItem.Salary,
                        LeaveReason = newItem.LeaveReason,
                        Phone = newItem.Phone,
                    }, item => item.ID == newItem.ID && item.StaffID == newItem.StaffID);
                }
                #endregion

                #region 删除项处理
                reponsitory.Delete<Layers.Data.Sqls.PvbErm.PersonalWorkExperiences>(item => diffids.Contains(item.ID));
                #endregion
            }
        }

        /// <summary>
        /// 保存家庭成员
        /// </summary>
        /// <param name="newFiles"></param>
        /// <param name="oldFiles"></param>
        private void SaveFamilyMembers(IEnumerable<PersonalFamilyMember> news, IEnumerable<PersonalFamilyMember> olds)
        {
            if (news == null)
            {
                return;
            }
            string[] newids = news.Select(item => item.ID).ToArray();
            string[] oldids = olds.Select(item => item.ID).ToArray();
            using (PvbErmReponsitory reponsitory = LinqFactory<PvbErmReponsitory>.Create())
            {
                //相同的ids,不同的ids
                string[] sameids = oldids.Intersect(newids).ToArray();
                string[] diffids = oldids.Where(item => !sameids.Contains(item)).ToArray();

                #region 新增项处理
                var InsertItems = news.Where(item => item.ID == null).Select(item => new Layers.Data.Sqls.PvbErm.PersonalFamilyMembers
                {
                    ID = Layers.Data.PKeySigner.Pick(PKeyType.FamilyMember),
                    StaffID = this.ID,
                    Name = item.Name,
                    Relation = item.Relation,
                    Age = item.Age,
                    Company = item.Company,
                    Position = item.Position,
                    Phone = item.Phone,
                });
                reponsitory.Insert(InsertItems.ToArray());
                #endregion

                #region 更新项处理
                foreach (var id in sameids)
                {
                    var newItem = news.Where(t => t.ID == id).FirstOrDefault();
                    reponsitory.Update<Layers.Data.Sqls.PvbErm.PersonalFamilyMembers>(new
                    {
                        Name = newItem.Name,
                        Relation = newItem.Relation,
                        Age = newItem.Age,
                        Company = newItem.Company,
                        Position = newItem.Position,
                        Phone = newItem.Phone,
                    }, item => item.ID == newItem.ID && item.StaffID == newItem.StaffID);
                }
                #endregion

                #region 删除项处理
                reponsitory.Delete<Layers.Data.Sqls.PvbErm.PersonalFamilyMembers>(item => diffids.Contains(item.ID));
                #endregion
            }
        }
    }

    public class PersonalWorkExperience : IUnique
    {
        public string ID { get; set; }

        public string StaffID { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        /// <summary>
        /// 工作单位
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// 职务
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// 薪资
        /// </summary>
        public decimal? Salary { get; set; }

        /// <summary>
        /// 离职原因
        /// </summary>
        public string LeaveReason { get; set; }

        /// <summary>
        /// 单位电话
        /// </summary>
        public string Phone { get; set; }

    }

    public class PersonalFamilyMember : IUnique
    {
        public string ID { get; set; }

        public string StaffID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 与本人关系
        /// </summary>
        public string Relation { get; set; }

        /// <summary>
        /// 工作单位
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// 职业
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { get; set; }

    }
}