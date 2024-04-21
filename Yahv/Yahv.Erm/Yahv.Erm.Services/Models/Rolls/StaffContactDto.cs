using System;
using System.Collections.Generic;
using System.Linq;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Layers.Linq;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views;
using Yahv.Underly;
using Yahv.Utils;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Erm.Services.Models.Rolls
{
    /// <summary>
    /// 大赢家通讯录接口返回信息
    /// </summary>
    public class StaffContactDto
    {
        #region 属性
        /*
         * "ID": 101,
          "姓名": "管理员",
          "昵称": "admin",
          "助记码": "GLY ",
          "部门": "公司",
          "部门ID": 101,
          "分公司": "总公司",
          "分公司ID": 100,
          "岗位": "应用技术人员（0绩效考核）",
          "电话": "",
          "手机": "1"
         */

        public int ID { get; set; }
        public string 姓名 { get; set; }
        public string 昵称 { get; set; }
        public string 助记码 { get; set; }
        public string 部门 { get; set; }
        public string 部门ID { get; set; }
        public string 分公司 { get; set; }
        public string 分公司ID { get; set; }
        public string 岗位 { get; set; }
        public string 电话 { get; set; }
        public string 手机 { get; set; }
        #endregion

        /// <summary>
        /// 新增
        /// </summary>
        public string Enter()
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            using (var tran = repository.OpenTransaction())
            using (var postionsView = new PostionsAll())
            using (var adminsView = new Erm.Services.Views.AdminsAll())
            {
                try
                {
                    var staffId = PKeySigner.Pick(PKeyType.Staff);

                    var staff = new Layers.Data.Sqls.PvbErm.Staffs()
                    {
                        ID = staffId,
                        Name = this.姓名,
                        Gender = 1,       //默认男
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        //Status = (int)StaffStatus.Applied,   //默认为面试申请中
                        Status = (int)StaffStatus.Normal,   //默认为面试申请中


                        Code = staffId.Replace("Staff", ""),
                        SelCode = staffId.Replace("Staff", ""),
                        DyjCompanyCode = this.分公司ID,
                        DyjDepartmentCode = this.部门ID,
                        DyjCode = this.ID.ToString(),
                        PostionID = postionsView.FirstOrDefault(item => item.Name == this.岗位)?.ID,

                    };
                    repository.Insert(staff);

                    //添加员工信息 电话
                    var personal = new Layers.Data.Sqls.PvbErm.Personals()
                    {
                        ID = staffId,
                        Mobile = this.电话,
                    };
                    repository.Insert(personal);

                    var adminId = PKeySigner.Pick(PKeyType.Admin);
                    var admin = new Layers.Data.Sqls.PvbErm.Admins()
                    {
                        ID = adminId,
                        RoleID = FixedRole.NewStaff.GetFixedID(),
                        StaffID = staffId,
                        RealName = this.姓名,
                        UserName = GetAccountByName(adminsView, this.姓名),
                        Password = "123456".MD5("x").PasswordOld(),
                        CreateDate = DateTime.Now,
                        SelCode = adminId.Replace(nameof(PKeyType.Admin), ""),
                        Status = (int)GeneralStatus.Normal,
                    };
                    repository.Insert(admin);
                    tran.Commit();

                    return admin.ID;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 批量新增员工
        /// </summary>
        /// <param name="list"></param>
        public void Batch(List<StaffContactDto> list)
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            using (var tran = repository.OpenTransaction())
            using (var postionsView = new PostionsAll())
            using (var adminsView = new Erm.Services.Views.AdminsAll())
            {
                try
                {
                    var staffIds = PKeySigner.Series(PKeyType.Staff, list.Count);
                    var adminIds = PKeySigner.Series(PKeyType.Admin, list.Count);

                    var staffList = new List<Layers.Data.Sqls.PvbErm.Staffs>();
                    var persList = new List<Layers.Data.Sqls.PvbErm.Personals>();
                    var adminList = new List<Layers.Data.Sqls.PvbErm.Admins>();

                    string staffId = string.Empty;
                    string adminId = string.Empty;
                    StaffContactDto staff;

                    for (int i = 0; i < list.Count; i++)
                    {
                        staffId = staffIds[i];
                        adminId = adminIds[i];
                        staff = list[i];

                        staffList.Add(new Staffs()
                        {
                            ID = staffId,
                            Name = staff.姓名,
                            Gender = 1,       //默认男
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            Status = (int)StaffStatus.Normal,

                            Code = staffId.Replace("Staff", ""),
                            SelCode = staffId.Replace("Staff", ""),
                            DyjCompanyCode = staff.分公司ID,
                            DyjDepartmentCode = staff.部门ID,
                            DyjCode = staff.ID.ToString(),
                            PostionID = postionsView.FirstOrDefault(item => item.Name == this.岗位)?.ID,
                        });

                        persList.Add(new Personals()
                        {
                            ID = staffId,
                            Mobile = staff.电话,
                        });

                        adminList.Add(new Admins()
                        {
                            ID = adminId,
                            RoleID = FixedRole.NewStaff.GetFixedID(),
                            StaffID = staffId,
                            RealName = staff.姓名,
                            UserName = staff.昵称,
                            Password = "123456".MD5("x").PasswordOld(),
                            CreateDate = DateTime.Now,
                            SelCode = adminId.Replace(nameof(PKeyType.Admin), ""),
                            Status = (int)GeneralStatus.Normal,
                        });
                    }

                    if (staffList.Count > 0 && persList.Count > 0 && adminList.Count > 0)
                    {
                        repository.Insert<Staffs>(staffList);
                        repository.Insert<Staffs>(persList);
                        repository.Insert<Staffs>(adminList);
                    }

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 根据姓名获取账号
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetAccountByName(IQueryable<Admin> adminsView, string name)
        {
            string result = string.Empty;

            if (string.IsNullOrWhiteSpace(name)) return result;

            result = PinyinHelper.GetPinyin(name.Trim());
            int num = 1;
            while (adminsView.Where(item => item.UserName == result).Count() >= 1)
            {
                if (num == 1)
                {
                    result = result + num.ToString();
                }
                else
                {
                    result = result.Replace((num - 1).ToString(), num.ToString());
                }
                num++;
            }

            return result;
        }
    }
}